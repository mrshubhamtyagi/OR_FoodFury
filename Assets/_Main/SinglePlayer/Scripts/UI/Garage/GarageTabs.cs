using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class GarageTabs : MonoBehaviour
    {
        [SerializeField] private GameObject regularSelectedImage;
        [SerializeField] private TextMeshProUGUI regularTitleTMP;

        [SerializeField] private GameObject specialSelectedImage;
        [SerializeField] private TextMeshProUGUI specialTitleTMP;

        [SerializeField] private Button regularBtn;
        [SerializeField] private Button specialBtn;


        public void OnClick_Regular()
        {
            StartCoroutine(GarageScreen.Instance.InitVehicles(Enums.GarageVehicleType.Regular));
            UpdateUI(Enums.GarageVehicleType.Regular);
        }

        public void OnClick_Special()
        {
            StartCoroutine(GarageScreen.Instance.InitVehicles(Enums.GarageVehicleType.Special));
            UpdateUI(Enums.GarageVehicleType.Special);
        }

        public void UpdateUI(Enums.GarageVehicleType _type)
        {
            regularSelectedImage.SetActive(_type == Enums.GarageVehicleType.Regular);
            specialSelectedImage.SetActive(_type == Enums.GarageVehicleType.Special);

            regularBtn.interactable = _type == Enums.GarageVehicleType.Special;
            specialBtn.interactable = _type == Enums.GarageVehicleType.Regular;

            regularTitleTMP.color = _type == Enums.GarageVehicleType.Regular ? Color.black : ColorManager.Instance.DarkGrey;
            specialTitleTMP.color = _type == Enums.GarageVehicleType.Special ? Color.black : ColorManager.Instance.DarkGrey;
        }
    }
}