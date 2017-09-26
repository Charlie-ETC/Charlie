using UnityEngine;
using System.Collections;
using System.Linq;


public class PhotoCaptureExample : MonoBehaviour
{
    UnityEngine.XR.WSA.WebCam.PhotoCapture photoCaptureObject = null;
    public Texture2D targetTexture = null;
    GameObject quad = null;


    int cnt = 0;
    private void Update()
    {
        cnt++;
        if (cnt > 400)
        {
            cnt = 0;
            Capture();
        }
    }

    // Use this for initialization
    void Capture()
    {
        Debug.Log("Capture");
        Resolution cameraResolution = UnityEngine.XR.WSA.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        UnityEngine.XR.WSA.WebCam.PhotoCapture.CreateAsync(false, delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture captureObject) {
            photoCaptureObject = captureObject;
            UnityEngine.XR.WSA.WebCam.CameraParameters cameraParameters = new UnityEngine.XR.WSA.WebCam.CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = UnityEngine.XR.WSA.WebCam.CapturePixelFormat.BGRA32;

            // Activate the camera
            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result) {
                // Take a picture
                Debug.Log("photoCaptureObject.StartPhotoModeAsync");
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
            });
        });
    }

    void OnCapturedPhotoToMemory(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result, UnityEngine.XR.WSA.WebCam.PhotoCaptureFrame photoCaptureFrame)
    {
        Debug.Log("OnCapturedPhotoToMemory");
        // Copy the raw image data into the target texture
        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

        // Create a GameObject to which the texture can be applied
        if (quad != null)
            GameObject.DestroyImmediate(quad);

        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
        quadRenderer.material = new Material(Shader.Find("Unlit/Texture"));

        quad.transform.parent = this.transform;
        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);

        quadRenderer.material.SetTexture("_MainTex", targetTexture);

        // Deactivate the camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(UnityEngine.XR.WSA.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        Debug.Log("OnStoppedPhotoMode");
        // Shutdown the photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
}