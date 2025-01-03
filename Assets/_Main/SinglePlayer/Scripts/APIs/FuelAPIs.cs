using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace FoodFury
{
    public static class FuelAPIs
    {
        public static async Task<ModelClass.ErrorAndResultResponse<FuelDataSO>> GetFuelData_API()
        {
            var _response = await APIManager.GetFuelPurchaseDataAsync();
            //Debug.Log($"GetFuelData_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<FuelDataSO>>(_response.result);
        }


    }
}
