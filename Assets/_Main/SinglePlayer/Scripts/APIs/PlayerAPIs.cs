using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace FoodFury
{
    public static class PlayerAPIs
    {
        public static async Task<bool> GetPlayerData_API(string _playerId)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return true;

            var _response = await APIManager.GetPlayerDataAsync(_playerId);
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<ModelClass.PlayerData>>(_response.result);
            if (!_data.error)
            {
                GameData.Instance.UpdatePlayerDataLocal(_data.result);
                GameData.Instance.AddMapsToPlayerLevels();
            }
            return !_data.error;
        }



        // When Level is completed Or retry munches are greater than prev value
        public static async Task<bool> UpdatePlayerDataLevel_API(int _munches, int _mapId, ModelClass.Analytics _analytics, bool _retry = false)
        {
            int _nextLevel;
            if (GameData.Instance.SelectedLevelNumber + 1 > GameData.Instance.GetLevels().Count)
                _nextLevel = GameData.Instance.SelectedLevelNumber;
            else
                _nextLevel = GameData.Instance.SelectedLevelNumber + 1;

            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                // Add to Player levels
                AddCompletedLevel(_analytics);

                if (!_retry && GameData.Instance.SelectedLevelNumber < GameData.Instance.GetLevels().Count) GameData.Instance.GetPlayerLevelNumberData().levelNumber++;
                GameData.Instance.PlayerData.Data.munches += _munches;
                GameData.Invoke_OnPlayerDataUpdate();
                return true;
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("fuel", GameData.Instance.PlayerData.Data.fuel > GameData.Instance.TankCapacityInSeconds ? GameData.Instance.TankCapacityInSeconds : GameData.Instance.PlayerData.Data.fuel);
            _params.Add("munches", _munches);
            if (!_retry)
            {
                _params.Add("mapId", _mapId);
                _params.Add("level", _nextLevel);
            }
            //Debug.Log($"UpdatePlayerDataLevel - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpdatePlayerDataLevelAsync(_params.ToJSONObject());
            if (_response.error) return false;
            //Debug.Log($"UpdatePlayerDataLevel_API - Status {_response.error} | Response {_response.result}");

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"Could not Update Player Data [L] - {_response}");
                return false;
            }

            AddCompletedLevel(_analytics);

            if (!_retry && GameData.Instance.SelectedLevelNumber < GameData.Instance.GetLevels().Count) GameData.Instance.GetPlayerLevelNumberData().levelNumber++;
            GameData.Instance.PlayerData.Data.munches += _munches;
            GameData.Invoke_OnPlayerDataUpdate();
            return true;


            void AddCompletedLevel(ModelClass.Analytics _analytics)
            {
                GameData.Instance.AddCompletedLevelStats(new ModelClass.PlayerLevelStats()
                {
                    level = _analytics.level,
                    munches = _munches,
                    chips = _analytics.chips,
                    stars = _analytics.stars
                }, _retry);
            }
        }



        // When order is delivered
        public static async Task<bool> UpdatePlayerDataOrder_API(Order _order)
        {
            int _chips = GameController.Instance.CalculateChips(_order);

            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                GameData.Instance.PlayerData.Data.deliveries++;
                GameData.Instance.PlayerData.Data.chips += _chips;
                GameData.Invoke_OnPlayerDataUpdate();
                return true;
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("deliveries", GameData.Instance.PlayerData.Data.deliveries + 1);
            _params.Add("chips", _chips);
            _params.Add("fuel", GameData.Instance.PlayerData.Data.fuel > GameData.Instance.TankCapacityInSeconds ? GameData.Instance.TankCapacityInSeconds : GameData.Instance.PlayerData.Data.fuel);
            //Debug.Log($"UpdatePlayerDataOrder - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpdatePlayerDataAsync(_params.ToJSONObject());
            //Debug.Log($"UpdatePlayerDataOrder_API - Status {_response.error} | Response {_response.result}");
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"Could not Update Player Data [O] - {_response}");
                return false;
            }

            GameData.Instance.PlayerData.Data.deliveries++;
            GameData.Instance.PlayerData.Data.chips += _chips;
            GameData.Invoke_OnPlayerDataUpdate();
            return true;
        }



        // When user takes Initial Booster on Game Start Popup | When Onboarding is completed
        public static async Task<bool> UpdatePlayerDataChips_API(float _cost)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                GameData.Instance.PlayerData.Data.chips += _cost;
                GameData.Invoke_OnPlayerDataUpdate();
                return true;
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("chips", _cost);
            //Debug.Log($"UpdatePlayerDataChips - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpdatePlayerDataAsync(_params.ToJSONObject());
            //Debug.Log($"UpdatePlayerDataChips_API - Status {_response.error} | Response {_response.result}");
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"Could not Update Player Data [C] - {_response}");
                return false;
            }

            GameData.Instance.PlayerData.Data.chips += _cost;
            GameData.Invoke_OnPlayerDataUpdate();
            return true;
        }



        // When current vehicle is changed
        public static async Task<bool> UpdatePlayerDataCurrentVehicle_API(int _vehicleId)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                ModelClass.GarageVehicleData previousCarData = GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle);
                GameData.Instance.PlayerData.Data.currentVehicle = _vehicleId;
                ModelClass.GarageVehicleData newCarData = GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle);

                AnalyticsManager.Instance.FireVehicleSwitchedSuccessfulEvent(previousCarData.id, previousCarData.name, newCarData.id, newCarData.name);
                return true;
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("currentVehicle", _vehicleId);
            //Debug.Log($"UpdatePlayerDataCurrentVehicle - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpdatePlayerDataAsync(_params.ToJSONObject());
            //Debug.Log($"UpdatePlayerDataCurrentVehicle_API - Status {_response.error} | Response {_response.result}");
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"Could not Update Player Data [V] - {_response}");
                return false;
            }

            ModelClass.GarageVehicleData _previousCarData = GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle);
            GameData.Instance.PlayerData.Data.currentVehicle = _vehicleId;
            ModelClass.GarageVehicleData _newCarData = GameData.Instance.GarageData.GetVehicleGarageData(GameData.Instance.PlayerData.Data.currentVehicle);

            AnalyticsManager.Instance.FireVehicleSwitchedSuccessfulEvent(_previousCarData.id, _previousCarData.name, _newCarData.id, _newCarData.name);
            return true;
        }



        // When Game is ended
        public static async Task<bool> UpdatePlayerDataFuel_API()
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return true;

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("fuel", GameData.Instance.PlayerData.Data.fuel > GameData.Instance.TankCapacityInSeconds ? GameData.Instance.TankCapacityInSeconds : GameData.Instance.PlayerData.Data.fuel);
            //Debug.Log($"UpdatePlayerDataFuel - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpdatePlayerDataAsync(_params.ToJSONObject());
            //Debug.Log($"UpdatePlayerDataFuel_API - Status {_response.error} | Response {_response.result}");
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"Could not Update Player Data [F] - {_response}");
                return false;
            }

            return !_data.error;
        }
        public static async Task<bool> VerifyCode_API(string _code)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return true;

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("tgId", _code);
            Debug.Log($"VerifyCode_API - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.VerifyCodeAsync(_params.ToJSONObject());
            //Debug.Log($"UpdatePlayerDataChips_API - Status {_response.erVerifyCode_APIror} | Response {_response.result}");
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<ModelClass.PlayerData>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"{_response.result}");
                return false;
            }

            Debug.Log($"No Error:- {_response.result}");
            PlayerPrefsManager.Instance.SavePlayerId(_data.result._id);
            GameData.Instance.UpdatePlayerDataLocal(_data.result);
            return true;
        }

        //public static async Task<bool> UpdateUserName_API()
        //{
        //    if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
        //        return true;

        //    Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
        //    _params.Add("username", GameData.Instance.TelegramUserName);
        //    //Debug.Log($"UpdatePlayerDataFuel - {JsonConvert.SerializeObject(_params)}");

        //    var _response = await APIManager.UpdateUserNameAsync(_params.ToJSONObject());
        //    //Debug.Log($"UpdatePlayerDataFuel_API - Status {_response.error} | Response {_response.result}");
        //    if (_response.error) return false;

        //    var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
        //    if (_data.error)
        //    {
        //        Debug.Log($"Could not Update userName [F] - {_response}");
        //        return false;
        //    }

        //    return !_data.error;
        //}



        // When Multiplayer Game is ended
        public static async Task<bool> UpdatePlayerDataGameOverMultiplayer_API(int _chips, int _munches)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                GameData.Instance.PlayerData.Data.munches += _munches;
                GameData.Instance.PlayerData.Data.chips += _chips;
                GameData.Invoke_OnPlayerDataUpdate();
                return true;
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("munches", _munches);
            _params.Add("chips", _chips);
            _params.Add("fuel", GameData.Instance.PlayerData.Data.fuel > GameData.Instance.TankCapacityInSeconds ? GameData.Instance.TankCapacityInSeconds : GameData.Instance.PlayerData.Data.fuel);
            Debug.Log($"UpdatePlayerDataGameOverMultiplayer - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpdatePlayerDataAsync(_params.ToJSONObject());
            //Debug.Log($"UpdatePlayerDataGameOverMultiplayer_API - Status {_response.error} | Response {_response.result}");
            if (_response.error) return false;

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            if (_data.error)
            {
                Debug.Log($"Could not Update Player Data [O] - {_response}");
                return false;
            }

            GameData.Instance.PlayerData.Data.munches += _munches;
            GameData.Instance.PlayerData.Data.chips += _chips;
            GameData.Invoke_OnPlayerDataUpdate();
            return true;
        }

    }
}
