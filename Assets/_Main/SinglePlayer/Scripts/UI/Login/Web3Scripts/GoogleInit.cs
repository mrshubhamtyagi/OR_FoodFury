using Google;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FoodFury;
public class GoogleInit : MonoBehaviour
{
    private bool isSignIn;
    public string webClientId = "1006547548137-mvn66kij6v043l0fkqdkau371jr1ee8k.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;
    private void Awake()
    {
        isSignIn = false;
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };
    }

    private void Start()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
    }
    [ContextMenu("Sign in")]
    public void SignInWithGoogle()
    {
        Debug.Log("Google SignIm Init");

#if !UNITY_WEBGL
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(HandleSignInResult);
#endif
    }


    private void HandleSignInResult(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Google Sign-In Error: " + task.Exception.ToString());
            Debug.LogError("Google Sign-In Failed. Check the console for details." + task.Result.Email);
            OverlayWarningPopup.Instance.ShowWarning("Google Sign-In Failed. Check the console for details." + task.Result.Email);
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                        (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);

                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Google Sign-In Canceled.");

        }
        else
        {
            isSignIn = true;
            Debug.Log("Welcome: " + task.Result.DisplayName + "!" + "email:" + task.Result.Email);
            LoginManager.Instance.OnLoginSuccessViaSocialAsync("GMAIL", task.Result.Email, 0);
        }
    }
    private void OnEnable()
    {
        isSignIn = false;
    }
    private void OnDisable()
    {
#if !UNITY_WEBGL
        if (isSignIn)
            GoogleSignIn.DefaultInstance.SignOut();
#endif
    }
}
