using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asyncoroutine;
using Charlie.Unsplash;

public class WindowAnimControl : MonoBehaviour {

    public string keyword;
    public bool trigger;

    private void Update()
    {
        if (trigger)
        {
            trigger = false;
            ChangeBG(keyword);
        }
    }

    // Use this for initialization
    void OnEnable ()
    {
        ChangeBG("Garden");
    }

    public async void ChangeBG(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            string FavoriteCity = await UserProfile.TryGetApiaiContext("favorite_city");
            content = FavoriteCity ?? "Water";
        }
        

        await new WaitForNextFrame();
        Texture x = await UnsplashService.Instance.GetRandomPhoto(content);
        transform.Find("Quad").GetComponent<MeshRenderer>().material.SetTexture("_MainTex", x);
    }
}
