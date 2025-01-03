using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class DishDetailsPrefab : MonoBehaviour
    {
        [SerializeField] private DishCard dishCard;
        [SerializeField] private TextMeshProUGUI dishCountsTMP, feesTMP;
        [SerializeField] private string highlightColor;
        // [SerializeField] private GameObject selector;
        [SerializeField] private Button levelUpBtn;

        [Header("-----Debug")]
        [SerializeField] private ModelClass.Dish dish;

        private int highestLevelDish => 7;

        public async Task SetData(ModelClass.Dish _dish)
        {
            dish = _dish;
            await dishCard.FillData(_dish);
            dishCountsTMP.text = $"<size={dishCountsTMP.fontSize + 5}><b><color={highlightColor}>{_dish.quantity}</color></b></size>/{_dish.qtyForUpgrade}";
            feesTMP.text = _dish.level < highestLevelDish ? _dish.upgradeFee.ToString() : "-";

            levelUpBtn.interactable = _dish.quantity >= _dish.qtyForUpgrade && _dish.level < highestLevelDish;
            //selector.SetActive(levelUpBtn.interactable);

            gameObject.name = _dish.name;
            gameObject.SetActive(true);
        }

        public void OnClick_LevelUp()
        {
            levelUpBtn.interactable = false;
            ConfirmPurchasePopup.Instance.Show("Are you sure you want tp Level up the dish?", dish.upgradeFee, async _result =>
            {
                if (_result)
                {
                    Debug.Log($"{gameObject.name} Leveled Up");
                    var _success = await DishAPIs.UpgraeDish_API(dish.tokenId);
                    if (_success)
                    {
                        GameData.Instance.PlayerData.Data.chips -= dish.upgradeFee;
                        GameData.Invoke_OnPlayerDataUpdate();
                    }
                    else
                    {
                        OverlayWarningPopup.Instance.ShowWarning("Something went wrong");
                    }

                    await ProfileScreen.Instance.Init();
                }
                else levelUpBtn.interactable = true;
            });
        }

        public void OnClick_Sell() => ProfileScreen.Instance.Sell();
    }
}
