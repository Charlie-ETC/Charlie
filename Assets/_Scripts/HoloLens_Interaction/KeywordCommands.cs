
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Charlie
{

    public class KeywordCommands : MonoBehaviour
    {


        /// <summary>
        /// keyword command functions to manipulate focused game object
        /// </summary>

        public static void OnTapIt()
        {
            Debug.Log("OnTapIt");
            //if (GestureManager.Instance.FocusedObject != null)
            //{
            //    CubeCommands cc = GestureManager.Instance.FocusedObject.GetComponentInParent<CubeCommands>();
            //    if (cc != null)
            //    {
            //        cc.OnSelect();
            //    }
            //}
        }

        //public static void OnDragIt()
        //{
        //    Debug.Log("OnDragIt");
        //    if (GestureManager.Instance.FocusedObject != null)
        //    {
        //        CubeCommands cc = GestureManager.Instance.FocusedObject.GetComponentInParent<CubeCommands>();
        //        if (cc != null)
        //        {
        //            cc.OnDrag();
        //        }
        //    }
        //}

        //public static void OnPlaceItHere()
        //{
        //    Debug.Log("OnPlaceItHere");
        //    if (GestureManager.Instance.FocusedObject != null)
        //    {
        //        CubeCommands cc = GestureManager.Instance.FocusedObject.GetComponentInParent<CubeCommands>();
        //        if (cc != null)
        //        {
        //            cc.OnPlaced();
        //        }
        //    }
        //}


        /// <summary>
        /// keyword command to change game system
        /// </summary>

        public static void OnReset()
        {
            Debug.Log("OnReset");
            SceneManager.LoadScene("Main");
        }
    }
}
