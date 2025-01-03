using UnityEngine;
using UnityEngine.UI;
using Assets.SimpleSignIn.Telegram.Scripts;

namespace Assets.SimpleSignIn.Telegram
{
    public class Example : MonoBehaviour
    {
        public TelegramAuth TelegramAuth;
        public Text Log;
        public Text Output;
        
        public void Start()
        {
            Application.logMessageReceived += (condition, _, _) => Log.text += condition + '\n';
            TelegramAuth = new TelegramAuth();
            TelegramAuth.TryResume(OnSignIn);
        }

        public void SignIn()
        {
            TelegramAuth.SignIn(OnSignIn, caching: true);
        }

        public void SignOut()
        {
            TelegramAuth.SignOut();
            Output.text = "Not signed in";
        }

        private void OnSignIn(bool success, string error, UserInfo userInfo)
        {
            Output.text = success ? $"Hello, {userInfo.Username}!" : error;
        }

        public void Navigate(string url)
        {
            Application.OpenURL(url);
        }
    }
}