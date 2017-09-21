using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterBurn : MonoBehaviour {

	void Start () {
        StartCoroutine(DestroyAfterSeconds());
	}
	
	IEnumerator DestroyAfterSeconds() {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
