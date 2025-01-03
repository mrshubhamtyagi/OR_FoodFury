using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
#endif
namespace FoodFury.Sharing
{
    public class SharingManager
    {
        public static void ShareOnTwitter(string text, Action<bool> callback, Texture2D image = null, string url = null)
        {
#if !UNITY_WEBGL


            if (SocialShareComposer.IsComposerAvailable(SocialShareComposerType.Twitter) == false)
            {
                callback?.Invoke(false);
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.twitter.android&hl=en&gl=US");
                //open link  to twitter
            }
            else
            {
                SocialShareComposer composer = SocialShareComposer.CreateInstance(SocialShareComposerType.Twitter);
                composer.SetText(text);
                if (image != null)
                {
                    Texture2D uncompressedTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);
                    uncompressedTexture.SetPixels(image.GetPixels());
                    uncompressedTexture.Apply();
                    composer.AddImage(uncompressedTexture);
                }
                if (url != null)
                {
                    composer.AddURL(URLString.URLWithPath(url));
                }
                composer.SetCompletionCallback((result, error) =>
                {
                    if (result.ResultCode == SocialShareComposerResultCode.Done || result.ResultCode == SocialShareComposerResultCode.Unknown)
                    {
                        Debug.Log("Social Share Composer was closed. Result code: " + result.ResultCode);
                        callback?.Invoke(true);

                    }
                    else
                    {
                        callback?.Invoke(false);
                    }
                });
                composer.Show();
            }
#endif
        }
        public static void ShareOnAvailablePlatforms(string text, Action<bool> callback, Texture2D image = null, string url = null)
        {
#if !UNITY_WEBGL
            ShareSheet shareSheet = ShareSheet.CreateInstance();
            shareSheet.AddText(text);
            if (image != null)
            {
                Texture2D uncompressedTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

                uncompressedTexture.SetPixels(image.GetPixels());
                uncompressedTexture.Apply();
                shareSheet.AddImage(uncompressedTexture);
            }
            if (url != null)
            {
                shareSheet.AddURL(URLString.URLWithPath(url));
            }
            shareSheet.SetCompletionCallback((result, error) =>
            {
                if (result.ResultCode == ShareSheetResultCode.Done || result.ResultCode == ShareSheetResultCode.Unknown)
                {
                    Debug.Log("Social Share Composer was closed. Result code: " + result.ResultCode);
                    callback?.Invoke(true);

                }
                else
                {
                    callback?.Invoke(false);
                }

            });
            shareSheet.Show();
#endif
        }


        public static void ShareTextOnTwitterDebug()
        {
            // ShareOnTwitter("Hello World");
        }
        public void ShareTextAndImageOnTwitterDebug()
        {
            Texture2D image = Resources.Load<Texture2D>("palette.png");
            Texture2D uncompressedTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

            uncompressedTexture.SetPixels(image.GetPixels());
            uncompressedTexture.Apply();
            //ShareOnTwitter("Hello World", uncompressedTexture);
        }
        public static void ShareTextAndImageAndUrlOnTwitterDebug()
        {
            Texture2D image = Resources.Load<Texture2D>("palette");
            Texture2D uncompressedTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

            uncompressedTexture.SetPixels(image.GetPixels());
            uncompressedTexture.Apply();

            //  ShareOnTwitter("Hello World", uncompressedTexture, "https://www.google.com");

        }
        public static void ShareAnywhereDebug()
        {
            Texture2D image = Resources.Load<Texture2D>("palette");
            Texture2D uncompressedTexture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

            uncompressedTexture.SetPixels(image.GetPixels());
            uncompressedTexture.Apply();
            //  ShareOnAvailablePlatforms("Hello World", uncompressedTexture, "https://www.google.com");

        }
    }
}


