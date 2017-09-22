using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GestureManager : MonoBehaviour {

    public static GestureManager Instance { get; private set; }

    // gameobject that is being gazed at
    public GameObject FocusedObject { get; private set; }
    public GameObject OldFocusedObject { get; private set; }

    private GestureRecognizer gestureRecognizer;

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
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.TappedEvent += OnAirTap;
        gestureRecognizer.StartCapturingGestures();
    }

    // Update is called once per frame
    void Update () {

        OldFocusedObject = FocusedObject;

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
        if (FocusedObject != OldFocusedObject) {
            gestureRecognizer.CancelGestures();
            gestureRecognizer.StartCapturingGestures();
        }

    }

    private void OnAirTap(InteractionSourceKind source, int tapCount, Ray headRay) {
        // air tap focused object to call its OnSelect()
        if (FocusedObject != null) {
            CubeCommands cc = FocusedObject.GetComponentInParent<CubeCommands>();
            if (cc != null) {
                cc.OnSelect();
            }
        }
    }
}
