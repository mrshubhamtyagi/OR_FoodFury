using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using UnityEngine;

namespace FoodFury
{
    public class AppleSignIn : MonoBehaviour
    {
        private IAppleAuthManager appleAuthManager;
        private bool checkForUpdate;

        //private void Start()
        //{
        //    // If the current platform is supported
        //    if (AppleAuthManager.IsCurrentPlatformSupported)
        //    {
        //        // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
        //        var deserializer = new PayloadDeserializer();
        //        // Creates an Apple Authentication manager with the deserializer
        //        _appleAuthManager = new AppleAuthManager(deserializer);
        //    }
        //}

        private void Update()
        {
            // Updates the AppleAuthManager instance to execute
            // pending callbacks inside Unity's execution loop
            if (checkForUpdate && appleAuthManager != null)
                appleAuthManager.Update();
        }


        public void CheckCredentialStatusForUserId(string appleUserId)
        {
            checkForUpdate = true;

            // If there is an apple ID available, we should check the credential state
            appleAuthManager.GetCredentialState(appleUserId,
                state =>
                {
                    checkForUpdate = false;
                    switch (state)
                    {
                        // If it's authorized, login with that user id
                        case CredentialState.Authorized:
                            // Skip login
                            return;

                        // If it was revoked, or not found, we need a new sign in with apple attempt
                        // Discard previous apple user id
                        case CredentialState.Revoked:
                        case CredentialState.NotFound:
                            // Show Login Screen & Delete Player Prefs
                            return;
                    }
                },
                error =>
                {
                    checkForUpdate = false;
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Error while trying to get credential state " + authorizationErrorCode.ToString() + " " + error.ToString());
                    // Show Login Screen & Delete Player Prefs
                });
        }

        public void AttemptQuickLogin()
        {
            checkForUpdate = true;
            var quickLoginArgs = new AppleAuthQuickLoginArgs();

            // Quick login should succeed if the credential was authorized before and not revoked
            appleAuthManager.QuickLogin(quickLoginArgs,
                credential =>
                {
                    checkForUpdate = false;
                    Debug.LogWarning($"-----APPLE USER ID : {credential.User}-----");

                    // If it's an Apple credential, save the user ID, for later logins
                    //var _cred = credential as IAppleIDCredential;
                    //if (_cred != null)
                    //{
                    //    var _userId = _cred.User; // Apple User ID. You should save the user ID somewhere in the device
                    //    var _email = _cred.Email; //(Received ONLY in the first login)
                    //    var fullName = _cred.FullName; //(Received ONLY in the first login)

                    //    // Identity token && Authorization code
                    //    var identityToken = Encoding.UTF8.GetString(_cred.IdentityToken, 0, _cred.IdentityToken.Length);
                    //    var authorizationCode = Encoding.UTF8.GetString(_cred.AuthorizationCode, 0, _cred.AuthorizationCode.Length);
                    //    Debug.Log($"Welcome: {fullName.GivenName } | Email: {_email} | UserID: {_userId}");
                    //}

                    LoginManager.Instance.OnLoginSuccessViaSocialAsync("APPLE", credential.User, 0);
                },
                error =>
                {
                    // If Quick Login fails, we should show the normal sign in with apple menu, to allow for a normal Sign In with apple
                    checkForUpdate = false;
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Quick Login Failed " + authorizationErrorCode.ToString() + " " + error.ToString());

                });
        }

        public void SignInWithApple()
        {
            if (!AppleAuthManager.IsCurrentPlatformSupported)
            {
                Debug.Log("[Apple Sign In] PLATFROM NOT SUPPORTED");
                return;
            }

            if (appleAuthManager == null)
            {
                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();

                // Creates an Apple Authentication manager with the deserializer
                appleAuthManager = new AppleAuthManager(deserializer);
            }


            checkForUpdate = true;
            var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
            appleAuthManager.LoginWithAppleId(loginArgs,
                credential =>
                {
                    checkForUpdate = false;
                    Debug.LogWarning($"-----APPLE USER ID : {credential.User}-----");

                    // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it

                    //var _cred = credential as IAppleIDCredential;
                    //if (_cred != null)
                    //{
                    //    var _userId = _cred.User; // Apple User ID. You should save the user ID somewhere in the device
                    //    var _email = _cred.Email; //(Received ONLY in the first login)
                    //    var fullName = _cred.FullName; //(Received ONLY in the first login)

                    //    // Identity token && Authorization code
                    //    var identityToken = Encoding.UTF8.GetString(_cred.IdentityToken, 0, _cred.IdentityToken.Length);
                    //    var authorizationCode = Encoding.UTF8.GetString(_cred.AuthorizationCode, 0, _cred.AuthorizationCode.Length);
                    //    Debug.Log($"Welcome: {fullName.GivenName } | Email: {_email} | UserID: {_userId}");
                    //}

                    LoginManager.Instance.OnLoginSuccessViaSocialAsync("APPLE", credential.User, 0);
                },
                error =>
                {
                    checkForUpdate = false;
                    var authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode.ToString() + " " + error.ToString());
                });
        }
    }
}
