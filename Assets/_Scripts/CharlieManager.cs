using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharlieManager : MonoBehaviour {

    public static CharlieManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(this);
        }
    }

}
