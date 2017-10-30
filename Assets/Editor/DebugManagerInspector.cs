#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Charlie;

namespace Assets.Editor
{
    [CustomEditor(typeof(DebugManager))]
    class DebugManagerInspector : UnityEditor.Editor
    {
        private static bool showSpatialUnderstanding;

        private DebugManager debugManager;

        SerializedProperty spatialUnderstandingMaterialProp;
        SerializedProperty spatialUnderstandingMaterialDebugProp;

        public void OnEnable()
        {
            debugManager = (DebugManager)target;
            spatialUnderstandingMaterialProp = serializedObject.FindProperty("spatialUnderstandingMaterial");
            spatialUnderstandingMaterialDebugProp = serializedObject.FindProperty("spatialUnderstandingMaterialDebug");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            showSpatialUnderstanding = EditorGUILayout.Foldout(showSpatialUnderstanding, new GUIContent("Spatial Understanding"));
            if (showSpatialUnderstanding)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(spatialUnderstandingMaterialProp, new GUIContent("Normal Material"));
                EditorGUILayout.PropertyField(spatialUnderstandingMaterialDebugProp, new GUIContent("Debug Material"));
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
