using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictationMonitor : MonoBehaviour {

    private TextMesh textMesh;
    
	void Start () {
        textMesh = GetComponent<TextMesh>();
	}

    public void HandleDictationResult(string text, string confidenceLevel)
    {
        textMesh.text = text;
    }
}
