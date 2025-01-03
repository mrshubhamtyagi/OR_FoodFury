using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class LevelScreen : MonoBehaviour, IScreen
    {
        [Header("-----Map List")]
        [SerializeField] private Transform contentParentMapList;
        [SerializeField] private MapTitlePrefab mapTitlePrefab;

        [Header("-----Map Info")]
        [SerializeField] private RawImage thumbnail;
        [SerializeField] private TextMeshProUGUI mapNameTMP;
        [SerializeField] private TextMeshProUGUI mapInfoTMP;
        [SerializeField] private TextMeshProUGUI dishCountTMP;
        [SerializeField] private TextMeshProUGUI levelStatsTMP;

        [Header("-----Other")]
        public Texture2D defaultMapThumbnail;
        [SerializeField] private LevelItemPrefab levelPrefab;
        [SerializeField] private RectTransform contentParent;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private TextMeshProUGUI playTMP;
        [SerializeField] private ScrollToLevel scrollToLevel;

        [Header("-----Debug")]
        [SerializeField] private int currentLevelIndex;

        private MapTitlePrefab selectedTitle;
        [SerializeField] private SelectedMap selectedmap;
        [SerializeField] private MapTitlePrefab currentSelectedMap;
        private CanvasGroup canvasGroup;

        public static LevelScreen Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }

        public void OnClick_Home() => ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, this);

        public void OnClick_Settings() => PopUpManager.Instance.ShowPopup(PopUpManager.Instance.SettingsPopup);

        public void OnClick_Fuel()
        {
            AudioManager.Instance.PlayAddFuelButtonSound();
            PopUpManager.Instance.ShowPopup(PopUpManager.Instance.FuelPopup);
        }

        //public void OnClick_Garage() => SceneManager.Swti(GameData.Instance.GameSettings.GarageScene);


        public void PlayerLevel()
        {
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase);
            Loader.Instance.ShowLoader(true);
            GameData.Instance.LoadGame();

        }


        #region Init
        [ContextMenu("InitLevels")]
        private void InitLevels()
        {
            int _childCount = contentParent.childCount;
            int _maxLevel = GameData.Instance.GetLevels().Count; //Mathf.Clamp(currentLevelIndex + 5, 0, GameData.Instance.LevelData.levels.Count);

            int _level = GameData.Instance.GetPlayerLevelNumberData().levelNumber;
            int _totalLevels = GameData.Instance.LevelData.levels.Count;
            currentLevelIndex = _level > _totalLevels ? _totalLevels + 1 : _level;
            GameData.Instance.SelectedLevelNumber = currentLevelIndex;
            LevelItemPrefab _levelItem;
            for (int i = 0; i < _maxLevel; i++)
            {
                _levelItem = i < _childCount ? contentParent.GetChild(i).GetComponent<LevelItemPrefab>() : Instantiate(levelPrefab, contentParent);

                if (i == currentLevelIndex - 1) // Current Level
                {
                    _levelItem.SetLevelData(new ModelClass.PlayerLevelStats { level = i + 1 }, true, false);
                }
                else if (i < currentLevelIndex - 1) // Completed Level
                {
                    _levelItem.SetLevelData(GameData.Instance.GetCompletedLevelStats(i), false, true);
                }
                else // Locked Level
                {
                    _levelItem.SetLevelData(new ModelClass.PlayerLevelStats { level = i + 1 }, false, false);
                }

                _levelItem.gameObject.SetActive(true);
                //contentParent.GetChild(i).gameObject.SetActive(true);
            }

            // Disable Extra Items
            for (int i = _maxLevel; i < contentParent.childCount; i++)
                contentParent.GetChild(i).gameObject.SetActive(false);

            GameData.Instance.CalculateMascotLevels();

            scrollRect.GetComponent<ScrollRect>().StopMovement();
            contentParent.anchoredPosition = Vector2.zero;
            scrollToLevel.ScrollToSelectedLevel();
        }



        private void InitMapList()
        {
            foreach (Transform item in contentParentMapList) Destroy(item.gameObject);

            foreach (var map in GameData.Instance.MapData.Data)
            {
                //if (!map.isActive) continue;

                var _currentMap = Instantiate(mapTitlePrefab, contentParentMapList);
                bool _isLocked = !map.isActive;// !GameData.Instance.IsOnboardingCompleted() && map.mapId != 1;
                _currentMap.SetData(map, _isLocked);
                if (map.mapId == GameData.Instance.SelectedMapId)
                {
                    _currentMap.OnClick_Select();

                }
                else
                    _currentMap.Deselect();
            }
        }

        public void SetMapInfo(MapTitlePrefab _title)
        {
            Loader.Instance.ShowLoader();

            // mapNameTMP.text = _title.Data.mapName;
            mapInfoTMP.text = _title.Data.mapDescription;
            dishCountTMP.text = GameData.Instance.GetFilteredDishes().Count.ToString();
            int _count = GameData.Instance.GetCompletedLevels() == null ? 0 : GameData.Instance.GetCompletedLevels().levels.Count;
            levelStatsTMP.text = $"{_count}/{GameData.Instance.GetLevels().Count}";

            StartCoroutine(APIManager.GetMapTexture(_title.Data.mapName + ".jpg", false, (_status, _texture) =>
            {
                if (_status) thumbnail.texture = _texture;
                else if (PopUpManager.Instance != null)
                {
                    OverlayWarningPopup.Instance.ShowWarning("Could not load Map image!");
                    thumbnail.texture = defaultMapThumbnail;
                }
                if (currentSelectedMap != null)
                {
                    currentSelectedMap.gameObject.SetActive(true);
                }
                currentSelectedMap = _title;
                currentSelectedMap.gameObject.SetActive(false);
                selectedmap.SetData(currentSelectedMap.Data, currentSelectedMap.GetThumnailTexture());
                Loader.Instance.HideLoader();
            }));

            if (selectedTitle != null) selectedTitle.Deselect();
            selectedTitle = _title;

            InitLevels();
        }
        #endregion



        [ContextMenu("Init")]
        public void Init()
        {
            // Set Selected Map
            GameData.Instance.SelectedMapId = GameData.Instance.GetCurrentMapId();

            // Set Selected Level
            int _level = GameData.Instance.GetPlayerLevelNumberData().levelNumber;
            int _totalLevels = GameData.Instance.LevelData.levels.Count;
            GameData.Instance.SelectedLevelNumber = _level > _totalLevels ? _totalLevels + 1 : _level;
            //  GameData.Instance.SelectedLevelNumber = GameData.Instance.GetPlayerLevelDataForSelectedMap().level;
            InitMapList();
        }



        public void Show(Action _callback = null)
        {
            ScreenManager.Instance.CurrentScreen = Enums.Screen.MapSelection;
            Init();
            MenuBar.Instance.Show();
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();
                Loader.Instance.HideLoader();
            });
        }

        public void Hide(Action _callback = null)
        {
            Loader.Instance.ShowLoader();
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }
    }
}