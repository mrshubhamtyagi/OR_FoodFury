using System.Collections.Generic;
using UnityEngine;
using FoodFury;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core.Environments;
using static FoodFury.ModelClass;
#if UNITY_IOS
using Assets.SimpleSignIn.Telegram.Scripts;
#endif

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private string playStoreURL => "https://play.google.com/store/apps/details?id=com.OneRare.FoodFury";

    [SerializeField] private string appStoreURL => "https://apps.apple.com/in/app/food-fury/id6504471602";

    [SerializeField] private string playerId;
    private string via;

    [Header("-----UpdateScreen")] [SerializeField]
    private GameObject updateScreen;

    [SerializeField] private GameObject dummyLogin, loginButtonHolder;

    [Header("-----Debugs")] [SerializeField]
    private bool activateDummyLogin;

    [SerializeField] private string gmail;
    [SerializeField] private string phone;
    [SerializeField] private string metamask;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField, TextArea] private string jsonstringForWebgl;
    private string url;

    [Space(20)] [SerializeField] private TestId[] testIds;

    public static LoginManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }

    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
        }

        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }

        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    // [ContextMenu("Init")]
    public async void Init()
    {
        if (GameData.Instance.serverMode == Enums.ServerMode.MainNet || !Application.isEditor)
            playerId = "";

        Loader.Instance.ShowLoader();
        loginButtonHolder.SetActive(!activateDummyLogin);
        dummyLogin.SetActive(activateDummyLogin);

        await CheckAndInitializeUnityServices();
        if (GameData.Instance.Platform != Enums.Platform.WebGl)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            if (IsUpdateVersion())
            {
                if (!string.IsNullOrEmpty(playerId))
                {
                    via = "Android";
                    SceneManager.SwitchToHomeScene(LoadSceneMode.Additive, UnloadLoginScreen);
                    return;
                }

                if (GameData.Instance.releaseMode == Enums.ReleaseMode.Release) LoginFromPlayerPref();
                else Loader.Instance.HideLoader();
            }
            else
            {
                if (GameData.Instance.GameSettings.resetPrefsOnUpdate)
                {
                    PlayerPrefsManager.Instance.RemovePlayerId();
                    PlayerPrefsManager.Instance.RemoveVia();
                }

                updateScreen.gameObject.SetActive(true);
                Loader.Instance.HideLoader();
            }
        }
        else
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;

            if (IsUpdateVersion())
            {
                // GetPlayerIdFromURL();

                if (!string.IsNullOrEmpty(playerId))
                {
                    via = "WebGL";
                    AnalyticsManager.Instance.FireLoginEvent(false);
                    SceneManager.SwitchToHomeScene(LoadSceneMode.Additive, UnloadLoginScreen);
                }
                else
                {
                    OverlayWarningPopup.Instance.ShowWarning("Please reopen the app");
                    PlayerPrefsManager.Instance.RemovePlayerId();
                    PlayerPrefsManager.Instance.RemoveVia();
                    // Loader.Instance.HideLoader();
                }
            }
            else
            {
                if (GameData.Instance.GameSettings.resetPrefsOnUpdate)
                {
                    PlayerPrefsManager.Instance.RemovePlayerId();
                    PlayerPrefsManager.Instance.RemoveVia();
                }

                updateScreen.gameObject.SetActive(true);
                Loader.Instance.HideLoader();
            }
        }
    }

    // [ContextMenu("SetPlayerData")]
    private void SetPlayerData_Debug()
    {
        SetPlayerIdAndReferalUrl_JS(jsonstringForWebgl);
    }

    public void SetPlayerIdAndReferalUrl_JS(string jsonData)
    {
        Debug.Log("Recieved player id from webgl :" + jsonData);
        PlayerIdAndReferal playerIdAndReferal = JsonConvert.DeserializeObject<PlayerIdAndReferal>(jsonData);
        playerId = playerIdAndReferal._id;
        GameData.Instance.SetReferalUrl(playerIdAndReferal.referalUrl);
        GameData.Instance.SetTelegramUserName(playerIdAndReferal.userName);
    }

    public async Task CheckAndInitializeUnityServices()
    {
        if (AnalyticsManager.Instance.environmentType == Enums.EnvironmentType.editor) return;

        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            var options = new InitializationOptions();
            options.SetEnvironmentName(AnalyticsManager.Instance.environmentType.ToString());
            try
            {
                await UnityServices.InitializeAsync(options);
                // await Task.Delay(100);
                await SignInAnonymouslyAsync();
                // await Task.Delay(100);
                AnalyticsService.Instance.StartDataCollection();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }


    // [ContextMenu("Login Via PlayerPref")]
    public void LoginFromPlayerPref()
    {
        if (string.IsNullOrEmpty(PlayerPrefsManager.Instance.PlayerId) == false)
        {
            Loader.Instance.ShowLoader();
            playerId = PlayerPrefsManager.Instance.PlayerId;
            AnalyticsManager.Instance.FireLoginEvent(true);
            SceneManager.SwitchToHomeScene(LoadSceneMode.Additive, UnloadLoginScreen);
        }
        else Loader.Instance.HideLoader();
    }

    // Handle -> Actual Address
    public async void OnLoginSuccessViaSocialAsync(string _via, string _handle, int referId)
    {
        Loader.Instance.ShowLoader();
        via = _via;
        Dictionary<string, object> _params = new() { { "via", _via } };
        _params.Add("handle", _handle);
        _params.Add("referId", referId);
        _params.Add("platform",
            (GameData.Instance.Platform == Enums.Platform.iOS || GameData.Instance.Platform == Enums.Platform.Android)
                ? "native"
                : "web");

        JObject jobject = _params.ToJSONObject();
        Debug.Log(jobject);

        //playerId = "668f7f0fdb726502ff15ae30";
        AnalyticsManager.Instance.FireLoginEvent(false);
        //SceneManager.SwitchToHomeScene(LoadSceneMode.Additive, UnloadLoginScreen);

        var _response = await APIManager.POST_SignUpPlayer(jobject);

        if (!_response.error)
        {
            Debug.Log(_response.result);
            //Debug.Log(JsonConvert.DeserializeObject<ErrorAndResultResponse<PlayerData>>(_response.result));
            ErrorAndResultResponse<PlayerData> _data =
                JsonConvert.DeserializeObject<ErrorAndResultResponse<PlayerData>>(_response.result);
            if (!_data.error)
            {
                playerId = _data.result._id;
                GameData.Instance.UpdatePlayerDataLocal(_data.result);
                GameData.Instance.AddMapsToPlayerLevels();
                AnalyticsManager.Instance.FireLoginEvent(false);
                SceneManager.SwitchToHomeScene(LoadSceneMode.Additive, UnloadLoginScreen);
            }
            else
            {
                Loader.Instance.HideLoader();
                OverlayWarningPopup.Instance.ShowWarning($"Could not load Player Data [{_data.message}]");
            }
        }
        else
        {
            Loader.Instance.HideLoader();
            OverlayWarningPopup.Instance.ShowWarning($"Could not load Player Data!!!");
        }
    }


    // [ContextMenu("GetPlayerIdFromURL")]
    public void GetPlayerIdFromURL()
    {
        string _id = string.Empty;
        string _url = Application.isEditor ? url : Application.absoluteURL;
        _url = url; // For Telegram

        string[] _splitArr = _url.Split('?');

        if (_splitArr.Length > 1) _id = _splitArr[1];
        Debug.Log("Decrypted -   " + _id);

        //if (encrypt && AESEncryptionECB.IsAes256Encrypted(_address))
        //{
        //    _address = AESEncryptionECB.Decrypt(_address);
        //    _address = _address.Remove(0, 1);
        //    _address = _address.Remove(_address.Length - 1, 1);
        //    Debug.Log("Decrypted -   " + _address);
        //}

        playerId = _id;

        //GetPlayerData(_address);
    }


    private void UnloadLoginScreen(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            //  obj.allowSceneActivation = true;
            PlayerPrefsManager.Instance.SavePlayerId(playerId);
            PlayerPrefsManager.Instance.SaveServerMode();
            PlayerPrefsManager.Instance.SaveVia(via);
            GameData.Instance.Init(playerId);
            SceneManager.UnloadSceneAsync(SceneManager.LoginScene);
        }
    }

    public void OnClick_Update()
    {
        if (GameData.Instance.Platform != Enums.Platform.WebGl)
        {
#if UNITY_ANDROID || UNITY_EDITOR
            Application.OpenURL(playStoreURL);
#elif UNITY_IOS
            SafariViewController.OpenURL(appStoreURL);
#endif
            // Loader.Instance.ShowLoader();
            PlayerPrefsManager.Instance.RemovePlayerId();
            PlayerPrefsManager.Instance.RemoveVia();
            GameData.Instance.ResetPlayerData();
            AnalyticsManager.Instance.FireLogoutEvent();
            //SceneManager.SwitchToLoginScene();
        }
        else
        {
#if UNITY_WEBGL
            JavaScriptCallbacks.Quit();
#endif
        }

        // Application.OpenURL(playStoreURL);
    }

    public bool IsUpdateVersion()
    {
        string _serverVersion = GameData.Instance.Platform == Enums.Platform.iOS
            ? GameData.Instance.GameSettings.gameVersioniOS
            : GameData.Instance.Platform == Enums.Platform.Android
                ? GameData.Instance.GameSettings.gameVersionAndroid
                : GameData.Instance.GameSettings.GameVersion; // WebGL Condition
        if (string.IsNullOrWhiteSpace(_serverVersion)) return false;

        var version1 = new Version(GameData.Instance.ReleasedVersion);
        var version2 = new Version(_serverVersion);
        return version1.CompareTo(version2) >= 0;
    }


    public void UsernameAndPasswordAppleLogin(string _email) => OnLoginSuccessViaSocialAsync("APPLE", _email, 0);

    [ContextMenu("Login Via PlayerID")]
    public void TestPlayerId() => SceneManager.SwitchToHomeScene(LoadSceneMode.Additive, UnloadLoginScreen);

    [ContextMenu("Login Via Google")]
    public void TestGoogleLogin() => OnLoginSuccessViaSocialAsync("GMAIL", gmail, 0);


    [ContextMenu("Login Via Apple")]
    public void TestAppleLogin() => OnLoginSuccessViaSocialAsync("APPLE", gmail, 0);

    //[ContextMenu("Login Via Phone")]
    //public void TestPhoneLogin() => OnLoginSuccessViaSocialAsync("PHONE", phone, 0);

    //[ContextMenu("Login Via Metamask")]
    //public void TestMetamask() => OnLoginSuccessViaSocialAsync("METAMASK", metamask, 0);
}


[Serializable]
public class TestId
{
    public string user;
    public string userId;
}