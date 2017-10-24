using UnityEngine;
using UnityEngine.Events;
using HoloToolkit.Unity;

namespace Charlie
{
    public class SpatialMapper : MonoBehaviour
    {
        public UnityEvent readyToScanEvent;
        public UnityEvent scanningEvent;
        public UnityEvent finishingEvent;
        public UnityEvent doneEvent;

        void Start()
        {
            SpatialUnderstanding.Instance.ScanStateChanged += HandleScanStateChanged;
        }

        void HandleScanStateChanged()
        {
            SpatialUnderstanding.ScanStates state = SpatialUnderstanding.Instance.ScanState;
            switch (state)
            {
                case SpatialUnderstanding.ScanStates.ReadyToScan:
                    Debug.Log("[SpatialMapper] Ready to perform room scanning");
                    readyToScanEvent.Invoke();
                    break;
                case SpatialUnderstanding.ScanStates.Scanning:
                    Debug.Log("[SpatialMapper]: Scanning room");
                    scanningEvent.Invoke();
                    break;
                case SpatialUnderstanding.ScanStates.Finishing:
                    Debug.Log("[SpatialMapper]: Finishing room scanning");
                    finishingEvent.Invoke();
                    break;
                case SpatialUnderstanding.ScanStates.Done:
                    Debug.Log("[SpatialMapper]: Room scanning complete");
                    doneEvent.Invoke();
                    break;
                default:
                    Debug.Log($"[SpatialMapper]: Unknown mapping state {state}");
                    break;
            }
        }

        public bool CanBeginMapping()
        {
            return SpatialUnderstanding.Instance.ScanState ==
                SpatialUnderstanding.ScanStates.ReadyToScan;
        }

        public bool CanFinishMapping()
        {
            return SpatialUnderstanding.Instance.ScanState ==
                SpatialUnderstanding.ScanStates.Scanning;
        }

        public void BeginMapping()
        {
            SpatialUnderstanding.Instance.RequestBeginScanning();
        }

        public void FinishMapping()
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
        }

        public void HandleDone()
        {
            SpatialUnderstandingDllTopology.TopologyResult[] result = {
                new SpatialUnderstandingDllTopology.TopologyResult()
            };

            var resultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(result);
            SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(1, resultPtr);
            Debug.Log($"[SpatialMapper]: Found area on floor: {result[0].position}");

            // TODO: Not sure how to unpin this particular object, because there
            // is only UnpinAllObjects() which might cause trouble with other
            // scripts.

            // TODO: Separation of concerns, this part should ideally be in the TargetRoot.
            Debug.Log("[SpatialMapper]: Setting TargetRoot to correct y position");
            GameObject cameraParent = GameObject.FindGameObjectWithTag("CameraParent");
            cameraParent.transform.position = new Vector3(
                cameraParent.transform.position.x,
                -result[0].position.y,
                cameraParent.transform.position.z
            );
        }
    }
}
