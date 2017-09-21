using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCommands : MonoBehaviour {
    public GameObject particlePrefab;

    public void OnSelect() {
        Debug.Log("once");

        // do something to the cube when selected by air tap
        if (particlePrefab != null) {
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }
        
    }
}
