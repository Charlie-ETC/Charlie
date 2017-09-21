
using UnityEngine;

public class WorldCursor : MonoBehaviour {

    private MeshRenderer meshRenderer;
    private Vector3 cameraPos;
    private Vector3 gazeDirection;

	void Start () {
        meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
        meshRenderer.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        cameraPos = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cameraPos, gazeDirection, out hit))
        {
            // move cursor to the hit point and hug the surface
            meshRenderer.enabled = true;
            transform.position = hit.point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
        else
        {
            meshRenderer.enabled = false;
        }
	}
}
