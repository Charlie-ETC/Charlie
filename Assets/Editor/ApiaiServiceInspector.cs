#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

[CustomEditor(typeof(ApiaiService))]
class ApiaiServiceInspector : Editor
{
    private static bool showTestConsole = true;
    private static string testConsoleSessionId = "";

    private static bool showQuery = true;
    private static bool querying = false;
    private static string testConsoleQuery = "Hello world!";
    private static string testConsoleResponseSpeech = "The returned speech will be displayed here.";

    private static bool showContext = true;
    private static bool gettingContext = false;
    private static string testConsoleContext = "";

    private ApiaiService apiaiService;

    public void OnEnable()
    {
        apiaiService = (ApiaiService)target;
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

                // Show the session ID.
                if (testConsoleSessionId.Length == 0)
                {
                    testConsoleSessionId = apiaiService.CreateSession();
                }
                EditorGUILayout.TextField(new GUIContent("Session ID"), testConsoleSessionId);

                // Show the query.
                showQuery = EditorGUILayout.Foldout(showQuery, new GUIContent("Query"));
                if (showQuery)
                {
                    EditorGUI.indentLevel++;
                    testConsoleQuery = EditorGUILayout.TextArea(testConsoleQuery, GUILayout.MinHeight(40));

                    // Show the response text.
                    EditorGUILayout.HelpBox(testConsoleResponseSpeech, MessageType.Info);

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

                    EditorGUI.indentLevel--;
                }

                // Show the contexts.
                showContext = EditorGUILayout.Foldout(showContext, new GUIContent("Context"));
                if (showContext)
                {
                    EditorGUI.indentLevel++;

                    // Show the context response.
                    GUI.enabled = false;
                    EditorGUILayout.TextArea(testConsoleContext, GUILayout.MinHeight(40));
                    GUI.enabled = true;

                    // Show the button.
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                    bool getContextClicked = GUILayout.Button(new GUIContent(gettingContext ? "Getting Context" : "Get Context"));
                    GUILayout.EndHorizontal();

                    if (getContextClicked)
                    {
                        HandleContextClicked();
                    }

                    EditorGUI.indentLevel--;
                }
            }
        }
        else
        {
            EditorGUILayout.HelpBox("A test console will be available here when you enter play mode.",
                MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }

    async public void HandleQueryClicked()
    {
        querying = true;
        Repaint();
        Response response = await apiaiService.Query(testConsoleSessionId, testConsoleQuery);
        testConsoleResponseSpeech = response.result.speech;
        querying = false;
        Repaint();
    }

    async public void HandleContextClicked()
    {
        gettingContext = true;
        Repaint();

        List<Context> contexts = await apiaiService.GetContexts(testConsoleSessionId);
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };
        testConsoleContext = JsonConvert.SerializeObject(contexts, settings);

        gettingContext = false;
        Repaint();
    }
}

#endif
