using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FoodFury
{
    public static class ChestAPIs
    {
        public static async Task<ModelClass.ErrorAndResultResponse<List<ModelClass.ChestData>>> GetUserChests_API()
        {
            var _response = await APIManager.GetUserChestsAsync(GameData.Instance.PlayerId);
            //Debug.Log($"GetUserChests_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<List<ModelClass.ChestData>>>(_response.result);
        }



        public static async Task<bool> BuyChest_API(Enums.ChestType _type)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return true;

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("chestType", _type.ToString());
            //Debug.Log($"BuyChest - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.BuyChestAsync(_params.ToJSONObject());
            //Debug.Log($"BuyChest_API - Status {_response.error} | Response {_response.result}");

            if (_response.error) return false;
            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<string>>(_response.result);
            return !_data.error;
        }

        public static async Task<ModelClass.ErrorAndResultResponse<ModelClass.ChestBuyResponse.Result>> UnlockChest_API(string _id)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                GameData.Instance.PlayerData.Data.munches += 10; // Adding Munches in Debug mode
                GameData.Invoke_OnPlayerDataUpdate();
                return new ModelClass.ErrorAndResultResponse<ModelClass.ChestBuyResponse.Result>()
                {
                    error = false,
                    result = new ModelClass.ChestBuyResponse.Result()
                    {
                        munch = 10,
                        dishes = null
                    }
                };
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("chestId", _id);
            //Debug.Log($"UnlockChest - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UnlockChestAsync(_params.ToJSONObject());
            //Debug.Log($"UnlockChest_API - Status {_response.error} | Response {_response.result}");

            if (_response.error) return new() { error = true, result = null };
            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<ModelClass.ChestBuyResponse.Result>>(_response.result);
            return _data;
        }


        public static async Task<ModelClass.ErrorAndResultResponse<ModelClass.ChestBuyResponse.Result>> UnlockChest_API(string _id, int _cost)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                if (!GameData.Instance.DeductCost(Enums.CostType.POINTS, _cost))
                {
                    OverlayWarningPopup.Instance.ShowWarning("Insufficient Balance!");
                    return new() { error = true, result = null };
                }


                GameData.Instance.PlayerData.Data.munches += 10; // Adding Munches in Debug mode
                GameData.Invoke_OnPlayerDataUpdate();
                return new ModelClass.ErrorAndResultResponse<ModelClass.ChestBuyResponse.Result>()
                {
                    error = false,
                    result = new ModelClass.ChestBuyResponse.Result()
                    {
                        munch = 10,
                        dishes = null
                    }
                };
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("chestId", _id);
            _params.Add("chips", _cost);
            //Debug.Log($"UnlockChest - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UnlockChestWithChipsAsync(_params.ToJSONObject());
            //Debug.Log($"UnlockChest_API - Status {_response.error} | Response {_response.result}");

            if (_response.error) return new() { error = true, result = null };
            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<ModelClass.ChestBuyResponse.Result>>(_response.result);
            return _data;
        }
    }
}
