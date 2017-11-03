#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
using System;

namespace UnityEngine.Windows.Speech
{
    public sealed class DictationRecognizer : IDisposable
    {
        public DictationRecognizer() {}
        public DictationRecognizer(ConfidenceLevel confidenceLevel) {}
        public DictationRecognizer(DictationTopicConstraint topic) {}
        public DictationRecognizer(ConfidenceLevel minimumConfidence, DictationTopicConstraint topic) {}

        ~DictationRecognizer() {}

        public float InitialSilenceTimeoutSeconds { get; set; }
        public SpeechSystemStatus Status { get; }
        public float AutoSilenceTimeoutSeconds { get; set; }

        public event DictationHypothesisDelegate DictationHypothesis;
        public event DictationResultDelegate DictationResult;
        public event DictationCompletedDelegate DictationComplete;
        public event DictationErrorHandler DictationError;

        public void Dispose() {}
        public void Start() {}
        public void Stop() {}

        public delegate void DictationCompletedDelegate(DictationCompletionCause cause);
        public delegate void DictationHypothesisDelegate(string text);
        public delegate void DictationResultDelegate(string text, ConfidenceLevel confidence);
        public delegate void DictationErrorHandler(string error, int hresult);
    }

    public enum ConfidenceLevel
    {
        High = 0,
        Medium = 1,
        Low = 2,
        Rejected = 3
    }
}
#endif
