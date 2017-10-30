#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DictationMonitor))]
public class DictationMonitorInspector : Editor
{
    private static bool showTestConsole = false;
    private static string testConsoleEventName;
    private static bool querying;

    private DictationMonitor monitor;

    public void OnEnable()
    {
        monitor = (DictationMonitor)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            showTestConsole = EditorGUILayout.Foldout(showTestConsole, new GUIContent("Test Console"));
            if (showTestConsole)
            {
                EditorGUI.indentLevel++;
                testConsoleEventName = EditorGUILayout.TextField(new GUIContent("Event"), testConsoleEventName);

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
        }
    }

    private async void HandleQueryClicked()
    {
        querying = true;
        Repaint();
        await monitor.TriggerApiaiEvent(testConsoleEventName);
        querying = false;
        Repaint();
    }
}

#endif
