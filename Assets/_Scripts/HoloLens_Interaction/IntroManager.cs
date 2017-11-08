using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

namespace Charlie
{
    public class IntroManager : MonoBehaviour, IInputClickHandler
    {
        public string NextScene;

        [Tooltip("Text element to update when ready to proceed to next scene.")]
        public Text ReadyText;

        // <summary>
        // Use this for initialization
        // </summary>
        protected virtual async void Start()
        {
            Debug.Log("[IntroManager]: Adding global input handler");
            InputManager.AssertIsInitialized();
            InputManager.Instance.AddGlobalListener(gameObject);

            // Wait for spatial mapping to complete...
            Debug.Log("[IntroManager]: Waiting for surfaces...");
            await SpatialMapper.Instance.WaitForSurfaces();

            // When stats are good enough, we can finish mapping.
            SpatialMapper.Instance.FinishMapping();
        }

        public void OnMappingDone()
        {
            // Update the text.
            ReadyText.text = "Scan complete. Tap to continue.";
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            Debug.Log("[IntroManager]: OnInputClicked");
            SceneManager.LoadSceneAsync(NextScene, LoadSceneMode.Single);
        }
    }
}
