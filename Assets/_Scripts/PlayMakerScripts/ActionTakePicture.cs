using HutongGames.PlayMaker;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Asyncoroutine;

public class ActionTakePicture : FsmStateAction
{

    public override void OnEnter()
    {
#if UNITY_WSA
        Debug.Log("[DictationMonitor] Taking a picture!");
        Resolution resolution = UnityEngine.XR.WSA.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        Texture2D texture = new Texture2D(resolution.width, resolution.height);
        //spatialMappingManager.StopObserver();

        UnityEngine.XR.WSA.WebCam.PhotoCapture.CreateAsync(true, delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture capture) {
            UnityEngine.XR.WSA.WebCam.CameraParameters cameraParams = new UnityEngine.XR.WSA.WebCam.CameraParameters() {
                hologramOpacity = 0.8f,
                cameraResolutionWidth = resolution.width,
                cameraResolutionHeight = resolution.height,
                pixelFormat = UnityEngine.XR.WSA.WebCam.CapturePixelFormat.BGRA32
            };

            capture.StartPhotoModeAsync(cameraParams, delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result) {
                capture.TakePhotoAsync(async delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result2, UnityEngine.XR.WSA.WebCam.PhotoCaptureFrame frame) {
                    await new WaitForSeconds(2f);
                    frame.UploadImageDataToTexture(texture);

                    // show photo here
                    GameObject.Find("PhotoTaken").GetComponent<RawImage>().texture = texture;
                    capture.StopPhotoModeAsync(delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result3) {
                        Debug.Log("[DictationMonitor] Stopping photo mode");
                    });

                    Finish();
                });
            });
        });
#endif
    }
}



