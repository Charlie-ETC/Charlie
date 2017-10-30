#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Charlie.Unsplash
{
    [CustomEditor(typeof(UnsplashService))]
    class UnsplashServiceInspector : Editor
    {
        private UnsplashService unsplashService;

        private static bool querying = false;

        private static string testConsoleQuery = "";
        private static Texture testConsoleTexture;

        public void OnEnable()
        {
            unsplashService = (UnsplashService)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (Application.isPlaying)
            {
                testConsoleQuery = EditorGUILayout.TextField(new GUIContent("Query"), testConsoleQuery);

                // Show the texture if we have it.
                if (testConsoleTexture != null)
                {
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(
                        testConsoleTexture, typeof(Texture), true,
                        GUILayout.Width(Screen.width - 40.0f),
                        GUILayout.Height(Screen.width - 40.0f)
                    );
                    GUI.enabled = true;
                }

                // Show the query button.
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                GUI.enabled = !querying;
                bool queryClicked = GUILayout.Button(new GUIContent(querying ? "Querying" : "Query"));
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                if (queryClicked)
                {
                    HandleQueryClicked();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("A test console will be available here when you enter play mode.",
                    MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        public async void HandleQueryClicked()
        {
            querying = true;
            Repaint();
            testConsoleTexture = await unsplashService.GetRandomPhoto(testConsoleQuery);
            querying = false;
            Repaint();
        }
    }
}
#endif
