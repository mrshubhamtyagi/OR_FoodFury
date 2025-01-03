using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace FoodFury
{
    public static class OtherAPIs
    {

        public static async Task<ModelClass.ErrorAndResultResponse<GameSettingSO>> GetGeneralSettings_API()
        {
            var _response = await APIManager.GetGeneralSettingsAsync();
            //Debug.Log($"GetGeneralSettings_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<GameSettingSO>>(_response.result);
        }



        public static async Task<bool> SetLicenseCompleted_API()
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return true;

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };

            var _response = await APIManager.SetLicenseCompletedAsync(_params.ToJSONObject());
            //Debug.Log($"SetLicenseCompleted_API - Status {_response.error} | Response {_response.result}");

            if (_response.error) return false;
            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<bool>>(_response.result);
            return !_data.error && _data.result;
        }


        public static async Task<bool> SaveDriverDetails_API()
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return true;

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            if (!string.IsNullOrWhiteSpace(GameData.Instance.PlayerData.Data.driverDetails.name)) _params.Add("name", GameData.Instance.PlayerData.Data.driverDetails.name);
            if (!string.IsNullOrWhiteSpace(GameData.Instance.PlayerData.Data.driverDetails.country)) _params.Add("country", GameData.Instance.PlayerData.Data.driverDetails.country);
            if (!string.IsNullOrWhiteSpace(GameData.Instance.PlayerData.Data.driverDetails.dob)) _params.Add("dob", GameData.Instance.PlayerData.Data.driverDetails.dob);
            if (!string.IsNullOrWhiteSpace(GameData.Instance.PlayerData.Data.driverDetails.gender)) _params.Add("gender", GameData.Instance.PlayerData.Data.driverDetails.gender);
            if (!string.IsNullOrWhiteSpace(GameData.Instance.PlayerData.Data.driverDetails.favFood)) _params.Add("favFood", GameData.Instance.PlayerData.Data.driverDetails.favFood);
            if (!string.IsNullOrWhiteSpace(GameData.Instance.PlayerData.Data.driverDetails.validTill)) _params.Add("validTill", GameData.Instance.PlayerData.Data.driverDetails.validTill);
            //Debug.Log($"DriverDetails - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.SaveDriverDetailsAsync(_params.ToJSONObject());
            //Debug.Log($"SaveDriverDetails_API - Status {_response.error} | Response {_response.result}");
            if (_response.error) return false;

            ModelClass.ErrorAndResultResponse<bool> _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<bool>>(_response.result);
            return !_data.error && _data.result;
        }



        public static async Task<bool> SaveAnalytics_API(ModelClass.Analytics _analytics, bool _retry = false)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return true;

            int _munches = 0;
            int _chips = 0;
            float _stars = 0.33f;

            if (_retry)
            {
                ModelClass.PlayerLevelStats _level = GameData.Instance.GetCompletedLevelStats(_analytics.level - 1);
                _munches = _analytics.munches > _level.munches ? _analytics.munches - _level.munches : 0;
                _chips = _analytics.chips > _level.chips ? _analytics.chips - _level.chips : 0;
                _stars = _analytics.stars > _level.stars ? _analytics.stars - _level.stars : 0;
            }
            else
            {
                _munches = _analytics.munches;
                _chips = _analytics.chips;
                _stars = _analytics.stars;
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("level", _analytics.level);
            _params.Add("munches", _munches);     // newvalue>oldvalue?difference:0
            _params.Add("chips", _chips);         // newvalue>oldvalue?difference:0
            _params.Add("stars", _stars);         // newvalue>oldvalue?difference:0
            _params.Add("time", _analytics.time);
            _params.Add("nftOrders", _analytics.nftOrders);
            _params.Add("totalDeliveries", _analytics.totalDeliveries);
            _params.Add("orders", _analytics.orders);
            _params.Add("greenEmojis", _analytics.greenEmojis);
            _params.Add("yellowEmojis", _analytics.yellowEmojis);
            _params.Add("redEmojis", _analytics.redEmojis);
            _params.Add("mapId", GameData.Instance.GetSelectedMapData().mapId);
            _params.Add("mapName", GameData.Instance.GetSelectedMapData().mapName);
            _params.Add("gameVersion", Application.version);
            _params.Add("retry", _retry);
            //Debug.Log($"Analytics - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.SaveAnalyticsDetailsAsync(_params.ToJSONObject());
            //Debug.Log($"SaveDriverDetails_API - Status {_response.error} | Response {_response.result}");
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<int>>(_response.result);
            GameData.Instance.PlayerData.Data.rank = _data.result;
            GameData.Invoke_OnPlayerDataUpdate();
            return !_data.error;
        }
    }
}
