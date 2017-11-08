using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using HoloToolkit.Unity;
using Asyncoroutine;

namespace Charlie
{
    public class SpatialMapper : Singleton<SpatialMapper>
    {
        [Tooltip("The number of seconds to wait before re-determining if the space is good enough")]
        public float initialScanPollingInterval = 3.0f;

        [Tooltip("Mesh material to use when scanning")]
        public Material scanningMeshMaterial;

        [Tooltip("Mesh material to use when scanning is completed")]
        public Material doneMeshMaterial;

        public UnityEvent readyToScanEvent;
        public UnityEvent scanningEvent;
        public UnityEvent finishingEvent;
        public UnityEvent doneEvent;

        public enum AdjustmentMode
        {
            CAMERA_PARENT,
            TARGET_ROOT
        }

        [Tooltip("Method to adjust the scene position")]
        public AdjustmentMode adjustmentMode = AdjustmentMode.TARGET_ROOT;

        void Start()
        {
            SpatialUnderstanding.Instance.ScanStateChanged += HandleScanStateChanged;
            DontDestroyOnLoad(gameObject);
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
            SpatialUnderstanding.Instance.UnderstandingCustomMesh.MeshMaterial = scanningMeshMaterial;
            SpatialUnderstanding.Instance.RequestBeginScanning();
        }

        public void FinishMapping()
        {
            SpatialUnderstanding.Instance.RequestFinishScan();
            SpatialUnderstanding.Instance.UnderstandingCustomMesh.MeshMaterial = doneMeshMaterial;
        }

        public void ShowMesh()
        {
            Debug.Log($"[SpatialMapper]: Showing mesh");
            SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = true;
        }

        public async Task WaitForSurfaces()
        {
            SpatialUnderstandingDll.Imports.PlayspaceStats stats = null;
            while (true)
            {
                var statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                {
                    Debug.Log($"[SpatialMapper]: Failed to query playspace stats");
                }
                else
                {
                    stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
                    Debug.Log($"[SpatialMapper]: Playspace stats: NumPlatform: {stats.NumPlatform}, NumFloor: {stats.NumFloor}, NumCeiling: {stats.NumCeiling}");
                    Debug.Log($"[SpatialMapper]: Playspace stats: NumWall_XNeg: {stats.NumWall_XNeg}, NumWall_XPos: {stats.NumWall_XPos}, NumWall_ZNeg: {stats.NumWall_ZNeg}, NumWall_ZPos: {stats.NumWall_ZPos}");
                    Debug.Log($"[SpatialMapper]: Playspace stats: HorizSurfaceArea: {stats.HorizSurfaceArea}, TotalSurfaceArea: {stats.TotalSurfaceArea}, UpSurfaceArea: {stats.UpSurfaceArea}, DownSurfaceArea: {stats.DownSurfaceArea}, WallSurfaceArea: {stats.WallSurfaceArea}");
                    Debug.Log($"[SpatialMapper]: Playspace stats: CellCount_IsSeenQualtiy_None: {stats.CellCount_IsSeenQualtiy_None}, CellCount_IsSeenQualtiy_Seen: {stats.CellCount_IsSeenQualtiy_Seen}, CellCount_IsSeenQualtiy_Good: {stats.CellCount_IsSeenQualtiy_Good}");

                    if (stats.NumPlatform > 0 && stats.NumWall_XPos > 0 && stats.NumWall_ZPos > 0)
                    {
                        break;
                    }
                }

                Debug.Log($"[SpatialMapper]: Waiting for {initialScanPollingInterval} more seconds to retry");
                await new WaitForSeconds(initialScanPollingInterval);
            }
        }

        public void HandleDone()
        {
            SpatialUnderstandingDllTopology.TopologyResult[] result = {
                new SpatialUnderstandingDllTopology.TopologyResult()
            };
            
            // TODO: Not sure how to unpin this particular object, because there
            // is only UnpinAllObjects() which might cause trouble with other
            // scripts.
            var resultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(result);

            // Get the adjustment target.
            GameObject adjustmentTarget;
            switch (adjustmentMode)
            {
                case AdjustmentMode.CAMERA_PARENT:
                    Debug.Log($"[SpatialMapper]: Adjusting using CameraParent");
                    adjustmentTarget = GameObject.FindGameObjectWithTag("CameraParent");
                    break;
                case AdjustmentMode.TARGET_ROOT:
                default:
                    Debug.Log($"[SpatialMapper]: Adjusting using TargetRoot");
                    adjustmentTarget = GameObject.FindGameObjectWithTag("TargetRoot");
                    break;
            }

            // Try looking for tables. If we can't find a table, then the floor it is.
            var placesFound = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsSittable(0.5f, 1.5f, 0.5f, 1, resultPtr);
            if (placesFound > 0)
            {
                Debug.Log($"[SpatialMapper]: Found area on table: {result[0].position}");
                if (adjustmentMode == AdjustmentMode.CAMERA_PARENT)
                {
                    adjustmentTarget.transform.position = new Vector3(
                        -result[0].position.x,
                        -result[0].position.y,
                        -result[0].position.z
                    );
                }
                else
                {
                    adjustmentTarget.transform.position = new Vector3(
                        result[0].position.x,
                        result[0].position.y,
                        result[0].position.z
                    );
                }
            }
            else
            {
                SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(1, resultPtr);
                Debug.Log($"[SpatialMapper]: Found area on floor: {result[0].position}");

                // TODO: Separation of concerns, this part should ideally be in the TargetRoot/CameraParent.
                // For the floor, it doesn't matter where the x and z positions are.
                if (adjustmentMode == AdjustmentMode.CAMERA_PARENT)
                {
                    adjustmentTarget.transform.position = new Vector3(
                        adjustmentTarget.transform.position.x,
                        -result[0].position.y,
                        adjustmentTarget.transform.position.z
                    );
                }
                else
                {
                    adjustmentTarget.transform.position = new Vector3(
                        adjustmentTarget.transform.position.x,
                        result[0].position.y,
                        adjustmentTarget.transform.position.z
                    );
                }
            }
        }
    }
}
