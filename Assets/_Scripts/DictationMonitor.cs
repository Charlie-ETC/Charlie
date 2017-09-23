using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DictationMonitor : MonoBehaviour {

    private ApiaiService apiaiService;
    private WatsonTTSService watsonTTSService;

    private TextMesh textMesh;
    private AudioSource audioSource;

    private string apiaiSessionId;

    private string lastRequest;
    private string lastResponse;

    // keyword commands collection
    private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

    void Start() {
        textMesh = GetComponent<TextMesh>();
        audioSource = GetComponent<AudioSource>();
        apiaiService = GetComponent<ApiaiService>();
        watsonTTSService = GetComponent<WatsonTTSService>();
        apiaiSessionId = apiaiService.CreateSession();

        // initiate keywords. Case sensitive
        keywords.Add("tap it", KeywordCommands.OnTapIt);
        keywords.Add("drag it", KeywordCommands.OnDragIt);
        keywords.Add("place it here", KeywordCommands.OnPlaceItHere);
        keywords.Add("reset", KeywordCommands.OnReset);
    }

    public async void HandleDictationResult(string text, string confidenceLevel)
    {
        lastRequest = text;
        textMesh.text = $"Request: {lastRequest}\nResponse: {lastResponse}";
        Response response = await apiaiService.Query(apiaiSessionId, text);
        string speech = response.result.speech;
        AudioClip clip = await watsonTTSService.Synthesize(speech);
        audioSource.PlayOneShot(clip);
        CharlieManager.Instance.SpeakAnimation(clip.length);
        lastResponse = speech;
    }

    public void HandleKeywordCommand(string text, string confidenceLevel)
    {
        Debug.Log("HandleKeywordCommand");

        System.Action action;
        if (keywords.TryGetValue(text, out action)) {
            action.Invoke();
        }
    }

}
