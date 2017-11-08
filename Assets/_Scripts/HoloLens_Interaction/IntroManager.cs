using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

namespace Charlie
{
    public class IntroManager : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("The name of the next scene to transition to.")]
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

            // Update the text.
            ReadyText.text = "Tap when you're satisfied with the results";
        }

        public void OnMappingDone()
        {
            // Update the text.
            SceneManager.LoadSceneAsync(NextScene, LoadSceneMode.Single);
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            // When stats are good enough, we can finish mapping.
            Debug.Log("[IntroManager]: OnInputClicked");
            SpatialMapper.Instance.FinishMapping();
        }
    }
}
