using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;

[ActionCategory(ActionCategory.Audio)]
public class ActionWalkTo : FsmStateAction
{
    public Transform target;
    public FsmOwnerDefault objSelf;

    Animator anim = null;
    bool ended = false;

    public override async void OnEnter()
    {
        ended = false;
        Debug.Log($"ActionWalkTo, start");
        var go = Fsm.GetOwnerDefaultTarget(objSelf);
        anim = go.GetComponentInChildren<Animator>();
        //anim.SetBool("toWalk", true);
        int i = UnityEngine.Random.Range(1, 3);
        Debug.Log(i);
        anim.SetInteger("toWalk", i);

        while (ended == false)
        {
            if ((go.transform.position - target.position).magnitude < 0.3)
                break;

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(go.transform.position, target.position, NavMesh.AllAreas, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {
                Debug.Log("Path Invalid");
                break;
            }
            var waypoints = new List<Vector3>(path.corners);

            float dist = Time.deltaTime * 0.8f;
            while (waypoints.Count > 0 && (go.transform.position - waypoints[0]).magnitude < dist)
            {
                dist -= (go.transform.position - waypoints[0]).magnitude;
                go.transform.position = waypoints[0];
                waypoints.RemoveAt(0);
            }

            if (waypoints.Count == 0)
            {
                Debug.Log("waypoints finished");
                break;
            }

            go.transform.LookAt(waypoints[0]);
            go.transform.Translate(Vector3.forward * dist, Space.Self);

            await new WaitForNextFrame();
        }
        Finish();
        Debug.Log($"ActionWalkTo, finish");
    }

    public override void OnExit()
    {
        ended = true;
        //anim.SetBool("toWalk", false);
        anim.SetInteger("toWalk", 0);
    }

}


