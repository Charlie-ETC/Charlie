using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

// for keyword commands
namespace Charlie
{

    public class SpeechManager : MonoBehaviour
    {

        private KeywordRecognizer keywordRecognizer;

        private Dictionary<string, System.Action> keywords = new Dictionary<string, System.Action>();

        private void Start()
        {
            // add a bunch of keywords
            keywords.Add("tap it", OnTapIt);

            // store keywords string in an array for constructor
            string[] keywordsArray = new string[keywords.Keys.Count];
            keywords.Keys.CopyTo(keywordsArray, 0);

            // initialize keywordRecognizer, register callback func
            keywordRecognizer = new KeywordRecognizer(keywordsArray);
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();
        }


        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            System.Action callBack;

            if (keywords.TryGetValue(args.text, out callBack))
            {
                Debug.Log("speech call back");
                callBack.Invoke();
            }
        }


        // on "tap it" is recognized
        private void OnTapIt()
        {

            //if (GestureManager.Instance.FocusedObject != null)
            //{
            //    CubeCommands cc = GestureManager.Instance.FocusedObject.GetComponentInParent<CubeCommands>();

            //    if (cc != null)
            //    {
            //        cc.OnSelect();
            //    }
            //}
        }


    }
}
