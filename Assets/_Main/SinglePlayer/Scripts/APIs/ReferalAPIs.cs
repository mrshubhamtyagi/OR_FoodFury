using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FoodFury
{
    public class ReferalAPIs : MonoBehaviour
    {
        public static async Task<ModelClass.ErrorAndResultResponse<ModelClass.ReferralDataResponse.Result>> GetReferalData()
        {
            var _response = await APIManager.GetReferalDataAsync(GameData.Instance.PlayerId);
            //Debug.Log($"GetUserChests_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<ModelClass.ReferralDataResponse.Result>>(_response.result);
        }
        public static async Task<ModelClass.ReferralDataResponse> ClaimReferal(int taskCode)
        {
            if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
                return new();

            Dictionary<string, object> _params = new() { { "userId", GameData.Instance.PlayerId } };
            _params.Add("taskNumber", taskCode);
            //Debug.Log($"BuyChest - {JsonConvert.SerializeObject(_params)}");

            var _response = await APIManager.ClaimReferalAsync(_params.ToJSONObject());
            //Debug.Log($"BuyChest_API - Status {_response.error} | Response {_response.result}");

            if (_response.error) return new();
            Debug.Log("Claim Referal:" + _response.result + _response.message);
            var _data = JsonConvert.DeserializeObject<ModelClass.ReferralDataResponse>(_response.result);
            return _data;
        }
    }
}
