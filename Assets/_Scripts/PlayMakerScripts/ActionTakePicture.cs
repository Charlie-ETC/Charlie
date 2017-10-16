using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Asyncoroutine;
using UnityEngine.AI;
using System.Linq;

public class ActionTakePicture : FsmStateAction
{

    public override void OnEnter()
    {
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
                    frame.UploadImageDataToTexture(texture);
                    byte[] jpegData = ImageConversion.EncodeToJPG(texture, 80);

                    Debug.Log("[DictationMonitor] Uploading picture to Twitter");
                    Media media = await DictationMonitor.Instance.twitterService.UploadMedia(jpegData);
                    DictationMonitor.Instance.twitterService.TweetWithMedia("hello!", new string[1] { media.mediaIdString });
                    //spatialMappingManager.StartObserver();

                    capture.StopPhotoModeAsync(delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result3) {
                        Debug.Log("[DictationMonitor] Stopping photo mode");
                    });

                    Finish();
                });
            });
        });
    }
}



