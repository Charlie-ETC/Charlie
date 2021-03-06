using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
#if UNITY_WSA
using UnityEngine.XR.WSA.WebCam;
#endif
using Asyncoroutine;
using HoloToolkit.Unity.SpatialMapping;

using Charlie.Apiai;
using Charlie.WatsonTTS;
using Charlie.Twitter;

public class DictationMonitor : MonoBehaviour {

    public static DictationMonitor Instance { get; private set; }

    public SpatialMappingManager spatialMappingManager;
    public List<IntentHandler> intentHandlers;

    private ApiaiService apiaiService;
    private WatsonTTSService watsonTTSService;
    public TwitterService twitterService;

    private TextMesh textMesh;
    //private AudioSource audioSource;

    private Dictionary<string, IntentHandler> intentHandlerIndex = new Dictionary<string, IntentHandler>();
    public string apiaiSessionId;

    private string lastRequest;
    private string lastResponse;   

    public FsmEventGenerator fsmEvent;
    public CharlieSlackLog charlieSlackLog;
    public AudioSource charlieAudio;
    public Animator charlieAnimator;
    //[HideInInspector]
    public bool plotSpeaking; // set by ActionSpeak
    public bool MissedQ;

    // singleton instance
    private void Awake()
    {
        Instance = this;
    }

    void Start() {
        textMesh = GetComponent<TextMesh>();
        //audioSource = GetComponent<AudioSource>();
        apiaiService = ApiaiService.Instance;
        watsonTTSService = WatsonTTSService.Instance;
        twitterService = TwitterService.Instance;
        apiaiSessionId = apiaiService.CreateSession();
        charlieSlackLog.SetAPISessionID(apiaiSessionId);
        MissedQ = false;
        plotSpeaking = false;
        
        // At startup, index the intentHandler.
        intentHandlers.ForEach(handler => intentHandlerIndex.Add(handler.name, handler));

        // Usage instructions for Twitter:
        // Media media = await twitterService.UploadMedia(File.ReadAllBytes("WIN_20161017_22_43_37_Pro.jpg"));
        // twitterService.TweetWithMedia("hello!", new string[1] { media.mediaIdString });
    }

    public async Task TriggerApiaiEvent(string eventName)
    {
        Response response = await apiaiService.Query(apiaiSessionId, eventName, true);
        await SpeakApiaiResponse(response);
    }

    public void HandleDictationHypothesis(string text)
    {
        transform.Find("Hypothesis").GetComponent<TextMesh>().text = $"({Time.time.ToString("0.00")})   :   [{text}]";
        fsmEvent.HandleDictationHypothesis(text);
        //textMesh.text = $"Request: {text}\nResponse: {lastResponse}";
    }

    public void HandleDictationComplete(string text)
    {
        transform.Find("Complete").GetComponent<TextMesh>().text =   $"({Time.time.ToString("0.00")})   :   [{text}]";
        fsmEvent.HandleDictationComplete(text);
    }

    public void HandleDictationError(string text, int i)
    {
        transform.Find("Error").GetComponent<TextMesh>().text =      $"({Time.time.ToString("0.00")})   :   [{text}]   code:{i}";
        fsmEvent.HandleDictationError(text);
    }

    public async void HandleDictationResult(string text, string confidenceLevel)
    {
        transform.Find("Result").GetComponent<TextMesh>().text =     $"({Time.time.ToString("0.00")})   :   [{text}]   confidence:{confidenceLevel}";
        Debug.Log(text + " " + MissedQ);
        fsmEvent.HandleDictationResult(text);

        // log what user says to slack
        if (!string.IsNullOrEmpty(text)) charlieSlackLog.SlackLog("user", text);

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();
        lastRequest = text;
        Response response = await apiaiService.Query(apiaiSessionId, text); // context is attached to apiai sessionid
        Debug.Log($"[DictationMonitor] perf: API.ai query took {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Reset();

        // send this to FSM anyway
        if (response != null) fsmEvent.HandleResponse(response);
        
        // We managed to get an intent, dispatch it.
        //if (response.result.metadata.intentName != null)
        //{
        //    stopwatch.Start();
        //    DispatchIntent(response.result.metadata.intentName, response);
        //    Debug.Log($"[DictationMonitor] intentName:{response.result.metadata.intentName}, perf: DispatchIntent took {stopwatch.ElapsedMilliseconds}ms");
        //    foreach (var kv in response.result.parameters)
        //    {
        //        Debug.Log($"{kv.Key}={kv.Value}");
        //    }
        //    stopwatch.Reset();
        //}

        //await SpeakApiaiResponse(response);
    }

    public async Task<bool> SpeakText(string speech)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        // API.ai crafted a speech response for us, use it.
        if (speech.Length != 0)
        {
            stopwatch.Start();
            AudioClip clip = await watsonTTSService.Synthesize(speech);
            Debug.Log($"[DictationMonitor] perf: Watson synthesis took {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Reset();

            // audio play
            if (charlieAudio.isPlaying)
            {
                if (plotSpeaking)
                {
                    MissedQ = true; // won't stop current plot speak, but note that a question is missed
                    return false;
                }
                else
                {
                    // stop speaking to play the new clip
                    charlieAudio.Stop();
                    charlieAudio.clip = clip;
                    charlieAudio.Play();
                    // log what charlie says to Slack
                    charlieSlackLog.SlackLog("charlie", speech);
                }
            }
            else
            {
                charlieAudio.clip = clip;
                charlieAudio.Play();
                // log what charlie says to Slack
                charlieSlackLog.SlackLog("charlie", speech);
            }

            charlieAnimator.SetBool("toTalk", true); // for facial animation
            charlieAnimator.SetInteger("toTalkBody", Random.Range(1, 9)); // for body talk animation

            await new WaitForSeconds(clip.length);

            if (charlieAudio.clip == clip)
            {
                charlieAnimator.SetBool("toTalk", false); // for facial animation
                charlieAnimator.SetInteger("toTalkBody", 0); // for body talk animation
            }


            //audioSource.PlayOneShot(clip);
            //CharlieManager.Instance.SpeakAnimation(clip.length);
            lastResponse = speech;
            textMesh.text = $"Request: {lastRequest}\nResponse: {lastResponse}";
            return true;
        }
        else
        {
            return false;
        }
    }

    /**
     * Speaks the speech that was returned in an API.ai response object.
     * If the speech cannot be found, this method does nothing.
     *
     * @param response The response returned from API.ai
     * @return A task that resolves to a boolean when the speech was
     *      downloaded from Watson and a play request was sent to Unity.
     *      The boolean is false if the response doesn't contain any speech.
     */
    public async Task<bool> SpeakApiaiResponse(Response response)
    {
        return await SpeakText(response.result.speech);
    }

    public void DispatchIntent(string intent, Response response)
    {
        Debug.Log($"Handling intent: {response.result.metadata.intentName}");
        IntentHandler handler;
        bool found = intentHandlerIndex.TryGetValue(intent, out handler);
        if (found)
        {
            handler.unityEvent.Invoke();
        }
        else
        {
            Debug.Log($"Intent {intent} does not have a handler.");
        }
    }

    public void HandleDrag()
    {
        Debug.Log("[DictationMonitor] DragHandler invoked!");
    }

    public void HandleTakePicture()
    {
        #if UNITY_WSA
        Debug.Log("[DictationMonitor] Taking a picture!");
        Resolution resolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Texture2D texture = new Texture2D(resolution.width, resolution.height);
        //spatialMappingManager.StopObserver();

        PhotoCapture.CreateAsync(true, delegate (PhotoCapture capture)
        {
            CameraParameters cameraParams = new CameraParameters()
            {
                hologramOpacity = 0.8f,
                cameraResolutionWidth = resolution.width,
                cameraResolutionHeight = resolution.height,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            capture.StartPhotoModeAsync(cameraParams, delegate (PhotoCapture.PhotoCaptureResult result)
            {
                capture.TakePhotoAsync(async delegate (PhotoCapture.PhotoCaptureResult result2, PhotoCaptureFrame frame)
                {
                    frame.UploadImageDataToTexture(texture);
                    byte[] jpegData = ImageConversion.EncodeToJPG(texture, 80);

                    Debug.Log("[DictationMonitor] Uploading picture to Twitter");
                    Media media = await twitterService.UploadMedia(jpegData);
                    twitterService.TweetWithMedia("hello!", new string[1] { media.mediaIdString });
                    //spatialMappingManager.StartObserver();

                    capture.StopPhotoModeAsync(delegate (PhotoCapture.PhotoCaptureResult result3)
                    {
                        Debug.Log("[DictationMonitor] Stopping photo mode");
                    });
                });
            });
        });
        #endif
    }
}
