using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictationMonitor : MonoBehaviour {

    private ApiaiService apiaiService;
    private TextMesh textMesh;
    private string apiaiSessionId;

    private string lastRequest;
    private string lastResponse;
    
	void Start () {
        textMesh = GetComponent<TextMesh>();
        apiaiService = GetComponent<ApiaiService>();
        apiaiSessionId = apiaiService.CreateSession();
	}

    public async void HandleDictationResult(string text, string confidenceLevel)
    {
        lastRequest = text;
        Response response = await apiaiService.Query(apiaiSessionId, text);
        lastResponse = response.result.speech;
    }

    void Update()
    {
        textMesh.text = $"Request: {lastRequest}\nResponse: {lastResponse}";
    }
}
