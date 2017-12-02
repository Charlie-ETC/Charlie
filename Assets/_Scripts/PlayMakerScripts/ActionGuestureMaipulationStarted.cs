using Charlie;
using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

public class ActionGuestureMaipulationStarted : FsmStateAction
{

    //[UIHint(UIHint.TextArea)]
    //public FsmString speech;

    public FsmOwnerDefault objSelf;
    private GameObject targetObj;
    private Vector3 PrevManipulationPosition;


    public override void OnEnter()
    {

        Debug.Log("OnManipulationStarted_IsDraggable");
        WorldCursor.Instance.cursorMaterial.color = Color.blue;
        targetObj = WorldCursor.Instance.newGazeHoveringObject;
        PrevManipulationPosition = Vector3.zero;

        // swtich on
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
        GestureManager.Instance.gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;
#endif

        Finish();
    }

    public override void OnExit()
    {
        // switch off
#if UNITY_2017_2_OR_NEWER && UNITY_WSA
        GestureManager.Instance.gestureRecognizer.ManipulationUpdated -= OnManipulationUpdated;
#endif
        Debug.Log("OnManipulationCompleted/cancelled");
        WorldCursor.Instance.cursorMaterial.color = Color.yellow;
    }


#if UNITY_2017_2_OR_NEWER && UNITY_WSA
    private void OnManipulationUpdated(ManipulationUpdatedEventArgs obj)
    {
        Debug.Log("OnManipulationUpdated");
        float distance = Vector3.Distance(targetObj.transform.position, Camera.main.transform.position);
        targetObj.transform.position += (obj.cumulativeDelta - PrevManipulationPosition) * distance * 2f;

        PrevManipulationPosition = obj.cumulativeDelta;
       
        //Debug.LogError($"[OnManipulationGesture] {obj.cumulativeDelta}");
    }
#endif
}
