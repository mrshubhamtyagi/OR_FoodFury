using System.Linq;
using UnityEngine;

namespace FoodFury
{
    public class VehicleVarient : MonoBehaviour
    {
        [SerializeField] private bool isPlayer;

        [Header("-----Player")]
        [SerializeField] private VehicleModel[] vehicleModels;
  

        [Header("-----Traffic")]
        [SerializeField] private bool onlyColor = false;
        [SerializeField] private GameObject[] varient;


        private Vehicle vehicle;
        void Awake() => vehicle = GetComponent<Vehicle>();


        void Start()
        {
            if (isPlayer) PlayerSetup();
            else TrafficSetup();
        }


        private void PlayerSetup()
        {
            VehicleModel _model = Instantiate(vehicleModels.First(v => v.VehicleID == GameData.Instance.PlayerData.Data.currentVehicle), transform);
            vehicle.SetBody(_model.body, _model.handle);


            if (_model.normalEngineSound == null) return;
            vehicle.VehicleSound.OnVehicleModelChanged(_model);
            //vehicle.VehicleEffects.SetWheels(_model.primaryWheel, _model.otherWheel);
        }

        private void TrafficSetup()
        {
            int _index = 0;

            if (onlyColor)
            {
                Color _randomColor = GetRandomColor();
                for (int i = 0; i < varient.Length; i++)
                    varient[i].GetComponent<Renderer>().materials.First(m => m.name.StartsWith("custom_1")).color = _randomColor;
            }
            else
            {
                _index = Random.Range(0, varient.Length);
                for (int i = 0; i < varient.Length; i++)
                    varient[i].SetActive(i == _index);

                varient[_index].GetComponent<Renderer>().materials.First(m => m.name.StartsWith("custom_1")).color = GetRandomColor();
            }
        }



        private Color GetRandomColor() => new Color(Random.Range(0.25f, 0.75f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f));
    }




}
