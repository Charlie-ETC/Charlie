using UnityEngine;
using HoloToolkit.Unity;

namespace Charlie
{
    public class DebugManager : MonoBehaviour
    {
        private DictationMonitor dictationMonitor;
        
        void Awake()
        {
            dictationMonitor = FindObjectOfType<DictationMonitor>();
        }

        public void Activate()
        {
            dictationMonitor.transform.position = new Vector3(
                0.0f, dictationMonitor.transform.position.y,
                dictationMonitor.transform.position.z);
        }

        public void Deactivate()
        {
            dictationMonitor.transform.position = new Vector3(
                99999.0f, dictationMonitor.transform.position.x,
                dictationMonitor.transform.position.z);
        }
    }
}
