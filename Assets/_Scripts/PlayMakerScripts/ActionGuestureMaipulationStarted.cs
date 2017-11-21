using Charlie;
using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

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
        GestureManager.Instance.gestureRecognizer.ManipulationUpdated += OnManipulationUpdated;

        Finish();
    }

    public override void OnExit()
    {
        // switch off
        GestureManager.Instance.gestureRecognizer.ManipulationUpdated -= OnManipulationUpdated;
        Debug.Log("OnManipulationCompleted/cancelled");
        WorldCursor.Instance.cursorMaterial.color = Color.yellow;
    }


    private void OnManipulationUpdated(ManipulationUpdatedEventArgs obj)
    {
        Debug.Log("OnManipulationUpdated");
        float distance = Vector3.Distance(targetObj.transform.position, Camera.main.transform.position);
        targetObj.transform.position += (obj.cumulativeDelta - PrevManipulationPosition) * distance * 2f;

        PrevManipulationPosition = obj.cumulativeDelta;
       
        //Debug.LogError($"[OnManipulationGesture] {obj.cumulativeDelta}");
    }
}
