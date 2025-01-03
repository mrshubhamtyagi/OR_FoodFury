using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using static FoodFury.ModelClass;
using static Unity.VisualScripting.Member;

namespace FoodFury
{
    public class AnalyticsManager : MonoBehaviour
    {
        #region Variables
        public static AnalyticsManager Instance { get; private set; }
        public Enums.EnvironmentType environmentType;
        [Header("Analytics Default Parameters ")]
        public static string EnvironmentString = "Environment";
        public static string GameModeString = "GameMode";
        public static string ReleaseModeString = "ReleaseMode";
        public static string ServerString = "Server";
        public static string TimeString = "Time";
        public static string PlayerIdString = "PlayerId";
        public static string TelegramhandleString = "TelegramHandle";
        [Header("Default Error/Warning/invalid Parameters ")]
        public static string ErrorMessageString = "error:";
        public static string WarningMessageString = "warning:";
        public static string InvalidMessageString = "UNKNOWN";

        [Header("Login Parameters ")]
        public static string LoginMethodUsedString = "LoginMethod";
        public static string AutoLoginString = "IsAutoLoggedIn";
        public static string LoginEventString = "LoginEvent";
        public static string LogoutEventString = "LogoutEvent";

        [Header("Screen Change Parameters ")]
        public static string ScreenChangeFromString = "ScreenChangingFrom";
        public static string ScreenChangeToString = "ScreenChangingTo";
        public static string ScreenChangeEventString = "ScreenChangeEvent";

        [Header("Vehicle Purchase Events Parameters ")]
        public static string VehicleIdString = "VehicleId";
        public static string VehicleNameString = "VehicleName";
        public static string NftBikeString = "NftBike";
        public static string NftBikeNameString = "NFTBikeName";
        public static string VehiclePurchaseSuccessfulEventString = "VehiclePurchaseSuccessfulEvent";

        [Header("Vehicle Upgrade Events Parameters ")]
        public static string VehicleUpgradeSuccessfulEventString = "VehicleUpgradeSuccessFullyEvent";
        public static string VehicleUpgradeLevelString = "VehicleUpgradeLevel";
        public static string VehicleUpgradeTypeString = "VehicleUpgradeType";

        [Header("Vehicle Switch Events Parameters ")]
        public static string VehicleSwitchFromIdString = "VehicleSwitchFromId";
        public static string VehicleSwitchFromNameString = "VehicleSwitchFromName";
        public static string VehicleSwitchToIdString = "VehicleSwitchToName";
        public static string VehicleSwitchToNameString = "VehicleSwitchToName";
        public static string VehicleSwitchedSuccessfulEventString = "VehicleSwitchedSuccessfullyEvent";

        [Header("Fuel Purchase Events Parameters ")]
        public static string FuelPurchaseIdString = "FuelPurchaseId";
        public static string FuelBeforePurchaseString = "FuelBeforePurchase";
        public static string FuelAfterPurchaseString = "FuelAfterPurchase";
        public static string FuelPurchaseCostTypeString = "FuelPurchaseCostType";
        public static string FuelPurchaseSuccesfulEventString = "FuelPurchaseSuccesfulEvent";

        [Header("Level Complete and Failed Events Parameters ")]
        public static string LevelString = "GameLevel";
        public static string MapNameString = "MapName";
        public static string MunchesString = "Munches";
        public static string ChipsString = "Chips";
        public static string DeliveriesCompletedString = "DeliveriesCompleted";
        public static string NFTOrdersString = "NFTOrders";
        public static string HealthString = "PlayerHealth";
        public static string TotalTimeString = "TotalTimeTaken";
        public static string GreenEmojiString = "GreenEmoji";
        public static string YellowEmojiString = "YellowEmoji";
        public static string RedEmojiString = "RedEmoji";
        public static string RetryString = "Retry";
        public static string LevelCompleteEventString = "LevelCompleteEvent";
        public static string LevelFailedEventString = "LevelFailedEvent";

        [Header("Weapon Collected And Fired Events Parameters ")]
        public static string WeaponTypeString = "WeaponType";
        public static string WeaponCollectedEventString = "WeaponCollectedEvent";
        public static string WeaponFiredEventString = "WeaponFiredEvent";

        [Header("Booster Collected Events Parameters ")]
        public static string BoosterTypeString = "BoosterType";
        public static string BoosterDurationString = "BoosterDurationType";
        public static string BoosterCollectedEventString = "BoosterCollectedEvent";

        [Header("Order Related Events Parameters ")]
        public static string IsNFTOrderString = "IsNFTOrder";
        public static string OrderNameString = "OrderName";
        public static string OrderIdString = "DishId";
        public static string OrderLevelString = "DishLevel";
        public static string OrderFailedReasonString = "OrderFailedReason";
        public static string OrderMissedEventString = "OrderMissedEvent";
        public static string OrderDeliveredEventString = "OrderDelivered";

        [Header("Driving Test Related Events Parameters ")]
        public static string LessonSkippedEventString = "LessonSkippedEvent";
        public static string LessonStepString = "LessonStep";
        public static string LessonFailedEventString = "LessonFailedEvent";
        public static string LessonCompletedEventString = "LessonCompletedEvent";
        public static string LicenseRecievedEventString = "LicenseRecievedEvent";
        [Header("Game Start Related Events Parameters ")]

        public static string GameStartedEventString = "GameStarted";
        public static string RankString = "RankInString";
        public static string HandleString = "Handle";

        [Header("Currency Related Events Parameters ")]
        public static string ChipsEarnedEventString = "ChipsEarned";
        public static string MunchesEarnedEventString = "MunchEarned";
        public static string SourceString = "Source";

        [Header("Telegram Related Events Parameters ")]
        public static string TelegramConnectedEventString = "ConnectedToTelegram";

        [Header("Multiplayer Related Events Parameters")]
        public static string MultiplayerGameStartedEventSring= "MultiplayerMatchStartedEvent";
        public static string MultiplayerGameEndedEventString= "MultiplayerMatchEndedEvent";
        public static string PlayerDataString = "PlayersIds";
        public static string GameIdString = "GameId";
        public static string NumberOfBotsString = "NumberOfBots";
        public static string NumberOfPlayersString = "NumberOfPlayers";
        public static string PlayerWonIdString = "PlayerWonId";
        public static string PlayerLostIdsString = "PlayerLostIds";

        #endregion


        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void AddDefaultParametersToCustomEvent(CustomEvent customEvent)
        {
            customEvent.Add(EnvironmentString,HelperFunctions.ConvertEnumToCleanString(environmentType));
            customEvent.Add(GameModeString, GameData.Instance.gameMode.ToString());
            customEvent.Add(ReleaseModeString, GameData.Instance.releaseMode.ToString());
            customEvent.Add(ServerString, GameData.Instance.serverMode.ToString());
            customEvent.Add(TimeString, DateTime.Now.ToString());
            customEvent.Add(PlayerIdString, GameData.Instance.PlayerId.ToString());
            //  customEvent.Add(TelegramhandleString, string.IsNullOrEmpty(GameData.Instance.PlayerData.Data.gmailUserId) ? InvalidMessageString : GameData.Instance.PlayerData.Data.signupvia.handle);
        }
        public void FireGameStartedEvent(string handle, string via, string rank, int munches, int chips)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Game Started Event");
                return;
            }
            CustomEvent gameStartEvent = new CustomEvent(GameStartedEventString);
            AddDefaultParametersToCustomEvent(gameStartEvent);
            gameStartEvent.Add(HandleString, handle);
            gameStartEvent.Add(LoginMethodUsedString, via);
            gameStartEvent.Add(RankString, rank);
            gameStartEvent.Add(MunchesString, munches);
            gameStartEvent.Add(ChipsString, chips);
            AnalyticsService.Instance.RecordEvent(gameStartEvent);
        }
        #region Login Events
        public void FireLoginEvent(bool AutoLoggedIn)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Login Event");
                return;
            }
            CustomEvent loginEvent = new CustomEvent(LoginEventString);
            AddDefaultParametersToCustomEvent(loginEvent);
            loginEvent.Add(LoginMethodUsedString, PlayerPrefsManager.Instance.Via == "" ? InvalidMessageString : PlayerPrefsManager.Instance.Via);
            loginEvent.Add(AutoLoginString, AutoLoggedIn == true ? "true" : "false");

            AnalyticsService.Instance.RecordEvent(loginEvent);
        }
        public void FireLogoutEvent()
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Logout Event");
                return;
            }
            CustomEvent logout = new CustomEvent(LogoutEventString);
            AddDefaultParametersToCustomEvent(logout);
            AnalyticsService.Instance.RecordEvent(logout);
        }
        #endregion

        #region Screen Related Events
        public void FireScreenChangeEvent(string screenChangeFrom, string screenChangeTo)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Screen Change Event");
                return;
            }
            CustomEvent screenChange = new CustomEvent(ScreenChangeEventString);
            AddDefaultParametersToCustomEvent(screenChange);
            if (screenChangeFrom == "")
            {
                screenChange.Add(ScreenChangeFromString, InvalidMessageString);
            }
            else
            {
                screenChange.Add(ScreenChangeFromString, screenChangeFrom);

            }
            screenChange.Add(ScreenChangeToString, screenChangeTo);
            AnalyticsService.Instance.RecordEvent(screenChange);
        }
        #endregion

        #region Vehicle Related Events
        public void FireVehiclePurchasedSuccessfulEvent(int vehicleID, string vehicleName, bool isNftBike, string nftBikeName = "")
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Vehicle Purchased Successfully Event");
                return;
            }
            CustomEvent vehiclePurchaseSuccessfulEvent = new CustomEvent(VehiclePurchaseSuccessfulEventString);
            AddDefaultParametersToCustomEvent(vehiclePurchaseSuccessfulEvent);
            vehiclePurchaseSuccessfulEvent.Add(VehicleIdString, vehicleID);
            vehiclePurchaseSuccessfulEvent.Add(VehicleNameString, vehicleName);
            vehiclePurchaseSuccessfulEvent.Add(NftBikeString, isNftBike == true ? "true" : "false");
            if (nftBikeName == "")
            {
                vehiclePurchaseSuccessfulEvent.Add(NftBikeNameString, InvalidMessageString);
            }
            AnalyticsService.Instance.RecordEvent(vehiclePurchaseSuccessfulEvent);
        }
        public void FireVehicleUpgradeSuccessfulEvent(int vehicleID, string vehicleName, bool isNftBike, string nftBikeName, int upgradeLevel, string upgradeType)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Vehicle Upgrade Successfully Event");
                return;
            }
            CustomEvent vehicleUpgradeSuccessfulEvent = new CustomEvent(VehicleUpgradeSuccessfulEventString);
            AddDefaultParametersToCustomEvent(vehicleUpgradeSuccessfulEvent);
            vehicleUpgradeSuccessfulEvent.Add(VehicleIdString, vehicleID);
            vehicleUpgradeSuccessfulEvent.Add(VehicleNameString, vehicleName);
            vehicleUpgradeSuccessfulEvent.Add(NftBikeString, isNftBike == true ? "true" : "false");
            if (nftBikeName == "")
            {
                vehicleUpgradeSuccessfulEvent.Add(NftBikeNameString, InvalidMessageString);
            }
            AnalyticsService.Instance.RecordEvent(vehicleUpgradeSuccessfulEvent);
        }
        public void FireVehicleSwitchedSuccessfulEvent(int previousVehicleID, string previousVehicleName, int currentVehicleID, string currentVehicleName)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Vehicle Switch Event");
                return;
            }
            CustomEvent vehicleSwitchedSuccessfulEvent = new CustomEvent(VehicleSwitchedSuccessfulEventString);
            AddDefaultParametersToCustomEvent(vehicleSwitchedSuccessfulEvent);
            vehicleSwitchedSuccessfulEvent.Add(VehicleSwitchFromIdString, previousVehicleID);
            vehicleSwitchedSuccessfulEvent.Add(VehicleSwitchFromNameString, previousVehicleName);
            vehicleSwitchedSuccessfulEvent.Add(VehicleSwitchToIdString, currentVehicleID);
            vehicleSwitchedSuccessfulEvent.Add(VehicleSwitchToNameString, currentVehicleName);
            AnalyticsService.Instance.RecordEvent(vehicleSwitchedSuccessfulEvent);
        }
        public void FireFuelPurchasedSuccessfulEvent(int fuelPurchaseId, int fuelBeforePurchase, int fuelAfterPurchase, string fuelPurchaseCostType)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Fuel Purchased Event");
                return;
            }
            CustomEvent fuelPurchasedSuccessfulEvent = new CustomEvent(FuelPurchaseSuccesfulEventString);
            AddDefaultParametersToCustomEvent(fuelPurchasedSuccessfulEvent);
            fuelPurchasedSuccessfulEvent.Add(FuelPurchaseIdString, fuelPurchaseId);
            fuelPurchasedSuccessfulEvent.Add(FuelBeforePurchaseString, fuelBeforePurchase);
            fuelPurchasedSuccessfulEvent.Add(FuelAfterPurchaseString, fuelAfterPurchase);
            fuelPurchasedSuccessfulEvent.Add(FuelPurchaseCostTypeString, fuelPurchaseCostType);
            AnalyticsService.Instance.RecordEvent(fuelPurchasedSuccessfulEvent);
        }
        #endregion

        #region Level Related Events
        public void FireLevelCompleteEvent(int gameLevel, string mapName, int deliveriesCompleted, int munches, int chips, int NFTOrders, int playerHealth, long totalTimeTaken, int greenEmoji, int yellowEmoji, int redEmoji, bool isLevelPlayedBefore)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Level Complete Event");
                return;
            }
            CustomEvent levelCompleteEvent = new CustomEvent(LevelCompleteEventString);
            AddDefaultParametersToCustomEvent(levelCompleteEvent);
            levelCompleteEvent.Add(LevelString, gameLevel);
            levelCompleteEvent.Add(MapNameString, mapName);
            levelCompleteEvent.Add(DeliveriesCompletedString, deliveriesCompleted);
            levelCompleteEvent.Add(MunchesString, munches);
            levelCompleteEvent.Add(ChipsString, chips);
            levelCompleteEvent.Add(NFTOrdersString, NFTOrders);
            levelCompleteEvent.Add(HealthString, playerHealth);
            levelCompleteEvent.Add(TotalTimeString, totalTimeTaken);
            levelCompleteEvent.Add(GreenEmojiString, greenEmoji);
            levelCompleteEvent.Add(YellowEmojiString, yellowEmoji);
            levelCompleteEvent.Add(RedEmojiString, redEmoji);
            levelCompleteEvent.Add(RetryString, isLevelPlayedBefore ? "true" : "false");
            AnalyticsService.Instance.RecordEvent(levelCompleteEvent);
        }
        public void FireLevelFailedEvent(int gameLevel, string mapName, int deliveriesCompleted, int munches, int chips, int NFTOrders, int playerHealth, long totalTimeTaken, int greenEmoji, int yellowEmoji, int redEmoji, bool isLevelPlayedBefore)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Login Failed Event");
                return;
            }
            CustomEvent levelFailedEvent = new CustomEvent(LevelFailedEventString);
            AddDefaultParametersToCustomEvent(levelFailedEvent);
            levelFailedEvent.Add(LevelString, gameLevel);
            levelFailedEvent.Add(MapNameString, mapName);
            levelFailedEvent.Add(DeliveriesCompletedString, deliveriesCompleted);
            levelFailedEvent.Add(MunchesString, munches);
            levelFailedEvent.Add(ChipsString, chips);
            levelFailedEvent.Add(NFTOrdersString, NFTOrders);
            levelFailedEvent.Add(HealthString, playerHealth);
            levelFailedEvent.Add(TotalTimeString, totalTimeTaken);
            levelFailedEvent.Add(GreenEmojiString, greenEmoji);
            levelFailedEvent.Add(YellowEmojiString, yellowEmoji);
            levelFailedEvent.Add(RedEmojiString, redEmoji);
            levelFailedEvent.Add(RetryString, isLevelPlayedBefore ? "true" : "false");
            AnalyticsService.Instance.RecordEvent(levelFailedEvent);
        }
        #endregion

        #region Order Related Events
        public void FireOrderMissedEvent(int level, string mapName, bool isNFTOrder, string orderName, Enums.OrderFailedReasonType reason)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Order Missed Event");
                return;
            }
            CustomEvent orderMissedEvent = new CustomEvent(OrderMissedEventString);
            AddDefaultParametersToCustomEvent(orderMissedEvent);
            orderMissedEvent.Add(MapNameString, mapName);
            orderMissedEvent.Add(LevelString, level);
            orderMissedEvent.Add(IsNFTOrderString, isNFTOrder ? "true" : "false");
            orderMissedEvent.Add(OrderNameString, orderName);
            orderMissedEvent.Add(OrderFailedReasonString, reason.ToString());
            AnalyticsService.Instance.RecordEvent(orderMissedEvent);
        }
        public void FireOrderDeliveredEvent(int gameLevel, string mapName, bool isNFTOrder, string orderName, int dishTokenId, int dishlevel, int chipsEarned)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Order Delivered Event");
                return;
            }
            CustomEvent orderDeliveredEvent = new CustomEvent(OrderDeliveredEventString);
            AddDefaultParametersToCustomEvent(orderDeliveredEvent);
            orderDeliveredEvent.Add(MapNameString, mapName);
            orderDeliveredEvent.Add(LevelString, gameLevel);
            orderDeliveredEvent.Add(IsNFTOrderString, isNFTOrder ? "true" : "false");
            orderDeliveredEvent.Add(OrderNameString, orderName);
            orderDeliveredEvent.Add(OrderIdString, dishTokenId);
            orderDeliveredEvent.Add(OrderLevelString, dishlevel);

            AnalyticsService.Instance.RecordEvent(orderDeliveredEvent);
        }
        #endregion

        #region Weapon Related Events
        public void FireWeaponCollectedEvent(string weaponType)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Weapon Collected Event");
                return;
            }
            CustomEvent weaponCollectedEvent = new CustomEvent(WeaponCollectedEventString);
            AddDefaultParametersToCustomEvent(weaponCollectedEvent);
            weaponCollectedEvent.Add(WeaponTypeString, weaponType);
            AnalyticsService.Instance.RecordEvent(weaponCollectedEvent);
        }
        public void FireWeaponFiredEvent(string weaponType)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Weapon Fired Event");
                return;
            }
            CustomEvent weaponFiredEvent = new CustomEvent(WeaponFiredEventString);
            AddDefaultParametersToCustomEvent(weaponFiredEvent);
            weaponFiredEvent.Add(WeaponTypeString, weaponType);
            AnalyticsService.Instance.RecordEvent(weaponFiredEvent);
        }
        #endregion

        #region Booster Related Events
        public void FireBoosterCollectedEvent(string boosterType, float boosterDuration)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Booster Collected Event");
                return;
            }
            CustomEvent boosterCollectedEvent = new CustomEvent(BoosterCollectedEventString);
            AddDefaultParametersToCustomEvent(boosterCollectedEvent);
            boosterCollectedEvent.Add(BoosterTypeString, boosterType);
            boosterCollectedEvent.Add(BoosterDurationString, boosterDuration);
            AnalyticsService.Instance.RecordEvent(boosterCollectedEvent);
        }
        #endregion

        #region Driving Test Related Events
        public void FireLessonSkippedEvent(int level)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Lesson Skipped Event");
                return;
            }
            CustomEvent lessionSkippedEvent = new CustomEvent(LessonSkippedEventString);
            AddDefaultParametersToCustomEvent(lessionSkippedEvent);
            lessionSkippedEvent.Add(LessonStepString, level);
            AnalyticsService.Instance.RecordEvent(lessionSkippedEvent);
        }
        public void FireLessonFailedEvent(int level)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Lesson Failed Event");
                return;
            }
            CustomEvent lessionFailedEvent = new CustomEvent(LessonFailedEventString);
            AddDefaultParametersToCustomEvent(lessionFailedEvent);
            lessionFailedEvent.Add(LessonStepString, level);
            AnalyticsService.Instance.RecordEvent(lessionFailedEvent);
        }
        public void FireLessonCompletedEvent(int level)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Lesson Completed Event");
                return;
            }
            CustomEvent lessionCompletedEvent = new CustomEvent(LessonCompletedEventString);
            AddDefaultParametersToCustomEvent(lessionCompletedEvent);
            lessionCompletedEvent.Add(LessonStepString, level);
            AnalyticsService.Instance.RecordEvent(lessionCompletedEvent);
        }
        public void FireLicenseRecievedEvent()
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired License Recieved Event");
                return;
            }
            CustomEvent licenseRecievedEvent = new CustomEvent(LicenseRecievedEventString);
            AddDefaultParametersToCustomEvent(licenseRecievedEvent);
            AnalyticsService.Instance.RecordEvent(licenseRecievedEvent);
        }
        #endregion

        #region Currency Related
        public void FireChipsEarnedEvent(Enums.SourceType source, int chips)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Order Delivered Event");
                return;
            }
            Debug.Log($"Chips Earned via {Enum.GetName(typeof(Enums.SourceType), source)} : {chips}");
            CustomEvent chipsEarnedEvent = new CustomEvent(ChipsEarnedEventString);
            AddDefaultParametersToCustomEvent(chipsEarnedEvent);
            chipsEarnedEvent.Add(SourceString, Enum.GetName(typeof(Enums.SourceType), source));
            chipsEarnedEvent.Add(ChipsString, chips);
            AnalyticsService.Instance.RecordEvent(chipsEarnedEvent);
        }
        public void FireMunchesEarnedEvent(Enums.SourceType source, int munches)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Order Delivered Event");
                return;
            }
            Debug.Log($"Munches Earned via {Enum.GetName(typeof(Enums.SourceType), source)} : {munches}");
            CustomEvent munchesEarnedEvent = new CustomEvent(MunchesEarnedEventString);
            AddDefaultParametersToCustomEvent(munchesEarnedEvent);
            munchesEarnedEvent.Add(SourceString, Enum.GetName(typeof(Enums.SourceType), source));
            munchesEarnedEvent.Add(MunchesString, munches);
            AnalyticsService.Instance.RecordEvent(munchesEarnedEvent);
        }

        public void FireConnectedToTelegramEvent()
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Connected To Telegram Event");
                return;
            }
            CustomEvent ConnectToTelegramEvent = new CustomEvent(TelegramConnectedEventString);
            AddDefaultParametersToCustomEvent(ConnectToTelegramEvent);
            ConnectToTelegramEvent.Add(TelegramhandleString, GameData.Instance.PlayerData.Data.signupvia.handle);
            AnalyticsService.Instance.RecordEvent(ConnectToTelegramEvent);
        }
        #endregion

        #region MultiPlayer Related
        public void FireMultiPlayerLevelStartedEvent(string gameId,string[] playerIds,int bots)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Game Started MultiPlayer Event");
                return;
            }
            var arrayInJsonFormat=new JArray(playerIds);
            CustomEvent gameStartMultiplayerEvent = new CustomEvent(MultiplayerGameStartedEventSring);
            AddDefaultParametersToCustomEvent(gameStartMultiplayerEvent);     
            gameStartMultiplayerEvent.Add(GameIdString, gameId);
            gameStartMultiplayerEvent.Add(PlayerDataString, arrayInJsonFormat.ToString(Formatting.None));
            gameStartMultiplayerEvent.Add(NumberOfBotsString, bots);
            gameStartMultiplayerEvent.Add(NumberOfPlayersString, playerIds.Length);
            AnalyticsService.Instance.RecordEvent(gameStartMultiplayerEvent);
        }
        public void FireMultiPlayerLevelCompleteEvent(string gameId, string playerWonId, string[] playerLostIds)
        {
            if (environmentType == Enums.EnvironmentType.editor)
            {
                Debug.Log($"Fired Level Ended MultiPlayer Event");
                return;
            }
            var arrayInJsonFormat = new JArray(playerLostIds);
            CustomEvent multiplayerGameEndedEvent = new CustomEvent(MultiplayerGameEndedEventString);
            AddDefaultParametersToCustomEvent(multiplayerGameEndedEvent);
            multiplayerGameEndedEvent.Add(GameIdString, gameId);
            multiplayerGameEndedEvent.Add(PlayerWonIdString, playerWonId);
            multiplayerGameEndedEvent.Add(PlayerLostIdsString, arrayInJsonFormat.ToString(Formatting.None));
            AnalyticsService.Instance.RecordEvent(multiplayerGameEndedEvent);
        }
        #endregion


        #region Testing Area
        [ContextMenu("Fire MultiPlayer Started Debug")]
        public void DebugFireLevelStartMultiPlayer()
        {
            FireMultiPlayerLevelStartedEvent("12345646578974564564", new string[] { "23132154445465465", "4654654879823132156464" ,"7978979816489413156498789"}, 3);
        }
        [ContextMenu("Fire MultiPlayer Ended Debug")]
        public void DebugFireLevelEndMultiPlayer()
        {
            FireMultiPlayerLevelCompleteEvent("12345646578974564564", "7978979816489413156498789", new string[] { "23132154445465465", "4654654879823132156464" });
        }
        #endregion
    }

}

