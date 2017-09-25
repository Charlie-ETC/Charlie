using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterBurn : MonoBehaviour {
    public float liveTime;

	void Start () {
        StartCoroutine(DestroyAfterSeconds());
	}
	
	IEnumerator DestroyAfterSeconds() {
        yield return new WaitForSeconds(liveTime);
        Destroy(gameObject);
    }
}
