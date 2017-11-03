using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace Charlie
{

    public class GestureManager : MonoBehaviour
    {

        public static GestureManager Instance { get; private set; }

        // gameobject that is being gazed at
        public GameObject FocusedObject { get; private set; }
        public GameObject OldFocusedObject { get; private set; }

#if UNITY_WSA
        private GestureRecognizer gestureRecognizer;
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
#if UNITY_WSA
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.TappedEvent += OnAirTap;
            gestureRecognizer.ManipulationUpdatedEvent += OnManipulationGesture;
            gestureRecognizer.StartCapturingGestures();
#endif
        }

#if UNITY_WSA
        private void OnManipulationGesture(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            Transform TargetRoot = GameObject.FindGameObjectWithTag("TargetRoot").transform;
            Transform cam = Camera.main.transform;

            float dist = (TargetRoot.position - cam.position).magnitude;
            dist += cumulativeDelta.y;
            TargetRoot.Rotate(new Vector3(0, cumulativeDelta.x, 0));

            TargetRoot.position = headRay.GetPoint(dist);
        }
#endif

        // Update is called once per frame
        void Update()
        {
            OldFocusedObject = FocusedObject;

#if UNITY_EDITOR

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.collider.name);
                    CubeCommands cc = hit.collider.gameObject.GetComponentInParent<CubeCommands>();
                    if (cc != null) { cc.OnSelect(); }
                }
            }

#if UNITY_WSA
            if (Input.GetKeyDown("space"))
            {
                OnAirTap(InteractionSourceKind.Controller, 1, new Ray());
            }
#endif

#else
        cameraPos = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cameraPos, gazeDirection, out hit))
        {
            FocusedObject = hit.collider.gameObject;
        }
        else {
            FocusedObject = null;
        }

        // if focusedObject changes?
#if UNITY_WSA
        if (FocusedObject != OldFocusedObject) {
            gestureRecognizer.CancelGestures();
            gestureRecognizer.StartCapturingGestures();
        }
#endif

#endif

        }

#if UNITY_WSA
        private void OnAirTap(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            // air tap focused object to call its OnSelect()
            //if (FocusedObject != null) {
            //    CubeCommands cc = FocusedObject.GetComponentInParent<CubeCommands>();
            //    if (cc != null) {
            //        cc.OnSelect();
            //    }
            //}

            SpatialMapper mapper = FindObjectOfType<SpatialMapper>();
            mapper.FinishMapping();
            //SceneManager.LoadScene("Main");
        }
#endif

    }
}
