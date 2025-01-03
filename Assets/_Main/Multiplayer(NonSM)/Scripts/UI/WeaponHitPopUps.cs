using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OneRare.FoodFury.Multiplayer
{
    public class WeaponHitPopUps : MonoBehaviour
    {
        
        [SerializeField] private Image ketchupHitImage;
        [SerializeField] private Image submissileHitImage;
        [SerializeField] private Image oilspillHitImage;
        private Player player;
        
        private Tween animationTween;
        public void Init(Player player)
        {
            /*ketchupHitImage.gameObject.SetActive(false);
            submissileHitImage.gameObject.SetActive(false);
            oilspillHitImage.gameObject.SetActive(false);*/
            this.player = player;
        }
        public void OnWeaponHit(WeaponManager.WeaponInstallationType weaponInstallationType)
        {
            if (weaponInstallationType == WeaponManager.WeaponInstallationType.KETCHUP)
            {
                ShowPopUp(ketchupHitImage.gameObject);
            }
            else if (weaponInstallationType == WeaponManager.WeaponInstallationType.SUBMISSILE)
            {
                ShowPopUp(submissileHitImage.gameObject);
            }
            else if (weaponInstallationType == WeaponManager.WeaponInstallationType.OILSPILL)
            {
                ShowPopUp(oilspillHitImage.gameObject);
            }
            else
            {
                Debug.Log("Weapon Not Installed");   
            }
        }
        
        private Vector3 startScale = new Vector3(0.0f, 0.0f, 0.0f);
        [SerializeField] private float animationDuration = 0.5f;
        private void ShowPopUp(GameObject go)
        {
            gameObject.SetActive(true);
            go.transform.localScale = startScale;

            // Use DOTween to animate the scale to normal size
            animationTween = go.transform.DOScale(Vector3.one, animationDuration)
                .SetEase(Ease.OutBack)  // Choose your desired easing function
                .OnComplete(() => 
                {
                    // This is called after the first scale animation completes
                    // Now return to the default scale
                    go.transform.DOScale(startScale, animationDuration)
                        .SetEase(Ease.InBack); // Choose an easing function for the reverse animation
                    //go.SetActive(false);
                });
        }
        
        private void OnDestroy()
        {
            // Kill the tween to prevent errors
            if (animationTween != null)
            {
                animationTween.Kill();
            }
        }

        private void OnDisable()
        {
            // Kill the tween to prevent errors
            if (animationTween != null && animationTween.IsActive())
            {
                animationTween.Kill();
            }
        }

    }
}
