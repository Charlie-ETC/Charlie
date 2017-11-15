#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Charlie
{
    [CustomEditor(typeof(SpatialMapper))]
    public class SpatialMapperInspector : Editor
    {
        private SpatialMapper mapper;

        public void OnEnable()
        {
            mapper = (SpatialMapper)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                GUILayout.BeginHorizontal();
                GUI.enabled = mapper.CanBeginMapping;
                bool beginMappingClicked = GUILayout.Button(new GUIContent("Begin Mapping"));
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUI.enabled = mapper.CanFinishMapping;
                bool finishMappingClicked = GUILayout.Button(new GUIContent("Finish Mapping"));
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                if (beginMappingClicked)
                {
                    HandleBeginMappingClicked();
                }

                if (finishMappingClicked)
                {
                    HandleFinishMappingClicked();
                }
            }
        }

        private void HandleBeginMappingClicked()
        {
            mapper.BeginMapping();
        }

        private void HandleFinishMappingClicked()
        {
            mapper.FinishMapping();
        }
    }
}

#endif
