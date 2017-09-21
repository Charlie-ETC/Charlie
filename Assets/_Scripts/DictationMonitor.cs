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
    
	void Start () {
        textMesh = GetComponent<TextMesh>();
        audioSource = GetComponent<AudioSource>();
        apiaiService = GetComponent<ApiaiService>();
        watsonTTSService = GetComponent<WatsonTTSService>();
        apiaiSessionId = apiaiService.CreateSession();
	}

    public async void HandleDictationResult(string text, string confidenceLevel)
    {
        lastRequest = text;
        Response response = await apiaiService.Query(apiaiSessionId, text);
        string speech = response.result.speech;
        AudioClip clip = await watsonTTSService.Synthesize(speech);
        audioSource.PlayOneShot(clip);
        lastResponse = speech;
    }

    void Update()
    {
        textMesh.text = $"Request: {lastRequest}\nResponse: {lastResponse}";
    }
}
