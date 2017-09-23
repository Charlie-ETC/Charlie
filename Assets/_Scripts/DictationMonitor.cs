using System.Collections.Generic;
using UnityEngine;

public class DictationMonitor : MonoBehaviour {

    public List<IntentHandler> intentHandlers;

    private ApiaiService apiaiService;
    private WatsonTTSService watsonTTSService;

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
        apiaiSessionId = apiaiService.CreateSession();

        // At startup, index the intentHandler.
        intentHandlers.ForEach(handler => intentHandlerIndex.Add(handler.name, handler));
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
}
