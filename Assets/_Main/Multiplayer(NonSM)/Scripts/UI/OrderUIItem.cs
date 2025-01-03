using FoodFury;
using OneRare.FoodFury.Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneRare.FoodFury.Multiplayer
{
    public class OrderUIItem : MonoBehaviour
    {

        private OrderMP order = new OrderMP();

        [Header("-----Order UI Settings")]
        //[SerializeField] private OrderStatus orderStatus;
        [SerializeField]
        private Image orderIcon;

        [SerializeField] private TextMeshProUGUI dishLevelTMP;
        [SerializeField] private Image orderTimeoutFillImage;
        [SerializeField] private Image dishlevelBGImage;
        //[SerializeField] private TextMeshProUGUI dishNameTMP;
        //[SerializeField] private TextMeshProUGUI orderTimeoutTMP;
        //[SerializeField] private RawImage orderImage;
        //[SerializeField] private Image timeoutFillImage;
        //[SerializeField] private Image dishNameImage;
        //[SerializeField] private Transform[] nftBag;
        //[SerializeField] private GameObject orderTimeout;
        private Image[] dishLevel;
        private OrderCollectedItem orderCollectedItem;

        public void SetOrder(OrderMP newOrder)
        {
            if (order != null)
            {
                order.OnOrderTimeUpdate -= HandleOrderTimeUpdate;
            }
            
            Color orderColor = Color.white;
            int colorIndex = newOrder.ContainerIndex;
           // Debug.LogError($"Color:{colorIndex}, CI:{newOrder.ContainerIndex}");
            switch (colorIndex)
            {
                case 0:
                    orderColor = ColorManager.Instance.Violet;
                    break;
                case 1:
                    orderColor = ColorManager.Instance.Orange;
                    break;
                case 2:
                    orderColor = ColorManager.Instance.Pink;
                    break;
            }

            order = newOrder;
            order.SetMinimapIconColor(orderColor);
            orderIcon.color = orderColor;
            if (order.IsNFTOrder)
            {
                dishLevelTMP.text = GameData.Instance.GetUserDishByToken(order.NFTOrderTokenId).level.ToString();
                dishlevelBGImage.color = ColorManager.Instance.DarkGreen;
            }
            else
            {
                dishLevelTMP.text = string.Empty;
                dishlevelBGImage.color = ColorManager.Instance.Red;
            }
            if (order != null)
            {
                order.OnOrderTimeUpdate += HandleOrderTimeUpdate;
            }
            /*orderImage.texture = order.Thumbnail;
            dishNameTMP.text = order.Dish.name;
            timeoutFillImage.fillAmount = 1;
            timeoutFillImage.color = ColorManager.Instance.DarkGreen;
            orderTimeoutTMP.text = order.RemainingTime.ToString();
            orderTimeout.SetActive(true);*/
            //order.OnOrderDeliveringTimeUpdate += orderStatus.ShowDeliveringStatus;
            //orderStatus.ShowOrderStatus(Enums.OrderStatus.NewOrder, _order.OrderNFTDetails, (Texture2D)orderImage.texture, _initialDelay: 2, _autoHideDelay: 3);

        }

        private int time;
        
        private void OnDisable()
        {
            if (order != null)
            {
                order.OnOrderTimeUpdate -= HandleOrderTimeUpdate;
            }
        }
        
        private void HandleOrderTimeUpdate(int remainingTimeInSeconds)
        {
            time = remainingTimeInSeconds;
            orderTimeoutFillImage.fillAmount = time / (GameData.Instance.LevelData.levels[1].orderTime * 1f);

            orderTimeoutFillImage.color = orderTimeoutFillImage.fillAmount < 0.33f ? ColorManager.Instance.Red :
                orderTimeoutFillImage.fillAmount < 0.66 ? ColorManager.Instance.Yellow :
                ColorManager.Instance.DarkGreen;
        }
        
    }
}