using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace FoodFury
{
    public class AddressableLoader : MonoBehaviour
    {
        private static SceneInstance sceneInstance;

        public static bool isMapLoading;
        public static LoadSceneResult MapLoadStatus;

        public static AddressableLoader Instance;
        void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                Instance = this;
                isMapLoading = false;
                MapLoadStatus = LoadSceneResult.Cancelled;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            Addressables.InitializeAsync();
            //Addressables.CleanBundleCache();
        }

        public static T SpawnObject<T>(GameObject _prefab, Transform _parent = null) => Instantiate(_prefab, _parent).GetComponent<T>();



        public static void InstantiateObject(string _key, Transform _parent, Action<GameObject> _onComplete = null)
        {
            Addressables.InstantiateAsync(_key, _parent).Completed += handler =>
            {
                if (handler.Status == AsyncOperationStatus.Succeeded)
                    _onComplete?.Invoke(handler.Result);
                else
                    _onComplete?.Invoke(null);
            };
        }
        public static void InstantiateObject(AssetReference _asset, Transform _parent, Action<GameObject> _onComplete = null)
        {
            _asset.InstantiateAsync(_parent).Completed += handler =>
            {
                if (handler.Status == AsyncOperationStatus.Succeeded)
                    _onComplete?.Invoke(handler.Result);
                else
                    _onComplete?.Invoke(null);
            };
        }

        public static void DeinstantiateObject(GameObject _instance)
        {
            if (_instance == null) return;
            Addressables.ReleaseInstance(_instance);
            Destroy(_instance);
        }


        public static async Task<LoadSceneResult> LoadSceneAsync(string _key, LoadSceneMode _mode, Action<float> _progress = null)
        {
            isMapLoading = true;
            var _operation = Addressables.LoadSceneAsync(_key, _mode, true);
            while (!_operation.IsDone)
            {
                _progress?.Invoke(_operation.GetDownloadStatus().Percent);
                await Task.Yield();
            }

            print($"---Map Loaded % - {_operation.GetDownloadStatus().Percent}");

            if (_operation.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Could not load Scene - " + _key);
                isMapLoading = false;
                MapLoadStatus = LoadSceneResult.Error;
                return LoadSceneResult.Error;
            }

            isMapLoading = false;
            MapLoadStatus = LoadSceneResult.Success;
            sceneInstance = _operation.Result;
            if (GameData.Instance.gameMode == Enums.GameModeType.SinglePlayer)
            {
                if (ScreenManager.Instance.CurrentScreen != Enums.Screen.PreGameplay)
                {
                    Debug.Log("Loaded scene at wrong Screen - " + _key);
                    await UnloadSceneAsync();
                    //Loader.Instance.HideLoader();
                    return LoadSceneResult.Cancelled;
                }
            }
            else
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Debug.Log("Loaded scene at wrong Screen - " + _key);
                    await UnloadSceneAsync();
                    //Loader.Instance.HideLoader();
                    return LoadSceneResult.Cancelled;
                }
            }


            return LoadSceneResult.Success;
        }

        public static async Task<bool> UnloadSceneAsync()
        {
            print($"IsLoaded:{sceneInstance.Scene.isLoaded} | IsValid: {sceneInstance.Scene.IsValid()} | NULL:{sceneInstance.Scene == null}");
            isMapLoading = false;
            MapLoadStatus = LoadSceneResult.Cancelled;
            if (!sceneInstance.Scene.isLoaded) return true;

            var _operation = Addressables.UnloadSceneAsync(sceneInstance);
            while (!_operation.IsDone) await Task.Yield();

            //if (_operation.Status != AsyncOperationStatus.Succeeded)
            //{
            //    print("4");
            //    Debug.Log("Could not unload Scene - " + sceneInstance.Scene.name);
            //    return false;
            //}

            //sceneInstance = default;
            return true;
        }



        public enum LoadSceneResult { Success, Error, Cancelled }

    }
}