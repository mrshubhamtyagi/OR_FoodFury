using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    public class CaptureScreenshot : MonoBehaviour
    {
        public string screenshotName = "Screenshot.png";
        
        void Start()
        {
        
        }

        [ContextMenu("TakeScreenshot")]
        public void TakeScreenshot()
        {
            ScreenCapture.CaptureScreenshot($"/Users/Office/Desktop/{screenshotName}", 1);
            //TakeTransparentScreenshot(GetComponent<Camera>(), Screen.width, Screen.height, screenshotName);
            // TakeTransparentScreenshot(GetComponent<Camera>(), 2048, 2048, $"/Users/Office/Desktop/{screenshotName}");
        }
    }
}
