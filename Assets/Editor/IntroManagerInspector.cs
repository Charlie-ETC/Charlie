using UnityEngine;
using UnityEditor;

namespace Charlie
{
    [CustomEditor(typeof(IntroManager))]
    public class IntroManagerInspector : Editor
    {
        private IntroManager introManager;

        public void OnEnable()
        {
            introManager = (IntroManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (Application.isPlaying)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 16.0f);
                bool nextSceneClicked = GUILayout.Button(new GUIContent("Next Scene"));
                GUILayout.EndHorizontal();

                if (nextSceneClicked)
                {
                    HandleNextSceneClicked();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void HandleNextSceneClicked()
        {
            introManager.OnMappingDone();
        }
    }
}
