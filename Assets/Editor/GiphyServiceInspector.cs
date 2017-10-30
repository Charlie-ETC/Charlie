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
                        testConsoleSearchQuery = EditorGUILayout.TextField(new GUIContent("Query", "Search query term or phrase"), testConsoleSearchQuery);
                        testConsoleSearchLimit = EditorGUILayout.IntField(new GUIContent("Limit", "The maximum number of records to return"), testConsoleSearchLimit);
                        testConsoleSearchOffset = EditorGUILayout.IntField(new GUIContent("Offset", "An optional results offset"), testConsoleSearchOffset);
                        testConsoleSearchRating = EditorGUILayout.TextField(new GUIContent("Rating", "Filters results by rating"), testConsoleSearchRating);
                        testConsoleSearchLang = EditorGUILayout.TextField(new GUIContent("Language", "Specify default country for regional content"), testConsoleSearchLang);

                        GUILayout.BeginHorizontal();
                        GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                        GUI.enabled = !searching;
                        bool searchClicked = GUILayout.Button(new GUIContent(searching ? "Searching" : "Search"));
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
