using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class MinimapIconClampingMP : MonoBehaviour
    {   public float MinimapSize;
        private Transform MinimapCam;
        Vector3 TempV3;

        private void Start()
        {
            MinimapCam = FindObjectOfType<MinimapCameraMP>().transform;
        }

        void Update()
        {
            TempV3 = transform.parent.transform.position;
            TempV3.y = transform.position.y;
            transform.position = TempV3;
        }

        void LateUpdate()
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, MinimapCam.position.x - MinimapSize,
                    MinimapSize + MinimapCam.position.x),
                transform.position.y,
                Mathf.Clamp(transform.position.z, MinimapCam.position.z - MinimapSize,
                    MinimapSize + MinimapCam.position.z)
            );
        }
    }
}