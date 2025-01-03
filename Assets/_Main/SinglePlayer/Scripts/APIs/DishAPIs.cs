using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FoodFury
{
    public static class DishAPIs
    {
        public static async Task<bool> GetUserDishes_API()
        {
            var _response = await APIManager.GetUserDishesAsync(GameData.Instance.PlayerId);
            //Debug.Log($"GetUserDishes_API - Status {_response.error} | Response {_response.result}");

            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<List<ModelClass.Dish>>>(_response.result);
            GameData.Instance.UserDishData.Data = _data.result;
            return !_response.error;
        }

        public static async Task<bool> UpgraeDish_API(int _id)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            {
                var _dish = GameData.Instance.UserDishData.GetDishByTokenId(_id);
                _dish.level++;
                _dish.quantity -= 2;
                return true;
            }

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("dishId", _id);
            //Debug.Log($"UpgradeDish - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.UpgradeDishAsync(_params.ToJSONObject());
            //Debug.Log($"UpgraeDish_API - Status {_response.error} | Response {_response.result}");

            if (_response.error) return false;
            var _data = JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<bool>>(_response.result);
            return !_data.error;
        }
    }
}
