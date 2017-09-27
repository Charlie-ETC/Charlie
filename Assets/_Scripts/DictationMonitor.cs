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
        textMesh.text = text;
    }

    public async void HandleDictationResult(string text, string confidenceLevel)
    {
        lastRequest = text;
        Response response = await apiaiService.Query(apiaiSessionId, text);

        // We managed to get an intent, dispatch it.
        if (response.result.metadata.intentName != null)
        {
            DispatchIntent(response.result.metadata.intentName, response);
        }

        // API.ai crafted a speech response for us, use it.
        if (response.result.speech.Length != 0)
        {
            string speech = response.result.speech;
            AudioClip clip = await watsonTTSService.Synthesize(speech);
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

    public void DragHandler()
    {
        Debug.Log("DragHandler invoked!");
    }

    public void TakePictureHandler()
    {
        Debug.Log("Taking a picture!");
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

                    Debug.Log("Uploading picture to Twitter");
                    Media media = await twitterService.UploadMedia(jpegData);
                    twitterService.TweetWithMedia("hello!", new string[1] { media.mediaIdString });
                    //spatialMappingManager.StartObserver();

                    capture.StopPhotoModeAsync(delegate (PhotoCapture.PhotoCaptureResult result3)
                    {
                        Debug.Log("Stopping photo mode");
                    });
                });
            });
        });
    }
}
