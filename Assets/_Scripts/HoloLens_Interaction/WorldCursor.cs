using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCursor : Charlie.Singleton<WorldCursor> {

    private MeshRenderer meshRenderer;
    private Vector3 cameraPos;
    private Vector3 gazeDirection;

    public GameObject GazeHoveringObject = null;

	void Start () {
        meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        cameraPos = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;

        RaycastHit hit;
        GameObject newGazeHoveringObject = null;

        if (Physics.Raycast(cameraPos, gazeDirection, out hit))
        {
            // move cursor to the hit point and hug the surface
            meshRenderer.enabled = true;
            transform.position = hit.point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            var sticker = hit.collider.gameObject;
            if (sticker.GetComponent<StickerController>() != null)
            {
                meshRenderer.enabled = false;
                newGazeHoveringObject = sticker;
            }
        }
        else
        {
            meshRenderer.enabled = false;
        }

        if (newGazeHoveringObject != GazeHoveringObject)
        {
            if (GazeHoveringObject != null)
                GazeHoveringObject?.GetComponent<StickerController>()?.ChangeHoverState(false);

            if (newGazeHoveringObject != null)
                newGazeHoveringObject?.GetComponent<StickerController>()?.ChangeHoverState(true);

            GazeHoveringObject = newGazeHoveringObject;
        }
	}
}
