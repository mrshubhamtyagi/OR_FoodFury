using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class OrderStatus : MonoBehaviour
    {
        [SerializeField] private RawImage orderStatusThumbnail;
        [SerializeField] private Image orderStatusFillImage;
        [SerializeField] private TextMeshProUGUI orderStatusTMP;
        [SerializeField] private Image statusIcon;
        [SerializeField] private OrderStatusDetail[] orderStatusDetails;

        [Header("-----NFT Info")]
        [SerializeField] private Image bg;
        [SerializeField] private TextMeshProUGUI titleTMP;
        [SerializeField] private TextMeshProUGUI nftLevelTMP;
        [SerializeField] private TextMeshProUGUI nftBonusTMP;
        [SerializeField] private GameObject nftIcon;
        [SerializeField] private GameObject lockIcon;

        private Image mainBg;
        private RectTransform rectTransform;
        private Coroutine routine;

        private int yPos = 180;
        private int xPosEnter = -500;
        private int xPosExit = 100;
        private int yPosMP = 320;
        private int xPosEnterMP = -500;
        private int xPosExitMP = 430;
        void Awake()
        {
            mainBg = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPosEnter, yPos);
        }

        public void HideOrderStatus()
        {
            if (GameData.Instance.gameMode == Enums.GameModeType.Multiplayer)
            {
                TweenHandler.UIPosition(rectTransform, new Vector2(xPosEnterMP, yPosMP),
                    ScreenManager.Instance.TweenDuration,
                    ScreenManager.Instance.TweenEase);
            }
            else
            {
                TweenHandler.UIPosition(rectTransform, new Vector2(xPosEnter, yPos),
                    ScreenManager.Instance.TweenDuration,
                    ScreenManager.Instance.TweenEase);
            }
        }

        public void ShowOrderStatus(Enums.OrderStatus _status, OrderNFTDetails _nftDetail = null, Texture2D _orderImage = null, float _initialDelay = 0, float _autoHideDelay = 1)
        {
            //Debug.Log(_status);
            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(Co_ShowOrderStatus(_status, _nftDetail, _orderImage, _initialDelay, _autoHideDelay));
        }

        private IEnumerator Co_ShowOrderStatus(Enums.OrderStatus _status, OrderNFTDetails _nftDetail, Texture2D _orderImage, float _initialDelay, float _autoHideDelay)
        {
            //if (_orderImage != null) orderStatusThumbnail.texture = _orderImage;
            yield return new WaitForSeconds(_initialDelay);
            switch (_status)
            {
                case Enums.OrderStatus.NewOrder:
                    orderStatusThumbnail.texture = _orderImage;
                    SetDetail(orderStatusDetails[0]);
                    orderStatusFillImage.fillAmount = 1;

                    lockIcon.SetActive(!_nftDetail.isNFT);
                    nftIcon.SetActive(_nftDetail.isNFT);
                    titleTMP.text = _nftDetail.isNFT ? "Bonus:" : "Unlock:";
                    nftLevelTMP.text = _nftDetail.isNFT ? "L" + _nftDetail.nftLevel.ToString() : string.Empty;
                    nftBonusTMP.text = "+" + _nftDetail.nftPoint.ToString();
                    titleTMP.color = nftBonusTMP.color = _nftDetail.isNFT ? Color.black : Color.white;
                    bg.color = mainBg.color = _nftDetail.isNFT ? ColorManager.Instance.LightGreen : ColorManager.Instance.DarkGrey;
                    break;

                case Enums.OrderStatus.Delivering:
                    SetDetail(orderStatusDetails[1]);
                    orderStatusFillImage.fillAmount = 0;
                    break;

                case Enums.OrderStatus.Delivered:
                    SetDetail(orderStatusDetails[2]);
                    orderStatusFillImage.fillAmount = 1;
                    break;

                case Enums.OrderStatus.MissedIt:
                    SetDetail(orderStatusDetails[3]);
                    orderStatusFillImage.fillAmount = 1;
                    break;
            }

            if (GameData.Instance.gameMode == Enums.GameModeType.Multiplayer)
            {
                TweenHandler.UIPosition(rectTransform, new Vector2(xPosExitMP, yPosMP), ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
                {
                    if (_autoHideDelay > 0) Invoke("HideOrderStatus", _autoHideDelay);
                });
            }
            else
            {
                TweenHandler.UIPosition(rectTransform, new Vector2(xPosExit, yPos), ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
                {
                    if (_autoHideDelay > 0) Invoke("HideOrderStatus", _autoHideDelay);
                });
            }
          
        }

        private void SetDetail(OrderStatusDetail _detail)
        {
            orderStatusTMP.text = _detail.title;
            orderStatusFillImage.color = _detail.color;
            statusIcon.sprite = _detail.icon;
        }



        public void ShowDeliveringStatus(float _time) => orderStatusFillImage.fillAmount = _time;

    }



    [Serializable]
    public class OrderStatusDetail
    {
        public string title;
        public Color color;
        public Sprite icon;
    }
}