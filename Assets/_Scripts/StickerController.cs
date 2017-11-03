using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeHoverState(bool isHovering)
    {
        transform.Find("Rim").gameObject.SetActive(isHovering);
    }

    public void ChangeSelectState(bool isSelecting)
    {
        var c = GameObject.Find("WorldCursor");
        foreach (Transform child in c.transform)
        {
            Destroy(child.gameObject);
        }
        GameObject img = Instantiate(transform.Find("Img").gameObject);
        img.transform.SetParent(c.transform);
        img.transform.localPosition = Vector3.zero;
        img.transform.localScale = Vector3.one;
        img.transform.localEulerAngles = 90 * Vector3.right;
    }
}
