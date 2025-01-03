using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class TipPrefab : MonoBehaviour
    {
        [SerializeField] private Image thumbnail;
        [SerializeField] private TMPro.TextMeshProUGUI infoTMP;
        [SerializeField] private bool center;


        [Header("-----Front")]
        [SerializeField] private Vector2 frontSize;
        [SerializeField] private Vector2 frontPosition;
        [SerializeField] private Color frontColor;

        [Header("-----Back")]
        [SerializeField] private Vector2 backSize;
        [SerializeField] private Vector2 backPosition;
        [SerializeField] private Color backColor;


        private Color opaque;
        private Color alpha;

        private RectTransform rectTransfrom;

        private void Awake() => rectTransfrom = thumbnail.GetComponent<RectTransform>();

        private void Start()
        {
            opaque = Color.white;
            alpha = Color.white;
            alpha.a = 0;
        }

        public void SetTip(Tip _tip, bool _initialize = false)
        {
            thumbnail.sprite = _tip.thumbnail;
            infoTMP.text = _tip.info;

            //thumbnail.color = center ? frontColor : backColor;

            if (rectTransfrom == null) rectTransfrom = thumbnail.GetComponent<RectTransform>();

            if (_initialize)
            {
                rectTransfrom.anchoredPosition = center ? frontPosition : backPosition;
                rectTransfrom.sizeDelta = center ? frontSize : backSize;
                thumbnail.color = center ? frontColor : backColor;
                //infoTMP.color = center ? opaque : alpha;
            }
            else
                Refresh(center);
        }


        public void Refresh(bool _center)
        {
            center = _center;
            TweenHandler.UIPosition(rectTransfrom, center ? frontPosition : backPosition, 1, DG.Tweening.Ease.InOutExpo);
            TweenHandler.UISize(rectTransfrom, center ? frontSize : backSize, 1, DG.Tweening.Ease.InOutExpo);
            TweenHandler.ImageColor(thumbnail, center ? frontColor : backColor, 1, DG.Tweening.Ease.InOutExpo);
            TweenHandler.TextColor(infoTMP, center ? opaque : alpha, 0.5f, DG.Tweening.Ease.InOutExpo);
        }
    }

}