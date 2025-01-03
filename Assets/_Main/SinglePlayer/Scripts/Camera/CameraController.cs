using System.IO;
using UnityEngine;

namespace FoodFury
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private RiderBehaviour rider;
        [SerializeField] private ModelClass.MapDetail.CameraSettings settings;
        [SerializeField] private MinimapCamera minimapCamera;
        //[SerializeField] private Vector2 xClamp;
        //[SerializeField] private Vector2 zClamp;

        private Vector3 targetPosition;
        private Vector3 currentVelocity;


        [SerializeField] private ScreenShake cameraShake;

        private void OnEnable()
        {
            Weapon.OnHitPlayer += DoCameraShakeOnHit;
            Weapon.OnWeaponTriggered += DoCameraShake;
        }
        private void OnDisable()
        {
            Weapon.OnHitPlayer -= DoCameraShakeOnHit;
            Weapon.OnWeaponTriggered -= DoCameraShake;
        }
        private void DoCameraShakeOnHit(WeaponType weaponType)
        {
            Debug.Log("Hit Player");
            if (weaponType == WeaponType.ketchup || weaponType == WeaponType.subMissile)
            {
                // Debug.Log("Shake Started");
                cameraShake.ShakeScreen();
            }
        }
        private void DoCameraShake(WeaponType weaponType, Enums.RiderType _riderType)
        {
            //  Debug.Log("Hit Player");
            if (weaponType == WeaponType.ketchup || weaponType == WeaponType.subMissile)
            {
                //  Debug.Log("Shake Started");
                cameraShake.ShakeScreen();
            }
        }

        public void SetCameraSettings(ModelClass.MapDetail.CameraSettings _settings) => settings = _settings;


        public void SetRider(RiderBehaviour _rider)
        {
            rider = _rider;
            minimapCamera.target = rider.transform;
            minimapCamera.transform.parent = null;
            GetRiderPosition();
        }

        private void FixedUpdate()
        {
            if (rider == null) return;

            ControlMainCamera();
        }

        [ContextMenu("GetRiderPosition")]
        public void GetRiderPosition()
        {
            targetPosition = new Vector3(rider.transform.position.x + settings.offset.x, settings.height, rider.transform.position.z + settings.offset.y);
            transform.position = targetPosition;
            transform.rotation = transform.rotation;
        }


        private void ControlMainCamera()
        {
            //Vector3 targetPosition = new Vector3(Mathf.Clamp(target.position.x, minClamp.x, maxClamp.x), Mathf.Clamp(target.position.y, minClamp.y, maxClamp.y), -height);
            targetPosition = new Vector3(rider.transform.position.x + settings.offset.x, settings.height, rider.transform.position.z + settings.offset.y);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, settings.smoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(settings.angleX, settings.angleY, 0)), settings.smoothTime);
            //transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, new Vector3(angleX, angleY, 0), smoothTime);
        }

        //private void ControlMinMapCamera()
        //{
        //    targetPosition = new Vector3(rider.transform.position.x + offset.x, height, rider.transform.position.z + offset.y);
        //    targetPosition.x = Mathf.Clamp(targetPosition.x, xClamp.x, xClamp.y);
        //    targetPosition.z = Mathf.Clamp(targetPosition.z, zClamp.x, zClamp.y);
        //    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
        //}

        #region ScreenShot
#if UNITY_EDITOR
        private void Update()
        {
            return;
            if (Input.GetKeyDown(KeyCode.Backspace) && Application.isEditor)
                TakeScreenshot();
            if (Input.GetKeyDown(KeyCode.Q) && Application.isEditor)
                cameraShake.ShakeScreen();
        }
#endif

        public string screenshotName = "Screenshot.png";
        [ContextMenu("TakeScreenshot")]
        public void TakeScreenshot()
        {
            ScreenCapture.CaptureScreenshot($"E:/Projects/ScreenShot/{screenshotName}", 1);
            //TakeTransparentScreenshot(GetComponent<Camera>(), Screen.width, Screen.height, screenshotName);
            //TakeTransparentScreenshot(GetComponent<Camera>(), 2048, 2048, $"/Users/Office/Desktop/{screenshotName}");
        }

        private static void TakeTransparentScreenshot(Camera cam, int width, int height, string savePath)
        {
            // Depending on your render pipeline, this may not work.
            var bak_cam_targetTexture = cam.targetTexture;
            var bak_cam_clearFlags = cam.clearFlags;
            var bak_RenderTexture_active = RenderTexture.active;

            var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
            // Must use 24-bit depth buffer to be able to fill background.
            var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            var grab_area = new Rect(0, 0, width, height);

            RenderTexture.active = render_texture;
            cam.targetTexture = render_texture;
            cam.clearFlags = CameraClearFlags.SolidColor;

            // Simple: use a clear background
            cam.backgroundColor = Color.clear;
            cam.Render();
            tex_transparent.ReadPixels(grab_area, 0, 0);
            tex_transparent.Apply();

            // Encode the resulting output texture to a byte array then write to the file
            byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
            File.WriteAllBytes(savePath, pngShot);

            cam.clearFlags = bak_cam_clearFlags;
            cam.targetTexture = bak_cam_targetTexture;
            RenderTexture.active = bak_RenderTexture_active;
            RenderTexture.ReleaseTemporary(render_texture);
            Texture2D.Destroy(tex_transparent);
        }
    }
    #endregion
}