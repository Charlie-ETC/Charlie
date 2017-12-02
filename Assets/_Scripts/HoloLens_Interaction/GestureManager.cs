using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace Charlie
{

    public class GestureManager : MonoBehaviour
    {

        public static GestureManager Instance { get; private set; }

        // gameobject that is being gazed at
        //public GameObject FocusedObject { get; private set; }
        //public GameObject OldFocusedObject { get; private set; }

#if UNITY_2017_2_OR_NEWER && UNITY_WSA
        public GestureRecognizer gestureRecognizer;
        private bool IsDragging { get; set; }
        private Vector3 PrevManipulationPosition { get; set; }
#endif

        private Vector3 cameraPos;
        private Vector3 gazeDirection;

        // make sure this class is singleton
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                // destroy this instance when guestureManager already exists
                Destroy(this);
            }
        }

        // initialize gesture events
        private void Start()
        {
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
            gestureRecognizer = new GestureRecognizer();
            // add recognizableGuestures
            gestureRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate | GestureSettings.Tap);

            // register events
            gestureRecognizer.TappedEvent += OnAirTap;

            // The system interprets the gesture based on which one you request when you create your GestureRecognizer.
            // Your application cannot request both Navigation and Manipulation simultaneously.
            // This is why Holograms 211 uses a voice command to switch between rotating and moving the astronaut.
            //gestureRecognizer.NavigationUpdated += OnNavigationUpdated;

            gestureRecognizer.ManipulationStarted += OnManipulationStarted;
            //gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
            gestureRecognizer.ManipulationCompleted += OnManipulationCompleted;
            gestureRecognizer.ManipulationCanceled += OnManipulationCanceled;
            //Debug.LogError($"[OnManipulationGesture] start");
            gestureRecognizer.StartCapturingGestures();
#endif
        }


        // Update is called once per frame
        void Update()
        {
            //Debug.Log("capturing: " + gestureRecognizer.IsCapturingGestures());
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
            // restart capturing gestures if focusedObject changes
            if (WorldCursor.Instance.prevGazeHoveringObject != WorldCursor.Instance.newGazeHoveringObject) {
                Debug.Log("Restart gesture capturing");
                gestureRecognizer.CancelGestures();
                gestureRecognizer.StartCapturingGestures();
            }       
#endif

            //#if UNITY_EDITOR

            //if (Input.GetMouseButtonDown(0))
            //{
            //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //    RaycastHit hit;

            //    if (Physics.Raycast(ray, out hit))
            //    {
            //        Debug.Log(hit.collider.name);
            //        CubeCommands cc = hit.collider.gameObject.GetComponentInParent<CubeCommands>();
            //        if (cc != null) { cc.OnSelect(); }
            //    }
            //}

#if UNITY_2017_2_OR_NEWER && UNITY_WSA
            if (Input.GetKeyDown("space"))
            {
                OnAirTap(InteractionSourceKind.Controller, 1, new Ray());
            }
#endif
        }

        //#if UNITY_WSA
        //        private void OnNavigationUpdated(NavigationUpdatedEventArgs obj)
        //        {
        //            //Debug.LogError($"[OnManipulationGesture] {obj.normalizedOffset}");

        //            //Transform TargetRoot = GameObject.FindGameObjectWithTag("TargetRoot").transform;
        //            //Transform cam = Camera.main.transform;

        //            //float dist = obj.normalizedOffset.y * 2f + 3f;

        //            //TargetRoot.eulerAngles = (new Vector3(0, obj.normalizedOffset.x * 60, 0));

        //            //TargetRoot.position = cam.position + cam.forward * dist;
        //        }
        //#endif

#if UNITY_2017_2_OR_NEWER && UNITY_WSA
        private void OnManipulationStarted(ManipulationStartedEventArgs obj)
        {
            //Debug.Log("OnManipulationStarted");
            //if (WorldCursor.Instance.newGazeHoveringObject != null && WorldCursor.Instance.newGazeHoveringObject.GetComponent<Draggable>()) {
            //    Debug.Log("OnManipulationStarted_IsDraggable");
            //    IsDragging = true;
            //    WorldCursor.Instance.cursorMaterial.color = Color.blue;
            //    PrevManipulationPosition = Vector3.zero;
            //}

            gameObject.GetComponent<PlayMakerFSM>().SendEvent("ManipulationStarted");

        }

        // drag movable stuff to move
        // OnManipulationUpdated is in ActionGuestureMaipulationStarted script

        private void OnManipulationCompleted(ManipulationCompletedEventArgs obj)
        {
            Debug.Log("OnManipulationCompleted");
            gameObject.GetComponent<PlayMakerFSM>().SendEvent("ManipulationCompleted");
        }

        private void OnManipulationCanceled(ManipulationCanceledEventArgs obj)
        {
            Debug.Log("OnManipulationCanceled");
            gameObject.GetComponent<PlayMakerFSM>().SendEvent("ManipulationCancelled");
        }
#endif


        public static StickerController SelectedObject = null;
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
        private void OnAirTap(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (SelectedObject != null)
            {
                if (Physics.Raycast(ray, out hit, 40, LayerMask.GetMask("Photo")))
                {
                    //hit.collider.GetComponent
                    SelectedObject.ChangeSelectState(false, true);
                    Debug.LogError("hah");
                }
                else
                {
                    SelectedObject.ChangeSelectState(false, false);
                }
                SelectedObject = null;
            }
            else if (Physics.Raycast(ray, out hit, 40, LayerMask.GetMask("Sticker")))
            {
                var sticker = hit.collider.GetComponent<StickerController>();
                if (sticker != null)
                {
                    SelectedObject = sticker;
                    SelectedObject.ChangeSelectState(true, false);
                }
            }
            else if (Physics.Raycast(ray, out hit, 40, LayerMask.GetMask("Tappable")))
            {
                hit.collider.gameObject.SendMessage("OnAirTap");
                Debug.LogError("Tappable");
            }
        }
#endif
        public void VoiceSelectObject(GameObject go)
        {
            var sticker = go.GetComponent<StickerController>();
            if (sticker != null)
            {
                SelectedObject = sticker;
                SelectedObject.ChangeSelectState(true, false);
            }
        }

        public void VoicePutObject()
        {

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (SelectedObject != null)
            {
                if (Physics.Raycast(ray, out hit, 40, LayerMask.GetMask("Photo")))
                {
                    //hit.collider.GetComponent
                    SelectedObject.ChangeSelectState(false, true);
                    Debug.LogError("hah");
                }
                SelectedObject = null;
            }
        }

        // unregister events
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
        private void OnDestroy()
        {
            gestureRecognizer.TappedEvent -= OnAirTap;

            gestureRecognizer.ManipulationStarted -= OnManipulationStarted;
            //gestureRecognizer.ManipulationUpdated -= OnManipulationUpdated;
            gestureRecognizer.ManipulationCompleted -= OnManipulationCompleted;
            gestureRecognizer.ManipulationCanceled -= OnManipulationCanceled;
        }
#endif
    }
}
