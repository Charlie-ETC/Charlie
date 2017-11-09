using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using HoloToolkit.Unity;
using Asyncoroutine;

namespace Charlie
{
    public class SpatialMapper : Singleton<SpatialMapper>
    {
        [Tooltip("The number of seconds to wait before re-determining if the space is good enough")]
        public float initialScanPollingInterval = 3.0f;

        [Tooltip("The GameObject to use as the initial spatial anchor")]
        public GameObject initialSpatialAnchor;

        [Tooltip("Mesh material to use when scanning is completed")]
        public Material doneMeshMaterial;
        
        public UnityEvent readyToScanEvent;
        public UnityEvent scanningEvent;
        public UnityEvent finishingEvent;
        public UnityEvent doneEvent;

        /// <summary>
        /// Whether to show the processed spatial understanding mesh or not.
        /// </summary>
        public bool ShowMesh
        {
            get
            {
                return SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh;
            }
            set
            {
                SpatialUnderstanding.Instance.UnderstandingCustomMesh.DrawProcessedMesh = value;
            }
        }

        /// <summary>
        /// True if mapping can be started.
        /// </summary>
        public bool CanBeginMapping
        {
            get
            {
                return SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.ReadyToScan;
            }
        }

        /// <summary>
        /// True if mapping can be finished.
        /// </summary>
        public bool CanFinishMapping
        {
            get
            {
                return SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Scanning;
            }
        }

        public enum AdjustmentMode
        {
            CAMERA_PARENT,
            TARGET_ROOT
        }

        void Start()
        {
            SpatialUnderstanding.Instance.ScanStateChanged += HandleScanStateChanged;
            SceneManager.activeSceneChanged += HandleActiveSceneChanged;
            DontDestroyOnLoad(gameObject);
        }

        private void HandleActiveSceneChanged(Scene previous, Scene current)
        {
            // Look for a GameObject tagged as TargetRoot, move that.
            GameObject targetRoot = GameObject.FindGameObjectWithTag("TargetRoot");
            if (targetRoot != null)
            {
                Debug.Log($"[SpatialMapper] Found TargetRoot, moving it to the platform");
                WorldAnchorManager.Instance.AttachAnchor(targetRoot, "TargetRoot");
            }
        }

        /// <summary>
        /// Handles changes in spatial understanding scan state. This is invoked
        /// by the SpatialUnderstand library and should not be called directly.
        /// </summary>
        private void HandleScanStateChanged()
        {
            SpatialUnderstanding.ScanStates state = SpatialUnderstanding.Instance.ScanState;
            HandleScanStateChanged(state);
        }

        void HandleScanStateChanged(SpatialUnderstanding.ScanStates state)
        {
            switch (state)
            {
                case SpatialUnderstanding.ScanStates.ReadyToScan:
                    Debug.Log("[SpatialMapper] Ready to perform room scanning");
                    readyToScanEvent.Invoke();
                    break;
                case SpatialUnderstanding.ScanStates.Scanning:
                    Debug.Log("[SpatialMapper] Scanning room");
                    scanningEvent.Invoke();
                    break;
                case SpatialUnderstanding.ScanStates.Finishing:
                    Debug.Log("[SpatialMapper] Finishing room scanning");
                    finishingEvent.Invoke();
                    break;
                case SpatialUnderstanding.ScanStates.Done:
                    Debug.Log("[SpatialMapper] Room scanning complete");
                    SpatialUnderstanding.Instance.UnderstandingCustomMesh.MeshMaterial = doneMeshMaterial;
                    MoveTargetRoot(initialSpatialAnchor);
                    WorldAnchorManager.Instance.AttachAnchor(initialSpatialAnchor, "TargetRoot");
                    doneEvent.Invoke();
                    break;
                default:
                    Debug.Log($"[SpatialMapper] Unknown mapping state {state}");
                    break;
            }
        }

        /// <summary>
        /// Begins the spatial mapping process.
        /// </summary>
        public void BeginMapping()
        {
            SpatialUnderstanding.Instance.RequestBeginScanning();
        }


        /// <summary>
        /// Finishes the spatial mapping process.
        /// </summary>
        public void FinishMapping()
        {
            if (XRDevice.isPresent)
            {
                SpatialUnderstanding.Instance.RequestFinishScan();
            }
            else
            {
                HandleScanStateChanged(SpatialUnderstanding.ScanStates.Finishing);
                HandleScanStateChanged(SpatialUnderstanding.ScanStates.Done);
            }
        }

        /// <summary>
        /// Wait for playspace stats to be available.
        /// </summary>
        /// <returns></returns>
        public async Task WaitForPlayspaceStats()
        {
            if (!XRDevice.isPresent)
            {
                Debug.Log($"[SpatialMapper] XR Device is not present, skipping wait");
                return;
            }

            SpatialUnderstandingDll.Imports.PlayspaceStats stats = null;
            while (true)
            {
                var statsPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStatsPtr();
                if (SpatialUnderstandingDll.Imports.QueryPlayspaceStats(statsPtr) == 0)
                {
                    Debug.Log($"[SpatialMapper] Failed to query playspace stats");
                }
                else
                {
                    stats = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceStats();
                    Debug.Log($"[SpatialMapper] Playspace stats: NumPlatform: {stats.NumPlatform}, NumFloor: {stats.NumFloor}, NumCeiling: {stats.NumCeiling}");
                    Debug.Log($"[SpatialMapper] Playspace stats: NumWall_XNeg: {stats.NumWall_XNeg}, NumWall_XPos: {stats.NumWall_XPos}, NumWall_ZNeg: {stats.NumWall_ZNeg}, NumWall_ZPos: {stats.NumWall_ZPos}");
                    Debug.Log($"[SpatialMapper] Playspace stats: HorizSurfaceArea: {stats.HorizSurfaceArea}, TotalSurfaceArea: {stats.TotalSurfaceArea}, UpSurfaceArea: {stats.UpSurfaceArea}, DownSurfaceArea: {stats.DownSurfaceArea}, WallSurfaceArea: {stats.WallSurfaceArea}");
                    Debug.Log($"[SpatialMapper] Playspace stats: CellCount_IsSeenQualtiy_None: {stats.CellCount_IsSeenQualtiy_None}, CellCount_IsSeenQualtiy_Seen: {stats.CellCount_IsSeenQualtiy_Seen}, CellCount_IsSeenQualtiy_Good: {stats.CellCount_IsSeenQualtiy_Good}");

                    if (stats.NumPlatform > 0 && stats.NumWall_XPos > 0 && stats.NumWall_ZPos > 0)
                    {
                        break;
                    }
                }

                Debug.Log($"[SpatialMapper] Waiting for {initialScanPollingInterval} more seconds to retry");
                await new WaitForSeconds(initialScanPollingInterval);
            }
        }

        private void MoveTargetRoot(GameObject targetRoot)
        {
            SpatialUnderstandingDllTopology.TopologyResult[] result = {
                new SpatialUnderstandingDllTopology.TopologyResult()
            };
            
            // TODO: Not sure how to unpin this particular object, because there
            // is only UnpinAllObjects() which might cause trouble with other
            // scripts.
            var resultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(result);

            // Try looking for tables. If we can't find a table, then the floor it is.
            var placesFound = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsSittable(0.5f, 1.5f, 0.5f, 1, resultPtr);
            if (placesFound > 0)
            {
                Debug.Log($"[SpatialMapper] Found area on table: {result[0].position}");
                targetRoot.transform.position = new Vector3(
                    result[0].position.x,
                    result[0].position.y,
                    result[0].position.z
                );
            }
            else
            {
                SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(1, resultPtr);
                Debug.Log($"[SpatialMapper] Found area on floor: {result[0].position}");

                // TODO: Separation of concerns, this part should ideally be in the TargetRoot/CameraParent.
                // For the floor, it doesn't matter where the x and z positions are.
                targetRoot.transform.position = new Vector3(
                    targetRoot.transform.position.x,
                    result[0].position.y,
                    targetRoot.transform.position.z
                );
            }
        }
    }
}
