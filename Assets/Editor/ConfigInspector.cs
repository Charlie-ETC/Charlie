#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Config))]
public class ConfigInspector : Editor {
    private static bool showIbmWatson;

    SerializedProperty apiaiClientAccessTokenProp;
    SerializedProperty ibmWatsonTtsUrlProp;
    SerializedProperty ibmWatsonUsernameProp;
    SerializedProperty ibmWatsonPasswordProp;

    void OnEnable()
    {
        apiaiClientAccessTokenProp = serializedObject.FindProperty("apiaiClientAccessToken");
        ibmWatsonTtsUrlProp = serializedObject.FindProperty("ibmWatsonTtsUrl");
        ibmWatsonUsernameProp = serializedObject.FindProperty("ibmWatsonUsername");
        ibmWatsonPasswordProp = serializedObject.FindProperty("ibmWatsonPassword");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(apiaiClientAccessTokenProp, new GUIContent("API.ai Token"));
        showIbmWatson = EditorGUILayout.Foldout(showIbmWatson, new GUIContent("IBM Watson"));
        if (showIbmWatson)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(ibmWatsonTtsUrlProp, new GUIContent("TTS Endpoint"));
            EditorGUILayout.PropertyField(ibmWatsonUsernameProp, new GUIContent("Username"));
            EditorGUILayout.PropertyField(ibmWatsonPasswordProp, new GUIContent("Password"));
            EditorGUI.indentLevel--;
        }
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
