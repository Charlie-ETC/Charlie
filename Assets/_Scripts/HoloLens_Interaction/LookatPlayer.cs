using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookatPlayer : MonoBehaviour {

    void Update()
    {
        transform.LookAt(Vector3.ProjectOnPlane(Camera.main.transform.position, Vector3.up));
       // transform.eulerAngles= new Vector3(0, transform.rotation.y, 0);
    }
}
