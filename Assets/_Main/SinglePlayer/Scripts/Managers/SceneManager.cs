using System;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : MonoBehaviour
{
    public static string LoginScene => "LoginScene";
    public static string HomeScene => "HomeScene";
    public static string LobbyScene => "LobbyScene";
    public static string DrivingScene => "DrivingScene";


    public static SceneManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }



    public static void SwitchToHomeScene(UnityEngine.SceneManagement.LoadSceneMode _mode = UnityEngine.SceneManagement.LoadSceneMode.Single, Action<AsyncOperation> _callback = null) =>
        LoadSceneAsync(HomeScene, _mode, _callback);

    public static void SwitchToDrivingScene(UnityEngine.SceneManagement.LoadSceneMode _mode = UnityEngine.SceneManagement.LoadSceneMode.Single, Action<AsyncOperation> _callback = null) =>
        LoadSceneAsync(DrivingScene, _mode, _callback);

    public static void SwitchToLoginScene(UnityEngine.SceneManagement.LoadSceneMode _mode = UnityEngine.SceneManagement.LoadSceneMode.Single, Action<AsyncOperation> _callback = null) =>
        LoadSceneAsync(LoginScene, _mode, (op) => { LoginManager.Instance.Init(); _callback?.Invoke(op); });



    public static void LoadSceneAsync(string _sceneName, UnityEngine.SceneManagement.LoadSceneMode _mode = UnityEngine.SceneManagement.LoadSceneMode.Single, Action<AsyncOperation> _callback = null) =>
        UnitySceneManager.LoadSceneAsync(_sceneName, _mode).completed += _callback;

    public static void UnloadSceneAsync(string _sceneName, Action<AsyncOperation> _callback = null) =>
        UnitySceneManager.UnloadSceneAsync(_sceneName).completed += _callback;
}
