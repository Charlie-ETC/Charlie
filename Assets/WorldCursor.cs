using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCursor : MonoBehaviour {

    private MeshRenderer meshRenderer;
    private Vector3 cameraPos;
    private Vector3 gazeDirction;

	void Start () {
        meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        cameraPos = Camera.main.transform.position;
        gazeDirction = Camera.main.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cameraPos, gazeDirction, out hit))
        {
            // move cursor to the hit point and hug the surface
            meshRenderer.enabled = true;
            transform.position = hit.point;
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
            Debug.Log("yes");
        }
        else
        {
            meshRenderer.enabled = false;
        }
	}
}
