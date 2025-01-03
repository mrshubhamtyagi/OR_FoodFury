using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodFury;
using Tanknarok.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace OneRare.FoodFury.Multiplayer
{
    public class LevelBehaviour : MonoBehaviour
    {
        // Class for storing the lighting settings of a level
        [System.Serializable]
        public struct LevelLighting
        {
            public Color ambientColor;
            public Color fogColor;
            public bool fog;
        }

        [SerializeField] private LevelLighting levelLighting;

        [SerializeField] private List<SpawnPoint> playerSpawnPoints;
       
        private void Awake()
        {
            playerSpawnPoints = new List<SpawnPoint>();
            playerSpawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .ToList();

            //LoadGame();
        }

        /*private void Start()
        {
            //LoadGame();
        }*/

        public async void LoadGame()
        {
            //OnLevelSelected?.Invoke();
            /*if (ScreenManager.Instance.CurrentScreen != Enums.Screen.Gameplay)
            {*/

            string mapName = "London.unity";
            if (GameData.Instance.MapData.HasMap(GameData.Instance.GameSettings.multiplayerMapId))
            {
                mapName =
                    $"{GameData.Instance.MapData.GetMapDetails(GameData.Instance.GameSettings.multiplayerMapId).sceneName}.unity";
            }
            else
            {
                Debug.LogError($"Map {GameData.Instance.GameSettings.multiplayerMapId} not found");
            }

            var _success = await AddressableLoader.LoadSceneAsync(mapName,
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                _progress => Loader.Instance.UpdateLoadingPercentage(_progress));

            if (_success == AddressableLoader.LoadSceneResult.Error)
            {
                ErrorBox.Show("Disconnected!", "Something went wrong! [SceneError]", () => OnDisconnect());
                //PopUpManager.Instance.ShowWarningPopup("Something went wrong! [SceneError]");
                Loader.Instance.HideLoader();
                return;
            }

            if (_success == AddressableLoader.LoadSceneResult.Cancelled)
                return;

            //GetMapMusic_API(() => { AddressableLoader.SpawnObject<GameController>(gameControllerPrefab).StartSetup(); });

            //GetMapMusic_API(() => { AddressableLoader.SpawnObject<GameController>(gameControllerPrefab).StartSetup(); });
            /*}
            else
            {
                GameController.Instance.SetNextLevel(false);
                Loader.Instance.HideLoader();
            }*/
        }

        public static async Task<bool> GetMapMusic_API()
        {
            var _response = await APIManager.DownloadTrack(GameData.Instance.GetSelectedMapData().mapTrack);
            //Debug.Log($"GetMapData_API - Status {_response.error} | Response {_response.result}");

            if (!_response.error) return false;
            AudioManager.Instance.SetGamePlayMusicAudio(_response.result);
            return true;
        }

        public void Activate()
        {
            GameManager.IsGamePaused = true;
            print($"LoadGame IsGamePaused - {GameManager.IsGamePaused}");
            LoadGame();
        }

        private void SetLevelLighting()
        {
            RenderSettings.ambientLight = levelLighting.ambientColor;
            RenderSettings.fogColor = levelLighting.fogColor;
            RenderSettings.fog = levelLighting.fog;
        }

        private MobileInput mobileInput;
        private GameManager gameManager;

        public void OnDisconnect()
        {
            gameManager = FindFirstObjectByType<GameManager>();
            gameManager.ShutdownOnInput();
            //mobileInput = FindObjectOfType<MobileInput>();
            //mobileInput.OnDisconnect();
        }

        public SpawnPoint GetPlayerSpawnPoint(int plyIndex)
        {
            return playerSpawnPoints[plyIndex].GetComponent<SpawnPoint>();
        }
    }
}