using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class GameStartPopup : PopupBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelTMP;
        //[SerializeField] private GameObject levelObjectivesParent;
        [SerializeField] private GameObject levelConditionsParent;
        [SerializeField] private GameObject levelBoosters;


        [Header("-----Objectives & Conditions")]
        [SerializeField] private GameObject objectiveSeparator;
        [SerializeField] private TextMeshProUGUI deliverValueTMP;
        [SerializeField] private TextMeshProUGUI emojiValueTMP;
        [SerializeField] private Image emojiIcon;
        private GameObject deliverParent, emojiParent;
        [Space(20)]
        [SerializeField] private TextMeshProUGUI healthValueTMP;
        [SerializeField] private TextMeshProUGUI timeValueTMP;
        private GameObject healthParent, timeParent;
        [SerializeField] private LevelDataSO.Level Test_level;

        [Header("---Boosters")]
        [SerializeField] private BoosterPrefab boosterLeft;
        [SerializeField] private BoosterPrefab boosterMiddle;
        [SerializeField] private BoosterPrefab boosterRight;

        [Header("---Engine Health")]
        [SerializeField] private Image enginerHealthImg;
        [SerializeField] private TextMeshProUGUI enginerHealthTMP;

        private ModelClass.GarageVehicleData vehicleData;

        void Awake()
        {
            Init(GetComponent<CanvasGroup>());
            deliverParent = deliverValueTMP.transform.parent.parent.gameObject;
            emojiParent = emojiValueTMP.transform.parent.parent.gameObject;

            healthParent = healthValueTMP.transform.parent.gameObject;
            timeParent = timeValueTMP.transform.parent.gameObject;
        }

        //private void OnEnable()
        //{
        //    BoosterPrefab.OnBoosterAdd += OnBoosterAdd;
        //    BoosterPrefab.OnBoosterRemove += OnBoosterRemove;
        //}

        //private void OnDisable()
        //{
        //    BoosterPrefab.OnBoosterAdd -= OnBoosterAdd;
        //    BoosterPrefab.OnBoosterRemove -= OnBoosterRemove;
        //}

        public override void Init()
        {
            base.Init();
            GameController.IsGamePaused = true;
            ScreenManager.Instance.CurrentScreen = Enums.Screen.PreGameplay;
            ResetInitialBooster();
            MenuBar.Instance.Show();

            levelTMP.text = "Level " + GameData.Instance.SelectedLevelNumber;
            SetObjective(GameController.Instance.LevelInfo.CurrentLevel);

            if (GameController.Instance.LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out int _value))
            {
                BoosterPrefab.OnBoosterAdd -= OnBoosterAdd;
                BoosterPrefab.OnBoosterRemove -= OnBoosterRemove;

                BoosterPrefab.OnBoosterAdd += OnBoosterAdd;
                BoosterPrefab.OnBoosterRemove += OnBoosterRemove;

                vehicleData = GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle);
                int _health = GetHealthStatus();

                enginerHealthTMP.text = UpdateTextColorBasedOnHealth(_value, _health);
                // enginerHealthImg.color = _health < _value ? ColorManager.Instance.Red : ColorManager.Instance.LightGreen;
                enginerHealthImg.gameObject.SetActive(true);
            }
            else enginerHealthImg.gameObject.SetActive(false);

            InitBoosters();



            void InitBoosters()
            {
                bool _flag = GameController.Instance.LevelInfo.CurrentLevel.boosterDetails.Count == 0;
                boosterLeft.DisableBooster(false);
                boosterMiddle.DisableBooster(false);
                boosterRight.DisableBooster(false);

                int _boosterCost = Mathf.Clamp(GameController.Instance.LevelInfo.CurrentLevel.levelBoosterCost * GameData.Instance.SelectedLevelNumber, 0, 150);
                boosterLeft.SetCost(_boosterCost.ToString());
                boosterMiddle.SetCost(_boosterCost.ToString());
                boosterRight.SetCost(_boosterCost.ToString());

                //if (GameData.Instance.SelectedMapId == 1) boosterLeft.OnClick_Add();
            }
        }

        private static string UpdateTextColorBasedOnHealth(int _value, int _health)
        {
            return _health >= _value ? $"<color=#BDFF00>{_health}</color>%" : $"<color=#FF5361>{_health}%</color>";
        }

        private void OnBoosterAdd(Enums.LevelBoosterType _type)
        {
            //print("OnBoosterAdd");
            int _health = GetHealthStatus();

            _health += _type == Enums.LevelBoosterType.Shield ? 25 : 0;
            GameController.Instance.LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out int _value);
            enginerHealthTMP.text = UpdateTextColorBasedOnHealth(_value, _health);

            //enginerHealthImg.color = _health < _value ? ColorManager.Instance.Red : ColorManager.Instance.LightGreen;
        }


        private void OnBoosterRemove(Enums.LevelBoosterType _type)
        {
            //print("OnBoosterRemove");
            if (_type != Enums.LevelBoosterType.Shield) return;

            int _health = GetHealthStatus();
            if (GameController.Instance.LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out int _value))
                enginerHealthTMP.text = UpdateTextColorBasedOnHealth(_value, _health);
            //enginerHealthImg.color = _health < _value ? ColorManager.Instance.Red : ColorManager.Instance.LightGreen;
        }


        [ContextMenu("SetObjective")]
        private void SetObjective_Debug() => SetObjective(Test_level);

        private void SetObjective(LevelDataSO.Level _level)
        {
            // Objective
            objectiveSeparator.SetActive(_level.objectives.Count > 1);
            if (_level.TryGetObjective(Enums.Objective.DeliverOrders, out int _value))
            {
                deliverValueTMP.text = _value.ToString();
                deliverParent.SetActive(true);
            }
            else deliverParent.SetActive(false);

            if (_level.TryGetObjective(Enums.Objective.CollectGreenEmoji, out _value))
            {
                emojiValueTMP.text = _value.ToString();
                emojiIcon.sprite = SpriteManager.Instance.greenEmoji;
                emojiParent.SetActive(true);
            }
            else if (_level.TryGetObjective(Enums.Objective.CollectYellowEmoji, out _value))
            {
                emojiValueTMP.text = _value.ToString();
                emojiIcon.sprite = SpriteManager.Instance.yellowEmoji;
                emojiParent.SetActive(true);
            }
            else if (_level.TryGetObjective(Enums.Objective.CollectRedEmoji, out _value))
            {
                emojiValueTMP.text = _value.ToString();
                emojiIcon.sprite = SpriteManager.Instance.redEmoji;
                emojiParent.SetActive(true);
            }
            else emojiParent.SetActive(false);


            // Condition
            if (_level.conditions.Count > 0)
            {
                if (_level.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out _value))
                {
                    healthValueTMP.text = $">  {_value}%";
                    healthParent.SetActive(true);

                }
                else healthParent.SetActive(false);

                if (_level.TryGetCondition(Enums.ObjectiveCondition.InTime, out _value))
                {
                    timeValueTMP.text = $"IN {HelperFunctions.ToTimerString(_value)}";
                    timeParent.SetActive(true);
                }
                else timeParent.SetActive(false);
            }

            levelConditionsParent.SetActive(_level.conditions.Count > 0);
        }


        public async void OnClick_Play()
        {
            if (GameController.Instance.LevelInfo.LevelBooster != Enums.LevelBoosterType.None)
            {
                if (GameData.Instance.PlayerData.Data.chips < (GameController.Instance.LevelInfo.CurrentLevel.levelBoosterCost * GameData.Instance.SelectedLevelNumber))
                {
                    OverlayWarningPopup.Instance.ShowWarning("Insufficient Chips!!!");
                    return;
                }

                int _cost = -1 * GameController.Instance.LevelInfo.CurrentLevel.levelBoosterCost * GameData.Instance.SelectedLevelNumber;
                var _success = await PlayerAPIs.UpdatePlayerDataChips_API(_cost);
                if (_success)
                {
                    if (GameController.Instance.LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out int _value))
                    {
                        int _health = GetHealthStatus();

                        if (GameController.Instance.LevelInfo.LevelBooster == Enums.LevelBoosterType.Shield) _health += 25;

                        if (_health < _value)
                        {
                            OverlayWarningPopup.Instance.ShowWarning("Not enough engine health!");
                            Debug.Log("Player health is " + _health);
                            return;
                        }
                    }


                    MenuBar.Instance.Hide();
                    BoosterPrefab.OnBoosterAdd -= OnBoosterAdd;
                    BoosterPrefab.OnBoosterRemove -= OnBoosterRemove;
                    Hide(() =>
                    {
                        GameplayScreen.Instance.SetInitialBooster(GameController.Instance.LevelInfo.LevelBooster);
                        ScreenManager.Instance.SwitchScreen(GameplayScreen.Instance, null);
                    });
                    Debug.Log("Points Deducted -> " + (GameController.Instance.LevelInfo.CurrentLevel.levelBoosterCost * GameData.Instance.SelectedLevelNumber));
                }
            }
            else
            {
                if (GameController.Instance.LevelInfo.CurrentLevel.TryGetCondition(Enums.ObjectiveCondition.MinHealth, out int _value))
                {
                    int _health = GetHealthStatus();

                    if (GameController.Instance.LevelInfo.LevelBooster == Enums.LevelBoosterType.Shield) _health += 25;

                    if (_health < _value)
                    {
                        OverlayWarningPopup.Instance.ShowWarning("Not enough engine health!");
                        Debug.Log("Player health is " + _health);
                        return;
                    }
                }


                MenuBar.Instance.Hide();
                BoosterPrefab.OnBoosterAdd -= OnBoosterAdd;
                BoosterPrefab.OnBoosterRemove -= OnBoosterRemove;
                Hide(() =>
                {
                    GameplayScreen.Instance.SetInitialBooster(Enums.LevelBoosterType.None);
                    ScreenManager.Instance.SwitchScreen(GameplayScreen.Instance, null);
                });
            }
        }



        private int GetHealthStatus()
        {
            int _shieldLevel = GameData.Instance.PlayerData.Data.GetCurrentVehicleLevels().shieldLevel;
            return _shieldLevel == 0 ? GameData.Instance.GameSettings.defaultEngineHealth : GameData.Instance.GameSettings.defaultEngineHealth + vehicleData.shieldUpgrades[_shieldLevel - 1];

        }


        private void ResetInitialBooster()
        {
            boosterLeft.ResetButton();
            boosterMiddle.ResetButton();
            boosterRight.ResetButton();
        }

    }
}
