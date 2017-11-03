#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Config))]
public class ConfigInspector : Editor {
    private static bool showIbmWatson;
    private static bool showTwitter;
    private static bool showUnsplash;
    private static bool showSlack;
    private static bool showGiphy;

    SerializedProperty apiaiClientAccessTokenProp;
    SerializedProperty ibmWatsonTtsUrlProp;
    SerializedProperty ibmWatsonUsernameProp;
    SerializedProperty ibmWatsonPasswordProp;
    SerializedProperty twitterConsumerKeyProp;
    SerializedProperty twitterConsumerSecretProp;
    SerializedProperty twitterAccessTokenProp;
    SerializedProperty twitterAccessTokenSecretProp;
    SerializedProperty unsplashAppIdProp;
    SerializedProperty unsplashImageSizeProp;
    SerializedProperty slackWebhookUrlProp;
    SerializedProperty slackCharlieIconProp;
    SerializedProperty slackUserIconProp;
    SerializedProperty slackChannelProp;
    SerializedProperty giphyDebugProp;
    SerializedProperty giphyApiKeyProp;

    void OnEnable()
    {
        apiaiClientAccessTokenProp = serializedObject.FindProperty("apiaiClientAccessToken");
        ibmWatsonTtsUrlProp = serializedObject.FindProperty("ibmWatsonTtsUrl");
        ibmWatsonUsernameProp = serializedObject.FindProperty("ibmWatsonUsername");
        ibmWatsonPasswordProp = serializedObject.FindProperty("ibmWatsonPassword");
        twitterConsumerKeyProp = serializedObject.FindProperty("twitterConsumerKey");
        twitterConsumerSecretProp = serializedObject.FindProperty("twitterConsumerSecret");
        twitterAccessTokenProp = serializedObject.FindProperty("twitterAccessToken");
        twitterAccessTokenSecretProp = serializedObject.FindProperty("twitterAccessTokenSecret");
        unsplashAppIdProp = serializedObject.FindProperty("unsplashAppId");
        unsplashImageSizeProp = serializedObject.FindProperty("unsplashImageSize");
        slackWebhookUrlProp = serializedObject.FindProperty("slackWebhookUrl");
        slackCharlieIconProp = serializedObject.FindProperty("slackCharlieIcon");
        slackUserIconProp = serializedObject.FindProperty("slackUserIcon");
        slackChannelProp = serializedObject.FindProperty("slackChannel");
        giphyDebugProp = serializedObject.FindProperty("giphyDebug");
        giphyApiKeyProp = serializedObject.FindProperty("giphyApiKey");
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
            EditorGUILayout.HelpBox("These parameters can be found on the IBM Bluemix console.", MessageType.Info);
            EditorGUI.indentLevel--;
        }

        showTwitter = EditorGUILayout.Foldout(showTwitter, new GUIContent("Twitter"));
        if (showTwitter)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(twitterConsumerKeyProp, new GUIContent("Consumer Key"));
            EditorGUILayout.PropertyField(twitterConsumerSecretProp, new GUIContent("Consumer Secret"));
            EditorGUILayout.PropertyField(twitterAccessTokenProp, new GUIContent("Access Token"));
            EditorGUILayout.PropertyField(twitterAccessTokenSecretProp, new GUIContent("Access Token Secret"));
            EditorGUILayout.HelpBox("These parameters can be found on apps.twitter.com.", MessageType.Info);
            EditorGUI.indentLevel--;
        }

        showUnsplash = EditorGUILayout.Foldout(showUnsplash, new GUIContent("Unsplash"));
        if (showUnsplash)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(unsplashAppIdProp, new GUIContent("App ID"));
            EditorGUILayout.PropertyField(unsplashImageSizeProp, new GUIContent("Image Size"));
            EditorGUI.indentLevel--;
        }

        showSlack = EditorGUILayout.Foldout(showSlack, new GUIContent("Slack Log"));
        if (showSlack)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(slackWebhookUrlProp, new GUIContent("Slack Webhook Url"));
            EditorGUILayout.PropertyField(slackCharlieIconProp, new GUIContent("Charlie Icon"));
            EditorGUILayout.PropertyField(slackUserIconProp, new GUIContent("User Icon"));
            EditorGUILayout.PropertyField(slackChannelProp, new GUIContent("Channel"));
            EditorGUILayout.HelpBox("These parameters can be found on Slack: https://cmuetcsv.slack.com/services/B7UFMTLT0", MessageType.Info);
        }

        showGiphy = EditorGUILayout.Foldout(showGiphy, new GUIContent("Giphy"));
        if (showGiphy)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(giphyDebugProp, new GUIContent("Debug"));
            EditorGUILayout.PropertyField(giphyApiKeyProp, new GUIContent("API Key"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif
