using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCommands : MonoBehaviour {
    public GameObject particlePrefab;

    private IEnumerator dragCoroutine;

    // selected by Air Tap or "Tap it"
    public void OnSelect() {
        Debug.Log("Tap it");
        if (particlePrefab != null) {
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }
    }

    // selected by "Drag it"
    public void OnDrag()
    {
        Debug.Log("Drag it");
        if (dragCoroutine != null)
        {
            StopCoroutine(dragCoroutine);
        }

        dragCoroutine = DragToMove();
        StartCoroutine(dragCoroutine);
    }

    // selected by "Place it here"
    public void OnPlaced()
    {
        Debug.Log("Place it here");
        if (dragCoroutine != null)
        {
            StopCoroutine(dragCoroutine);
        } 
    }

    private IEnumerator DragToMove()
    {
        Debug.Log("DragToMove");
        // always keep the same distance as you say the command
        Vector3 distanceVec = Camera.main.transform.position - transform.position;

        while (gameObject == GestureManager.Instance.FocusedObject) {

            transform.position = Camera.main.transform.position - distanceVec;
            transform.LookAt(Camera.main.transform);
            yield return new WaitForSeconds(Time.deltaTime);

        }   
    }
}
