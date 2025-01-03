using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using VoxelBusters.CoreLibrary.Parser;

namespace FoodFury
{
    public class APIManager
    {
        private static string BaseTestUrl => "https://testapi.gobbl.io";
        private static string BaseLiveUrl => "https://api.gobbl.io";

        private static string FilteredDishesURL => "https://api.gobbl.io/api/dishes/filteredDishes/";
        private static string AWSBaseURL => "https://gobbl-bucket.s3.ap-south-1.amazonaws.com/";

        private static string FlagURL => AWSBaseURL + "tgFlags";
        private static string DishThumbnailURL => AWSBaseURL + "tgDishes";
        private static string MapThumbnailURL => AWSBaseURL + "FoodFuryThumbnails/Maps";
        private static string MapIconURL => AWSBaseURL + "FoodFuryThumbnails/Maps/Icons";
        private static string MusicURL => AWSBaseURL + "FoodFuryMusic";
        private static string VehicleThumbnailURL => AWSBaseURL + "FoodFuryThumbnails/Vehicles";

        private static string getGeneralSettings => "/api/foodfury/getGeneralSettings";
        private static string getCalculations => "/api/foodfury/getCalculations";
        private static string getMaintenance => "/api/foodfury/getMaintainence/";
        private static string getLeaderboardData => "/api/foodfive/getGameLeaderBoard";

        // -----------------------------------------------------------Player Data
        private static string getPlayerData => "/api/foodfury/getPlayerData";
        private static string updatePlayerData => "/api/foodfury/updatePlayerData";
        private static string updatePlayerDataLevel => "/api/foodfury/updatePlayerLevel";
        private static string setLicenseCompleted => "/api/foodfury/setLicenseCompleted";
        private static string saveDriverDetails => "/api/foodfury/saveDriverDetails";
        private static string saveLevelDetails => "/api/foodfury/saveLevelDetails"; // For Analytics
        private static string updateUserName = "/api/user/updateUsername";


        // -----------------------------------------------------------Garage & Vehicle Data
        private static string getAllVehicles => "/api/foodfury/getAllVehicles";
        private static string purchaseVehicle => "/api/foodfury/purchaseVehicle";
        private static string updateVehicleData => "/api/foodfury/updateVehicleData";
        private static string unlockVehicleUsingNFT => "/api/foodfury/unlockBikeUsingNFT";


        // -----------------------------------------------------------Fuel
        private static string getFuelPurchaseData => "/api/foodfury/getFuelPurchaseData";
        private static string purchaseFuel => "/api/foodfury/purchaseFuel";
        private static string restoreFuel => "/api/foodfury/restoreFuel";


        // -----------------------------------------------------------Levels
        private static string getLatestLevels => "/api/foodfury/getLatestLevels";
        private static string getPlayerLevels => "/api/foodfury/getPlayerLevels";


        // -----------------------------------------------------------Others
        private static string getMapDetails => "/api/foodfury/getMapDetails";
        private static string getUserDishes => "/api/user/getUserDishes";
        private static string upgradeDish => "/api/user/upgradeDish";


        // -----------------------------------------------------------Login
        private static string signUpPlayer => "/api/foodfury/signUpPlayer";



        // -----------------------------------------------------------Chest
        private static string getUserChests => "/api/chest/getUserChests";
        private static string buyChest => "/api/chest/buyChest";
        private static string unlockChest => "/api/chest/unlockChest";
        private static string unlockChestWithChips => "/api/chest/unlockChestWithChips";

        // -----------------------------------------------------------Referal
        private static string getReferalData => "/api/foodfury/getPlayerReferralData";
        private static string claimReferal => "/api/foodfury/setReferralTaskCompleted";

        // -----------------------------------------------------------Telegram Verification
        private static string verifyCode = "/api/foodfury/verifyCode";

        private static string GetBaseUrl() => GameData.Instance.serverMode == Enums.ServerMode.TestNet ? BaseTestUrl : BaseLiveUrl;

        public enum AWSUrl { Dish, Vehicle, MapThumbnail, MapIcon, Flag }
        private static bool showDebug = false;


        #region ---------------------------------------------------------------------------------------------- Others
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetGeneralSettingsAsync()
        {
            string _url = GetBaseUrl() + getGeneralSettings;
            return await GetRequestRawAsync(_url, null);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetCalculationDataAsync()
        {
            string _url = GetBaseUrl() + getCalculations;
            return await GetRequestRawAsync(_url, null);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetMaintenanceDataAsync()
        {
            string _url = GetBaseUrl() + getMaintenance;
            return await GetRequestRawAsync(_url, null);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> SetLicenseCompletedAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + setLicenseCompleted;
            return await PostRequestRawAsync(_url, _jsonObject);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> SaveAnalyticsDetailsAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + saveLevelDetails;
            return await PostRequestRawAsync(_url, _jsonObject);
        }
        #endregion



        #region ---------------------------------------------------------------------------------------------- Player Data
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetPlayerDataAsync(string _playerId)
        {
            string _param = $"userId={_playerId}";
            string _url = GetBaseUrl() + getPlayerData + $"?{_param}";
            return await GetRequestRawAsync(_url, _param);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> SaveDriverDetailsAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + saveDriverDetails;
            return await PostRequestRawAsync(_url, _jsonObject);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> UpdatePlayerDataAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + updatePlayerData;
            return await PostRequestRawAsync(_url, _jsonObject);
        }
        public static async Task<ModelClass.ErrorAndResultResponse<string>> VerifyCodeAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + verifyCode;
            return await PostRequestRawAsync(_url, _jsonObject);
        }
        public static async Task<ModelClass.ErrorAndResultResponse<string>> UpdatePlayerDataLevelAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + updatePlayerDataLevel;
            return await PostRequestRawAsync(_url, _jsonObject);
        }
        //public static async Task<ModelClass.ErrorAndResultResponse<string>> UpdateUserNameAsync(JObject _jsonObject)
        //{
        //    string _url = GetBaseUrl() + updateUserName;
        //    return await PostRequestRawAsync(_url, _jsonObject);
        //}

        #endregion



        #region ---------------------------------------------------------------------------------------------- Vehicle
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetGarageDataAsync()
        {
            string _url = GetBaseUrl() + getAllVehicles;
            return await GetRequestRawAsync(_url, null);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> UpdateVehicleDataAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + updateVehicleData;
            return await PostRequestRawAsync(_url, _jsonObject);
        }


        public static async Task<ModelClass.ErrorAndResultResponse<string>> PurchaseVehicleAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + purchaseVehicle;
            return await PostRequestRawAsync(_url, _jsonObject);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> UnlockVehicleUsingNFTAsync(string _userId, string _nftName)
        {
            string _param = $"userId={_userId}&nftName={_nftName}";
            string _url = GetBaseUrl() + unlockVehicleUsingNFT + $"?{_param}";
            return await GetRequestRawAsync(_url, _param);
        }
        #endregion



        #region ---------------------------------------------------------------------------------------------- Fuel
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetFuelPurchaseDataAsync()
        {
            string _url = GetBaseUrl() + getFuelPurchaseData;
            return await GetRequestRawAsync(_url, null);
        }


        public static IEnumerator PurchaseFuel(JObject _jsonObject, Action<bool, string> _callback)
        {
            string _url = GetBaseUrl() + purchaseFuel;
            if (showDebug) Debug.Log("PurchaseFuel -> " + _url);
            EncryptionResult encryptionResult = EncryptAndBuildFormData(_jsonObject);
            using (UnityWebRequest req = UnityWebRequest.Post(_url, encryptionResult.FormData))
            {
                HmacGenerator.AddHmacAndTimestampToRequest(encryptionResult.JsonData, req);
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    _callback(false, req.error);
                else
                    _callback(true, req.downloadHandler.text);

                req.Dispose();
            }
        }

        public static IEnumerator RestoreFuel(JObject _jsonObject, Action<bool, string> _callback)
        {
            string _url = GetBaseUrl() + restoreFuel;
            if (showDebug) Debug.Log("RestoreFuel -> " + _url);
            EncryptionResult encryptionResult = EncryptAndBuildFormData(_jsonObject);
            using (UnityWebRequest req = UnityWebRequest.Post(_url, encryptionResult.FormData))
            {
                HmacGenerator.AddHmacAndTimestampToRequest(encryptionResult.JsonData, req);
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    _callback(false, req.error);
                else
                    _callback(true, req.downloadHandler.text);

                req.Dispose();
            }
        }
        #endregion



        #region ---------------------------------------------------------------------------------------------- Leaderboard
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetLeaderboardDataAsync(string userId)
        {
            string _param = $"userId={userId}";
            string _url = GetBaseUrl() + getLeaderboardData + $"?{_param}";
            return await GetRequestRawAsync(_url, _param);
        }
        #endregion



        #region ---------------------------------------------------------------------------------------------- Levels
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetMapsDetailsAsync()
        {
            string _url = GetBaseUrl() + getMapDetails;
            return await GetRequestRawAsync(_url, null);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetLatestLevelsAsync()
        {
            string _url = GetBaseUrl() + getLatestLevels;
            return await GetRequestRawAsync(_url, null);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetPlayerLevelsAsync(string _userId)
        {
            string _param = $"userId={_userId}";
            string _url = GetBaseUrl() + getPlayerLevels + $"?{_param}";
            return await GetRequestRawAsync(_url, _param);
        }
        #endregion



        #region ---------------------------------------------------------------------------------------------- Dish
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetUserDishesAsync(string _userId)
        {
            string _param = $"userId={_userId}";
            string _url = GetBaseUrl() + getUserDishes + $"?{_param}";
            return await GetRequestRawAsync(_url, _param);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> UpgradeDishAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + upgradeDish;
            return await PostRequestRawAsync(_url, _jsonObject);
        }

        public static IEnumerator GetFilteredDishes(string _dataString, Action<bool, string> _callback)
        {
            if (showDebug) Debug.Log("GetFilteredDishes -> " + FilteredDishesURL);
            using (UnityWebRequest req = new UnityWebRequest(FilteredDishesURL, "POST"))
            {
                req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(_dataString));
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success)
                    _callback(false, req.error);
                else
                    _callback(true, req.downloadHandler.text);

                req.uploadHandler.Dispose();
                req.downloadHandler.Dispose();
                req.Dispose();
            }
        }
        #endregion




        #region ---------------------------------------------------------------------------------------------- Chest
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetUserChestsAsync(string _userId)
        {
            string _param = $"userId={_userId}";
            string _url = GetBaseUrl() + getUserChests + $"?{_param}";
            return await GetRequestRawAsync(_url, _param);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> BuyChestAsync(JObject _jsonObject)
        {
            // userId, chestType
            string _url = GetBaseUrl() + buyChest;
            return await PostRequestRawAsync(_url, _jsonObject);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> UnlockChestAsync(JObject _jsonObject)
        {
            // userId, chestType
            string _url = GetBaseUrl() + unlockChest;
            return await PostRequestRawAsync(_url, _jsonObject);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> UnlockChestWithChipsAsync(JObject _jsonObject)
        {
            // userId, chestType
            string _url = GetBaseUrl() + unlockChestWithChips;
            return await PostRequestRawAsync(_url, _jsonObject);
        }
        #endregion
        #region ---------------------------------------------------------------------------------------------- Referal
        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetReferalDataAsync(string _userId)
        {
            string _param = $"userId={_userId}";
            string _url = GetBaseUrl() + getReferalData + $"?{_param}";
            return await GetRequestRawAsync(_url, _param);
        }
        public static async Task<ModelClass.ErrorAndResultResponse<string>> ClaimReferalAsync(JObject _jsonObject)
        {
            string _url = GetBaseUrl() + claimReferal;
            return await PostRequestRawAsync(_url, _jsonObject);
        }
        #endregion


        #region ---------------------------------------------------------------------------------------------- Textures & Track
        public static async Task<Texture2D> GetTextureAsync(string _key, AWSUrl _url)
        {
            string _finalUrl = _url switch
            {
                AWSUrl.Dish => $"{DishThumbnailURL}/{_key}.png",
                AWSUrl.Vehicle => $"{VehicleThumbnailURL}/{_key}.png",
                AWSUrl.MapThumbnail => $"{MapThumbnailURL}/{_key}.png",
                AWSUrl.MapIcon => $"{MapIconURL}/{_key}.png",
                AWSUrl.Flag => $"{FlagURL}/{_key}.png",
                _ => ""
            };

            UnityWebRequest _req = UnityWebRequestTexture.GetTexture(_finalUrl);
            var _operation = _req.SendWebRequest();

            while (!_operation.isDone)
                await Task.Yield();

            Texture2D _tex = null;
            if (_req.result == UnityWebRequest.Result.Success)
                _tex = DownloadHandlerTexture.GetContent(_req);

            _req.Dispose();
            return _tex;
        }

        public static IEnumerator GetDishTexture(int _dishTokenId, Action<bool, Texture2D> _callback)
        {
            string _url = $"{DishThumbnailURL}/{_dishTokenId}.png";
            Debug.Log("GetDishTexture -> " + _url);
            if (showDebug) Debug.Log("GetDishTexture -> " + _url);
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(_url))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success) _callback(false, null);
                else _callback(true, DownloadHandlerTexture.GetContent(req));

                req.Dispose();
            }
        }


        public static IEnumerator GetMapTexture(string _key, bool _isMapIcon, Action<bool, Texture2D> _callback)
        {
            string _url = _isMapIcon ? $"{MapIconURL}/{_key}" : $"{MapThumbnailURL}/{_key}";
            if (showDebug) Debug.Log("GetMapTexture -> " + _url);
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(_url))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success) _callback(false, null);
                else _callback(true, DownloadHandlerTexture.GetContent(req));

                req.Dispose();
            }
        }

        public static IEnumerator GetVehicleTexture(string _key, Action<bool, Texture2D> _callback)
        {
            string _url = $"{VehicleThumbnailURL}/{_key}";
            if (showDebug) Debug.Log("GetVehicleTexture -> " + _url);
            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(_url))
            {
                yield return req.SendWebRequest();

                if (req.result != UnityWebRequest.Result.Success) _callback(false, null);
                else _callback(true, DownloadHandlerTexture.GetContent(req));

                req.Dispose();
            }
        }

        public static IEnumerator GetTexture(string _url, Action<Texture2D> _callback)
        {
            if (showDebug) Debug.Log("GetTexture -> " + _url);
            if (string.IsNullOrWhiteSpace(_url))
            {
                _callback?.Invoke(null);
                yield break;
            }

            using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(_url))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success) _callback(null);
                else _callback(DownloadHandlerTexture.GetContent(req));
                //_callback(true, ((DownloadHandlerTexture)req.downloadHandler).texture);

                req.Dispose();
            }
        }

        public static async Task<ModelClass.ErrorAndResultResponse<AudioClip>> DownloadTrack(string _key)
        {
            string _url = $"{MusicURL}/{_key}";
            UnityWebRequest _req = UnityWebRequestMultimedia.GetAudioClip(_url, _key.ToAudioType());
            var _operation = _req.SendWebRequest();

            while (!_operation.isDone)
                await Task.Yield();

            ModelClass.ErrorAndResultResponse<AudioClip> _response = new();
            _response.error = _req.result != UnityWebRequest.Result.Success;
            _response.result = _response.error ? null : DownloadHandlerAudioClip.GetContent(_req);

            _req.Dispose();
            return _response;
        }

        public static IEnumerator DownloadTrack(string _key, Action<AudioClip> _callback)
        {
            string _url = $"{MusicURL}/{_key}";
            if (showDebug) Debug.Log("DownloadTrack -> " + _url);
            using (UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(_url, _key.ToAudioType()))
            {
                yield return req.SendWebRequest();
                if (req.result != UnityWebRequest.Result.Success) _callback(null);
                else _callback(DownloadHandlerAudioClip.GetContent(req));
                UnityWebRequest _req = UnityWebRequestMultimedia.GetAudioClip(_url, _key.ToAudioType());
                var _operation = _req.SendWebRequest();

                req.Dispose();
            }
        }

        #endregion



        #region ---------------------------------------------------------------------------------------------- Login
        public static async Task<ModelClass.ErrorAndResultResponse<string>> POST_SignUpPlayer(JObject _jsonObject)
        {

            string _url = GetBaseUrl() + signUpPlayer;
            return await PostRequestRawAsync(_url, _jsonObject);
            // Use the EncryptAndBuildFormData function
            //Debug.Log(GetBaseUrl() + signUpPlayer);
            //EncryptionResult encryptionResult = EncryptAndBuildFormDataWithDataString(_jsonObject);
            //Debug.Log(_jsonObject);
            //using (UnityWebRequest www = UnityWebRequest.Post(GetBaseUrl() + signUpPlayer, encryptionResult.FormData))
            //{
            //    HmacGenerator.AddHmacAndTimestampToRequest(encryptionResult.JsonData, www);
            //    yield return www.SendWebRequest();

            //    if (www.result != UnityWebRequest.Result.Success)
            //    {
            //        Debug.Log(www.downloadHandler.text);
            //    }
            //    else
            //    {
            //        var _result = JObject.Parse(www.downloadHandler.text);
            //        string _userId = _result["result"].ToString();
            //        callback(_userId);
            //    }
            //}
        }
        #endregion





        #region Encryption
        private static EncryptionResult EncryptAndBuildFormData(JObject jObject, string _formKey = "data")
        {
            string jsString = AESEncryptionECB.Encrypt(JsonConvert.SerializeObject(jObject));

            jsString = jsString.Replace("\"", "");
            WWWForm form = new WWWForm();
            form.AddField(_formKey, jsString);
            //Debug.Log(_formKey);
            //Debug.Log(jsString);
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { _formKey, jsString }
            };

            string jsonData = JsonConvert.SerializeObject(parameters);

            // Creating an instance of the data class and setting values
            EncryptionResult result = new EncryptionResult
            {
                JsonData = jsonData,
                FormData = form
            };

            // Now the result is available for further use
            return result;
        }
        private static EncryptionResult EncryptAndBuildFormDataWithDataString(JObject jObject)
        {
            string jsString = AESEncryptionECB.Encrypt(JsonConvert.SerializeObject(jObject));
            // Convert the encrypted string to bytes

            WWWForm form = new WWWForm();
            form.AddField("data", jsString);

            //Debug.Log(jsString);

            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "data", jsString }
            };

            string jsonData = JsonConvert.SerializeObject(parameters);

            // Creating an instance of the data class and setting values
            EncryptionResult result = new EncryptionResult
            {
                JsonData = jsonData,
                FormData = form
            };

            // Now the result is available for further use
            return result;
        }
        public class EncryptionResult
        {
            public string JsonData { get; set; }
            public WWWForm FormData { get; set; }
        }
        #endregion






        public static async Task<ModelClass.ErrorAndResultResponse<string>> GetRequestRawAsync(string _url, string _params)
        {
            //Debug.Log("GetRequestRawAsync -> " + _url);

            UnityWebRequest _req = UnityWebRequest.Get(_url);
            HmacGenerator.AddHmacAndTimestampToRequest(_params, _req);
            var _operation = _req.SendWebRequest();
            while (!_operation.isDone) await Task.Yield();

            ModelClass.ErrorAndResultResponse<string> _response = new();
            _response.error = _req.result != UnityWebRequest.Result.Success;
            _response.result = _response.error ? _req.error : _req.downloadHandler.text;

            _req.Dispose();
            return _response;
        }

        public static async Task<ModelClass.ErrorAndResultResponse<string>> PostRequestRawAsync(string _url, JObject _jsonObject)
        {
            // Debug.Log("PostRequestRawAsync -> " + _url);
            // Debug.Log(_jsonObject.ToJson());
            EncryptionResult encryptionResult = EncryptAndBuildFormData(_jsonObject);
            UnityWebRequest req = UnityWebRequest.Post(_url, encryptionResult.FormData);
            HmacGenerator.AddHmacAndTimestampToRequest(encryptionResult.JsonData, req);
            var _operation = req.SendWebRequest();
            while (!_operation.isDone)
            {
                await Task.Yield();
            }
            ModelClass.ErrorAndResultResponse<string> _response = new();
            _response.error = req.result != UnityWebRequest.Result.Success;
            _response.result = _response.error ? req.error : req.downloadHandler.text;
            // Debug.Log($"_response result {_response.result}");
            // Debug.Log($"_response result {_response}");
            req.Dispose();
            return _response;
        }





        public async Task<ModelClass.ErrorAndResultResponse<T>> GetRequestAsync<T>(string _url)
        {
            // Debug.Log("GetRequest -> " + _url);

            ModelClass.ErrorAndResultResponse<T> _response = new();
            _response.result = default;

            using (UnityWebRequest req = UnityWebRequest.Get(_url))
            {
                HmacGenerator.AddHmacAndTimestampToRequest(null, req);
                var _operation = req.SendWebRequest();
                while (!_operation.isDone) await Task.Yield();

                bool _error = req.result != UnityWebRequest.Result.Success;
                if (!_error)
                {
                    _response = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<T>>(req.downloadHandler.text);
                    if (_response.error) _response.result = default;
                }

                req.Dispose();
            }
            return _response;
        }



    }
}



