using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace FoodFury
{
    public class JavaScriptCallbacks : MonoBehaviour
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        public static extern void CallReactClipboard(string message);

        [DllImport("__Internal")]
        public static extern void SendTG_URL(string message);

        [DllImport("__Internal")]
        public static extern void Quit();
        public void OnOrientationUpdated(string orientation)
        {
            Debug.Log("OnOrientationUpdated:" + orientation);
            Enums.Orientation oreintation = (Enums.Orientation)Enum.Parse(typeof(Enums.Orientation), orientation, true);
            GameData.Invoke_OnOrientationUpdate(oreintation);
        }
        [ContextMenu("OnOrientationUpdated")]
        public void Debug_OrientationPortrait()
        {
            OnOrientationUpdated(Enums.Orientation.Portrait.ToString());
        }
#endif

    }
}
