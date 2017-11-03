#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Charlie.Giphy;
using Asyncoroutine;

namespace Charlie.Sticker {
    [CustomEditor(typeof(StickerService))]
    class StickerServiceInspector : Editor
    {
        private StickerService stickerService;

        private static bool showTestConsole = false;

        private static Texture testConsoleTexture;
        private static Texture testConsoleComposedTexture;
        private static Vector2 testConsoleScale;
        private static Vector2 testConsoleOffset;
        private static string testConsoleGiphyStickerID;

        public void OnEnable()
        {
            stickerService = (StickerService)target;
            testConsoleTexture = Texture2D.whiteTexture;
            testConsoleScale = Vector2.one;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (Application.isPlaying)
            {
                showTestConsole = EditorGUILayout.Foldout(showTestConsole, new GUIContent("Test Console"));
                if (showTestConsole)
                {
                    EditorGUI.indentLevel++;

                    testConsoleGiphyStickerID = EditorGUILayout.TextField(new GUIContent("Giphy Sticker ID"), testConsoleGiphyStickerID);

                    testConsoleTexture = EditorGUILayout.ObjectField(
                        testConsoleTexture, typeof(Texture), true,
                        GUILayout.Width(Screen.width - 40.0f),
                        GUILayout.Height(Screen.width - 40.0f)
                    ) as Texture;

                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(
                        testConsoleComposedTexture, typeof(RenderTexture), true,
                        GUILayout.Width(Screen.width - 40.0f),
                        GUILayout.Height(Screen.width - 40.0f)
                    );
                    GUI.enabled = true;

                    testConsoleScale = EditorGUILayout.Vector2Field(new GUIContent("Scale"), testConsoleScale);
                    testConsoleOffset = EditorGUILayout.Vector2Field(new GUIContent("Offset"), testConsoleOffset);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                    bool composeClicked = GUILayout.Button(new GUIContent("Compose"));
                    GUILayout.EndHorizontal();

                    if (composeClicked)
                    {
                        HandleComposeClicked();
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("A test console will be available here when you enter play mode.",
                    MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private async void HandleComposeClicked()
        {
            Response<Charlie.Giphy.Sticker> response = await GiphyService.Instance.GetByID(testConsoleGiphyStickerID);
            ISticker sticker = await GiphyService.Instance.Export(response.data);
            testConsoleComposedTexture = stickerService.AddStickerToTexture(
                sticker, testConsoleTexture, testConsoleScale, testConsoleOffset);
            await new WaitForEndOfFrame();
            Repaint();
        }
    }
}
#endif
