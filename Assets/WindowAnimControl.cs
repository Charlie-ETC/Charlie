using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asyncoroutine;

public class WindowAnimControl : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
        ChangeBG("Water");
    }

    public async void ChangeBG(string content)
    {
        await new WaitForNextFrame();
        Texture x = await GetComponent<Unsplash.UnsplashService>().GetRandomPhoto(content);
        transform.Find("Quad").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", x);
    }
}
