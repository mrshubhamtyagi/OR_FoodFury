using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FoodFury
{
    public static class LevelsAndMapAPIs
    {
        public static async Task<ModelClass.ErrorAndResultResponse<List<ModelClass.MapDetail>>> GetMapData_API()
        {
            var _response = await APIManager.GetMapsDetailsAsync();
            //Debug.Log($"GetMapData_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<List<ModelClass.MapDetail>>>(_response.result);
        }


        public static async Task<bool> GetMapMusic_API()
        {
            var _response = await APIManager.DownloadTrack(GameData.Instance.GetSelectedMapData().mapTrack);
            //Debug.Log($"GetMapData_API - Status {_response.error} | Response {_response.result}");

            if (!_response.error) return false;
            AudioManager.Instance.SetGamePlayMusicAudio(_response.result);
            return true;
        }


        public static async Task<ModelClass.ErrorAndResultResponse<LevelDataSO>> GetLevels_API()
        {
            var _response = await APIManager.GetLatestLevelsAsync();
            //Debug.Log($"GetLevels_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<LevelDataSO>>(_response.result);
        }

        public static async Task<ModelClass.ErrorAndResultResponse<List<ModelClass.CompletedLevels>>> GetCompletedLevels_API()
        {
            var _response = await APIManager.GetPlayerLevelsAsync(GameData.Instance.PlayerId);
            //Debug.Log($"GetLevels_API - Status {_response.error} | Response {_response.result}");
            return _response.error ? new() : JsonConvert.DeserializeObject<ModelClass.ErrorAndResultResponse<List<ModelClass.CompletedLevels>>>(_response.result);
        }
    }
}
