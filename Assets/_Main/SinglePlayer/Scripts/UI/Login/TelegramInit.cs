//using Assets.SimpleSignIn.Telegram.Scripts;
using UnityEngine;

namespace FoodFury
{
    public class TelegramInit : MonoBehaviour
    {
       // private TelegramAuth TelegramAuth;

        private bool isSignIn;

        public void Start()
        {
            isSignIn = false;
        //    TelegramAuth = new TelegramAuth();
          //  TelegramAuth.TryResume(OnSignIn);
        }

        private void OnEnable() => isSignIn = false;
        private void OnDisable()
        {
#if !UNITY_WEBGL
            if (isSignIn)
                SignOut();
#endif
        }


        public void SignIn() => Debug.Log("Sign in");//TelegramAuth.SignIn(OnSignIn, caching: false);


        public void SignOut() => Debug.Log("Sign out"); //TelegramAuth.SignOut();


        private void OnSignIn(bool success, string error)//, UserInfo userInfo)
        {
            //if (!success)
            //{
            //    OverlayWarningPopup.Instance.ShowWarning(error);
            //    return;
            //}

            //Debug.Log($"Welcome: {userInfo.Username} | ID: {userInfo.Id} name:{userInfo.FirstName}");
            //LoginManager.Instance.OnLoginSuccessViaSocialAsync("TELEGRAM", userInfo.Id.ToString(), 0);
        }

    }
}