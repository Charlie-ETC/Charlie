using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows.Speech;

public class DictationRecognizer : MonoBehaviour {


    [System.Serializable]
    public class DictationCompleteEvent : UnityEvent<string> { }

    [System.Serializable]
    public class DictationErrorEvent : UnityEvent<string, int> { }

    [System.Serializable]
    public class DictationHypothesisEvent : UnityEvent<string> { }

    [System.Serializable]
    public class DictationResultEvent : UnityEvent<string, string> { }

    private UnityEngine.Windows.Speech.DictationRecognizer dictationRecognizer;

    public DictationCompleteEvent dictationCompleteEvent;
    public DictationErrorEvent dictationErrorEvent;
    public DictationHypothesisEvent dictationHypothesisEvent;
    public DictationResultEvent dictationResultEvent;

    [Tooltip("The time length in seconds before dictation recognizer session " +
        "ends due to lack of audio input.")]
    public float autoSilenceTimeoutSeconds = 20.0f;

    [Tooltip("The time length in seconds before dictation recognizer session " +
        "ends due to lack of audio input in case there was no audio heard in " +
        "the current session.")]
    public float initialSilenceTimeoutSeconds = 5.0f;

    private bool isDestroying = false;

	// Use this for initialization
	void Start () {
        dictationRecognizer = new UnityEngine.Windows.Speech.DictationRecognizer();
        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
        dictationRecognizer.DictationComplete += OnDictationComplete;
        dictationRecognizer.DictationError += OnDictationError;
        dictationRecognizer.AutoSilenceTimeoutSeconds = autoSilenceTimeoutSeconds;
        dictationRecognizer.InitialSilenceTimeoutSeconds = initialSilenceTimeoutSeconds;
        dictationRecognizer.Start();
	}

    public string FakeDictationResult;
    public bool TriggerFakeDictationResult;

    private void Update()
    {
        if (TriggerFakeDictationResult)
        {
            OnDictationResult(FakeDictationResult, ConfidenceLevel.High);
            TriggerFakeDictationResult = false;
            FakeDictationResult = "";
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetDictationRecognizer();
        }
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.Log($"[DictationRecognizer] OnDictationError (error: {error}, result: {hresult})");
        dictationErrorEvent.Invoke(error, hresult);
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        Debug.Log($"[DictationRecognizer] OnDictationComplete (cause: {cause.ToString()})");
        dictationCompleteEvent.Invoke(cause.ToString());
        if (!isDestroying)
        {
            Debug.Log("[DictationRecognizer] Restarting DictationRecognizer after completion");
            dictationRecognizer.Start();
        }
    }

    private void OnDictationHypothesis(string text)
    {
        Debug.Log($"[DictationRecognizer] OnDictationHypothesis (text: {text})");
        dictationHypothesisEvent.Invoke(text);
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log($"[DictationRecognizer] OnDictationResult (text: {text}, confidence: {confidence.ToString()})");
        dictationResultEvent.Invoke(text, confidence.ToString());
        dictationRecognizer.Stop();
    }

    private void OnDestroy()
    {
        Debug.Log("[DictationRecognizer] Destroying DictationRecognizer");
        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            isDestroying = true;
            dictationRecognizer.Stop();
        }
    }

    public void ResetDictationRecognizer()
    {
        dictationRecognizer.Stop();
        dictationRecognizer.Start();
    }
}
