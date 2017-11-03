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

    GameObject ImgObj = null;
    public void ChangeSelectState(bool isSelecting, bool isOnPhoto)
    {
        if (isSelecting)
        {
            var c = GameObject.Find("WorldCursor");
            ImgObj = Instantiate(transform.Find("Img").gameObject);
            ImgObj.transform.SetParent(c.transform, true);
            ImgObj.transform.localPosition = Vector3.zero;
            ImgObj.transform.localScale = Vector3.one * 3;
            ImgObj.transform.localEulerAngles = 90 * Vector3.right;
            ImgObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
        }
        else
        {
            if (isOnPhoto)
            {
                if (ImgObj != null)
                {
                    ImgObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                    ImgObj.transform.SetParent(GameObject.Find("Photoframe").transform, true);
                    CharlieManager.Instance.GetComponent<FsmEventGenerator>().BroadcastEvent("StickerPlaced");
                }
            }
            else
            {
                if (ImgObj != null)
                {
                    Destroy(ImgObj);
                }
            }
            ImgObj = null;
        }
    }

    public void OnAirTap()
    {
    }
}
