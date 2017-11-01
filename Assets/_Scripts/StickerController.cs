using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeHoverState(bool isHovering)
    {
        transform.Find("Rim").gameObject.SetActive(isHovering);
    }
}
