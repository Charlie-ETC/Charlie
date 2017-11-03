using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnAirTapLoadLevel : MonoBehaviour {

    public string sceneName;

	public void OnAirTap()
    {
        SceneManager.LoadScene(sceneName);
        
    }
}
