
using DG.Tweening;
using Fusion;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class SplashUIItem : MonoBehaviour
    {
        [SerializeField] private GameObject victoryUI;
        [SerializeField] private GameObject eliminatedUI;
        [SerializeField] private GameObject gameOverUI;

        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private Vector3 startScale = new Vector3(0.01f, 0.01f, 0.01f);
        private Tween animationTween;
        public void ShowVictory()
        {
            ShowPopUp(victoryUI);
        }

        public void ShowEliminated()
        {
            ShowPopUp(eliminatedUI);
        }

        public void ShowGameOver()
        {
            ShowPopUp(gameOverUI);
        }

        private void ShowPopUp(GameObject go)
        {
            go.transform.localScale = startScale;

            // Use DOTween to animate the scale to normal size
            animationTween =  go.transform.DOScale(Vector3.one, animationDuration)
                .SetEase(Ease.OutBack)  // Choose your desired easing function
                .OnComplete(() => 
                {
                    // This is called after the first scale animation completes
                    // Now return to the default scale
                    go.transform.DOScale(startScale, animationDuration)
                        .SetEase(Ease.InBack); // Choose an easing function for the reverse animation
                });
            
            Invoke("HideSplashUI",3f);
        }
        
        void HideSplashUI()
        {
            Destroy(gameObject);
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
            if (animationTween != null)
            {
                animationTween.Kill();
            }
        }
    }
}
