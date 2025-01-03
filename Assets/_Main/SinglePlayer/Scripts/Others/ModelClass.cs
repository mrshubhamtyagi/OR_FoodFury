using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Audio;

namespace FoodFury
{
    public class ModelClass
    {
        [Serializable]
        public class Analytics
        {
            public int level;
            public int totalDeliveries;
            public int munches;
            public int chips;
            public long time;
            public float stars;
            public int nftOrders;
            public int greenEmojis;
            public int yellowEmojis;
            public int redEmojis;
            public List<int> orders;
        }


        [Serializable]
        public class PlayerData
        {
            public string _id;
            public string username;
            public int deliveries = 0;
            public int rank = 99;
            public float chips = 1000;
            public int munches = 0;
            public string profilePic;
            [HideInInspector] public float gameBalance = 0;
            public string gmailUserId;
            // FUEL
            public int fuel = 1800;
            public int fuelTankLevel = 0;
            public int fuelRestoreLevel = 0;
            public long fuelRestoreTimestamp;

            public bool isLicenseComplete;
            public bool isTelegramConnected;
            public SignUpVia signupvia;
            //public string ffCode;
            //public bool ffConnected;
            //public PlayerAirdropData airdropData;

            public int currentVehicle = 1;
            public List<VehicleLevels> vehicles;
            public List<PlayerLevel> playerLevel;
            public DriverDetails driverDetails = new();
            //public Referral referralCode = new();

            public VehicleLevels GetCurrentVehicleLevels() => vehicles.FirstOrDefault(v => v.id == currentVehicle);

            [Serializable]
            public class PlayerAirdropData
            {
                public bool hasClaimedIngredientLvl1;
                public bool hasClaimedIngredientLvl2;
                public bool hasClaimedIngredientLvl3;
                public bool hasCooked;
                public bool hasLeveledUp;
                public bool hasRented;
                public bool onboardingComplete;
            }


            [Serializable]
            public class VehicleLevels
            {
                public int id;
                public int speedLevel;
                public int mileageLevel;
                public int shieldLevel;
                public int armourLevel;
            }

            [Serializable]
            public class DriverDetails
            {
                public string name;
                public string country;
                public string dob; // seperated by .
                public string gender;
                public string favFood;
                public string validTill;
            }

            [Serializable]
            public class Referral
            {
                public string code;
                public int count;
            }

            [Serializable]
            public class PlayerLevel
            {
                public int mapId;
                public int levelNumber;

            }
        }

        [Serializable]
        public class CompletedLevels
        {
            public int mapId;
            public List<PlayerLevelStats> levels;
        }

        [Serializable]
        public class PlayerLevelStats
        {
            public int level;
            public int chips;
            public int munches;
            public float stars;
        }
        [Serializable]
        public class SignUpVia
        {
            public string via;
            public string handle;
        }

        [Serializable]
        public class GameSettings
        {
            public string GameVersion;
            public int defaultFuel;
            public int restoreFuelInterval = 5;

            [Header("---Default Health---")]
            public int defaultDamage = 10;
            public int defaultEngineHealth = 100;
            public int defaultArmourMultiplier = 5;
            public int defaultSubMissileDamage = 15;

            [Header("---Map---")]
            public int currentMapIdWebGL = 1;
            public int currentMapIdAndroid = 1;
            public int currentMapIdiOS = 1;
            public int levelSpeedBoosterValue = 3;
            public bool showMascot;

            [Header("---Chest---")]
            public int chestSnackCost = 100;
            public int chestTreatCost = 400;
            public int chestFeastCost = 1000;
            public float chestUnlockDiscount = 0.9f;
        }


        [Serializable]
        public class LeaderboardPlayer
        {
            public int rank;
            public string userId;
            public string username;
            public string profilePic;
            public string munch; // Munches
            public int dish; // Brownie
            public int chips; // Gourmet
            //public string total;
        }

        [Serializable]
        public class LeaderboardItem
        {
            public int rank;
            public string username;
            public int munches;
            public int deliveries;
            public int reward;
        }


        [Serializable]
        public class GarageVehicleData
        {
            public int id;
            public string name;
            public string tagline;
            public int cost;
            public Enums.CostType costType;
            public string thumbnailUrl;
            public Texture2D thumbnail;
            public string nftName;
            public Initial initial;
            public List<float> speedUpgrades;
            public List<int> shieldUpgrades;
            public List<float> mileageUpgrades;
            public List<int> armourUpgrades;
            public List<UpgradeCost> upgradesCost;

            //public int LowestSpeed;
            //public int HighestSpeed;

            public bool IsSpecialVehicle() => !string.IsNullOrWhiteSpace(nftName);


            [Serializable]
            public class Initial
            {
                public float speed;
                public int shield;
                public float mileage;
                public int armour;
                public int damage = 10;
            }

            [Serializable]
            public class UpgradeCost
            {
                public int cost;
                public Enums.CostType costType;
            }


            [Serializable]
            public class Upgrade
            {
                public int level;
                public float value;
                public UpgradeCost upgradeCost;
            }
        }



        [Serializable]
        public class VehicleUpgrade
        {
            public int id;
            public int level;
            public int cost;
            public Enums.CostType costType;
        }


        [Serializable]
        public class FuelPurchaseItem
        {
            public int id;
            public int fuel;
            public int cost;
            public Enums.CostType costType;
        }


        [Serializable]
        public class BoosterData
        {
            public Enums.BoosterType type;
            public int value;
            public int duration = 120;
            public float remaining = 0;
            public float delay = 0;
            [TextArea] public string description;

            public void Reset()
            {
                type = Enums.BoosterType.None;
                value = 0;
                duration = 0;
                remaining = 0;
                delay = 0;
            }
        }


        [Serializable]
        public class Dish
        {
            public string name;
            public int level;
            public int points;
            public int tokenId;
            public int baseId;      // Level 1 id

            [Space(20)]
            public int season;
            public string brand;
            public string country;
            public string region;
            public string continent;
            public string diet;
            public Enums.DishMethod method;
            public int skill_meter;
            public int spice_meter;
            public int time_meter;
            public int number_of_ingredients;

            [Space(20)]
            public int qtyForUpgrade;
            public int quantity;
            public int upgradeFee;
        }



        [Serializable]
        public class MapDetail
        {
            public string mapName;
            public string sceneName;
            public string mapDescription;
            public string thumbnailUrl;
            public int mapId;
            public string mapTrack;
            public bool isActive;
            public CameraSettings cameraSettings;
            public List<Dish> dishDetails = new List<Dish>();

            [Serializable]
            public class CameraSettings
            {
                public Offset offset;
                public float smoothTime;
                public int height;
                public int angleX;
                public int angleY;
            }

            [Serializable]
            public class Offset
            {
                public int x;
                public int y;
            }
        }




        [Serializable]
        public class ChestData
        {
            public string _id;
            public string userId;
            public Enums.ChestType type;
            public int price;
            public long unlockTime;
            public long creationTime;
            public string status;
            public ChestData dummyChest()
            {
                ChestData dummy = new ChestData();
                dummy._id = "dummy";
                dummy.userId = GameData.Instance.PlayerId;
                dummy.type = Enums.ChestType.SNACK;
                dummy.price = 100;
                dummy.unlockTime = new DateTimeOffset(DateTime.Now + new TimeSpan(0, 50, 50)).ToUnixTimeMilliseconds();
                dummy.creationTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                dummy.status = "dummy";
                return dummy;
            }
        }



        #region --------------------------------------------------------------------------------------------------- Level


        [Serializable]
        public class BoosterDetail
        {
            public Enums.BoosterType type;
            public List<int> values;
        }


        [Serializable]
        public class ObstacleDetail
        {
            public int timeout = 30;
            public List<Enums.ObstacleVarient> varients;
        }


        [Serializable]
        public class DishDetails
        {
            public Enums.DishRegion region;
            public Enums.DishContinent continent;
            public Enums.DishCountry country;
            public Enums.DishDiet diet;
            public Enums.DishMethod method;
            public Enums.DishBrand brand;
            public Enums.DishMeter meter;
            public Enums.MeterCondition meterCondition;
            [Range(0, 100)] public int meterValue;
        }



        [Serializable]
        public class LevelInfo
        {
            public Enums.LevelBoosterType LevelBooster = Enums.LevelBoosterType.None;
            public LevelDataSO.Level CurrentLevel;
            public int LevelTimer;

            [Header("-----Stats")]
            public int TotalDeliveries;
            public int NFTDeliveries;
            public int Chips;
            public int Munches;
            public int NFTScore;
            public int GreenEmojis;
            public int YellowEmojis;
            public int RedEmojis;
            public float TimeBonus;
            public float HealthBonus;
            public float StarValue = 0.33f;
            public long StartTime;
            public List<int> TotalOrdersList = new List<int>();
            public List<Dish> FilteredDishData = new List<Dish>();


            public void ResetStats()
            {
                LevelBooster = Enums.LevelBoosterType.None;
                TotalDeliveries = NFTDeliveries = Chips = Munches = NFTScore = GreenEmojis = YellowEmojis = RedEmojis = 0;
                TimeBonus = 0;
                StarValue = 0.33f;
                StartTime = LevelTimer = 0;
                TotalOrdersList.Clear();
            }

            public bool IsPrimaryObjectiveCompleted()
            {
                if (CurrentLevel.objectives.Count == 1)
                {
                    if (CurrentLevel.TryGetObjective(Enums.Objective.DeliverOrders, out int _value))
                        return TotalDeliveries >= _value;

                    if (CurrentLevel.TryGetObjective(Enums.Objective.CollectGreenEmoji, out _value))
                        return GreenEmojis >= _value;

                    if (CurrentLevel.TryGetObjective(Enums.Objective.CollectYellowEmoji, out _value))
                        return (GreenEmojis + YellowEmojis) >= _value;

                    if (CurrentLevel.TryGetObjective(Enums.Objective.CollectRedEmoji, out _value))
                        return (GreenEmojis + YellowEmojis + RedEmojis) >= _value;

                    return false;
                }
                else
                {
                    if (CurrentLevel.TryGetObjective(Enums.Objective.DeliverOrders, out int _value))
                        return TotalDeliveries >= _value;

                    return false;
                }
            }

            public bool IsSecondaryObjectiveCompleted()
            {
                if (CurrentLevel.objectives.Count == 1) return true;

                if (CurrentLevel.TryGetObjective(Enums.Objective.CollectGreenEmoji, out int _value))
                    return GreenEmojis >= _value;

                if (CurrentLevel.TryGetObjective(Enums.Objective.CollectYellowEmoji, out _value))
                    return (GreenEmojis + YellowEmojis) >= _value;

                if (CurrentLevel.TryGetObjective(Enums.Objective.CollectRedEmoji, out _value))
                    return (GreenEmojis + YellowEmojis + RedEmojis) >= _value;

                return false;
            }
        }

        #endregion


        [Serializable]
        public class CalculationData
        {
            public List<int> chipsPoints;
            //public List<int> NFTLevelMultiplier;
            public List<PercentageWithMultiplierData> HealthMultiplier;
            public List<PercentageWithMultiplierData> TimeMultiplier;
            public int NonNFTMunchValue = 2;
            public float LevelXValue;
            public float LevelYValue;

            [Serializable]
            public class PercentageWithMultiplierData
            {
                public float MinPercentage;
                public float MaxPercentage;
                public float Multiplier;
            }
        }
        [Serializable]
        public class AudioData
        {
            [Header("Audio")]
            public AudioClip AudioClip;
            public AudioMixerGroup AudioMixerGroup;
            [Header("Options")]
            public bool Loop;
            public float Volume;
            public float Pitch;
            public bool Is3DSound;
            public Vector2 MinMaxRangeOf3DEffect;
            [Header("Randomization Options")]
            public bool RandomizeVolume;
            //[MinMax(-1, 1)]
            public Vector2 RandomVolumeThreshold;
            public bool RandomizePitch;
            //[MinMax(-2, 2)]
            public Vector2 RandomPitchThreshold;
        }

        public class PlayerIdAndReferal
        {
            public string _id;
            public string referalUrl;
            public string userName;
        }


        #region --------------------------------------------------------------------------------------------------- Response Classes
        public class ErrorAndResultResponse<T>
        {
            public bool error = true;
            public string message;
            public T result = default;
        }

        [Serializable]
        public class MaintenanceResponse
        {
            public bool error;
            public Result result;

            [Serializable]
            public class Result
            {
                public string message;
                public bool webgl;
                public bool android;
                public bool ios;
            }
        }


        [Serializable]
        public class ChestBuyResponse
        {
            public bool error;
            public Result result;

            [Serializable]
            public class Result
            {
                public int munch;
                public Dish[] dishes;
            }


        }


        [Serializable]
        public class LeaderboardResponse
        {
            //public List<LeaderboardPlayer> BROWNIE;
            public LeaderboardPlayer[] result;

        }
        [Serializable]
        public class ReferralDataResponse
        {

            public bool error;
            public Result result = new Result();
            [Serializable]
            public class DATA
            {
                public int totalReferrals;
                public int referralPoints;
            }
            [Serializable]
            public class FinalTask
            {
                public int taskNumber;
                public string text;
                public int inviteLimit;
                public int reward;
                public bool isClaimed;
            }

            public class Result
            {
                public List<FinalTask> finalTasks = new List<FinalTask>();
                public DATA DATA;
            }
        }
        #endregion



    }


}