#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Charlie.Giphy
{
    [CustomEditor(typeof(GiphyService))]
    class GiphyServiceInspector : Editor
    {
        private GiphyService giphyService;

        private static bool showTestConsole = true;
        private static bool showTestConsoleSearch = true;

        private static string testConsoleSearchQuery = "Cat";
        private static int testConsoleSearchLimit = 25;
        private static int testConsoleSearchOffset = 0;
        private static string testConsoleSearchRating = "G";
        private static string testConsoleSearchLang = "en";

        private static bool searching = false;

        public void OnEnable()
        {
            giphyService = (GiphyService)target;
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

                    showTestConsoleSearch = EditorGUILayout.Foldout(showTestConsoleSearch, new GUIContent("Search"));
                    if (showTestConsoleSearch)
                    {
                        EditorGUI.indentLevel++;
                        testConsoleSearchQuery = EditorGUILayout.TextField(new GUIContent("Query"), testConsoleSearchQuery);
                        testConsoleSearchLimit = EditorGUILayout.IntField(new GUIContent("Limit"), testConsoleSearchLimit);
                        testConsoleSearchOffset = EditorGUILayout.IntField(new GUIContent("Offset"), testConsoleSearchOffset);
                        testConsoleSearchRating = EditorGUILayout.TextField(new GUIContent("Rating"), testConsoleSearchRating);
                        testConsoleSearchLang = EditorGUILayout.TextField(new GUIContent("Language"), testConsoleSearchLang);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !searching;
                        bool searchClicked = GUILayout.Button(new GUIContent(searching ? "Querying" : "Search"));
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();

                        if (searchClicked)
                        {
                            HandleSearchClicked();
                        }

                        EditorGUI.indentLevel--;
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

        async public void HandleSearchClicked()
        {
            searching = true;
            Repaint();

            Response<List<Sticker>> response = await giphyService.Search(testConsoleSearchQuery);

            searching = false;
            Repaint();
        }
    }
}
#endif
