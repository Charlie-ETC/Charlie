using UnityEngine;
using HoloToolkit.Unity;

namespace Charlie
{
    public class DebugManager : MonoBehaviour
    {
        public Material spatialUnderstandingMaterial;
        public Material spatialUnderstandingMaterialDebug;

        private SpatialUnderstandingCustomMesh spatialUnderstandingCustomMesh;
        private DictationMonitor dictationMonitor;
        
        void Awake()
        {
            spatialUnderstandingCustomMesh = FindObjectOfType<SpatialUnderstandingCustomMesh>();
            dictationMonitor = FindObjectOfType<DictationMonitor>();
        }

        public void Activate()
        {
            spatialUnderstandingCustomMesh.MeshMaterial = spatialUnderstandingMaterialDebug;
            dictationMonitor.transform.position = new Vector3(
                0.0f, dictationMonitor.transform.position.y,
                dictationMonitor.transform.position.z);
        }

        public void Deactivate()
        {
            spatialUnderstandingCustomMesh.MeshMaterial = spatialUnderstandingMaterial;
            dictationMonitor.transform.position = new Vector3(
                99999.0f, dictationMonitor.transform.position.x,
                dictationMonitor.transform.position.z);
        }
    }
}
