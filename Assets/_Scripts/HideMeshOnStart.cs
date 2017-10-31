using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMeshOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().enabled = false ;
	}
}
