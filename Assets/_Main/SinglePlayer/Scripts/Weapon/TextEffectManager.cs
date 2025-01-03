using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace FoodFury
{
    public class TextEffectManager : MonoBehaviour
    {
        [SerializeField] private GameObject gotchaText;
        [SerializeField] private GameObject oilSpillText;
        [SerializeField] private GameObject ketchupText;
        [SerializeField] private GameObject submissileText;
        // Start is called before the first frame update
        private void OnEnable()
        {
            Weapon.OnHitAI += ShowGotchaText;
            Weapon.OnHitPlayer += ShowText;
        }
        private void OnDisable()
        {
            Weapon.OnHitAI -= ShowGotchaText;
            Weapon.OnHitPlayer -= ShowText;
        }
        private void ShowText(WeaponType weaponType)
        {
            if (weaponType == WeaponType.subMissile)
            {
                AnimateText(submissileText);
            }
            else if (weaponType == WeaponType.oilspill)
            {
                AnimateText(oilSpillText);
            }
            else if (weaponType == WeaponType.ketchup)
            {
                AnimateText(ketchupText);
            }
        }

        private void ShowGotchaText(WeaponType weaponType, Enums.RiderType weaponTriggedBy)
        {
            if (weaponTriggedBy == Enums.RiderType.Player)
            {
                AnimateText(gotchaText);
            }
        }
        public void AnimateText(GameObject text)
        {
            RectTransform rectTransform = text.GetComponent<RectTransform>();
            // Enable the text
            text.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            rectTransform.localScale = Vector3.zero;
            Color initialColor = Color.white;
            initialColor.a = 1f;
            text.GetComponent<Image>().color = initialColor;

            sequence.Append(rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce));
            // Scale down the image from one to zero over a duration of 0.5 second using DoTween
            sequence.Append(text.GetComponent<Image>().DOFade(0, 0.5f).SetEase(Ease.Linear)).SetDelay(0.1f);

            // When the animation is complete, disable the image
            sequence.OnComplete(() =>
            {
                text.SetActive(false);
            });
        }
        [ContextMenu("Show Random Text")]
        public void ShowRandomTextPopUp()
        {
            int index = UnityEngine.Random.Range(1, 5);
            if (index == 1)
            {
                ShowGotchaText(WeaponType.none, Enums.RiderType.Player);
            }
            else if (index == 2)
            {
                AnimateText(oilSpillText);
            }
            else if (index == 3)
            {
                AnimateText(ketchupText);
            }
            else if (index == 4)
            {
                AnimateText(submissileText);
            }
        }
    }

}
