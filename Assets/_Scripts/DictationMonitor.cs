using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;
using HoloToolkit.Unity.SpatialMapping;

public class DictationMonitor : MonoBehaviour {

    public SpatialMappingManager spatialMappingManager;
    public List<IntentHandler> intentHandlers;

    private ApiaiService apiaiService;
    private WatsonTTSService watsonTTSService;
    private TwitterService twitterService;

    private TextMesh textMesh;
    private AudioSource audioSource;

    private Dictionary<string, IntentHandler> intentHandlerIndex = new Dictionary<string, IntentHandler>();
    private string apiaiSessionId;

    private string lastRequest;
    private string lastResponse;

    void Start() {
        textMesh = GetComponent<TextMesh>();
        audioSource = GetComponent<AudioSource>();
        apiaiService = GetComponent<ApiaiService>();
        watsonTTSService = GetComponent<WatsonTTSService>();
        twitterService = GetComponent<TwitterService>();
        apiaiSessionId = apiaiService.CreateSession();

        // At startup, index the intentHandler.
        intentHandlers.ForEach(handler => intentHandlerIndex.Add(handler.name, handler));

        // Usage instructions for Twitter:
        // Media media = await twitterService.UploadMedia(File.ReadAllBytes("WIN_20161017_22_43_37_Pro.jpg"));
        // twitterService.TweetWithMedia("hello!", new string[1] { media.mediaIdString });
    }

    public void HandleDictationHypothesis(string text)
    {
        textMesh.text = $"Request: {text}\nResponse: {lastResponse}";
    }

    public async void HandleDictationResult(string text, string confidenceLevel)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();
        lastRequest = text;
        Response response = await apiaiService.Query(apiaiSessionId, text);
        Debug.Log($"[DictationMonitor] perf: API.ai query took {stopwatch.ElapsedMilliseconds}ms");
        stopwatch.Reset();

        // We managed to get an intent, dispatch it.
        if (response.result.metadata.intentName != null)
        {
            stopwatch.Start();
            DispatchIntent(response.result.metadata.intentName, response);
            Debug.Log($"[DictationMonitor] intentName:{response.result.metadata.intentName}, perf: DispatchIntent took {stopwatch.ElapsedMilliseconds}ms");
            foreach (var kv in response.result.parameters)
            {
                Debug.Log($"{kv.Key}={kv.Value}");
            }
            stopwatch.Reset();
        }

        // API.ai crafted a speech response for us, use it.
        if (response.result.speech.Length != 0)
        {
            string speech = response.result.speech;

            stopwatch.Start();
            AudioClip clip = await watsonTTSService.Synthesize(speech);
            Debug.Log($"[DictationMonitor] perf: Watson synthesis took {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Reset();

            audioSource.PlayOneShot(clip);
            CharlieManager.Instance.SpeakAnimation(clip.length);
            lastResponse = speech;
            textMesh.text = $"Request: {lastRequest}\nResponse: {lastResponse}";
        }
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

    async public void HandleDebugGetContexts()
    {
        List<Context> contexts = await apiaiService.GetContexts(apiaiSessionId);
        foreach (Context context in contexts)
        {
            Debug.Log($"[DictationMonitor] context name={context.name}, " +
                $"lifespan={context.lifespan}, parameters={context.parameters}");
        }
    }

    public void HandleTakePicture()
    {
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
    }
}
