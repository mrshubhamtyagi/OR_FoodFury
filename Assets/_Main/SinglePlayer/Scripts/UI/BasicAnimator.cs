using UnityEngine;

namespace FoodFury
{
    public class BasicAnimator : MonoBehaviour
    {
        [SerializeField] private Enums.AnimType animType = Enums.AnimType.Scale;
        [SerializeField] private DG.Tweening.Ease ease;
        [SerializeField] private float duration;
        [SerializeField] private Vector3 startValue;
        [SerializeField] private Vector3 endValue;
        [SerializeField] private bool play = false;
        [SerializeField] private bool playAtStart = false;


        private void Start()
        {
            if (playAtStart)
                Play(animType);
        }

        public void Play(Enums.AnimType _animType)
        {
            play = true;
            if (_animType == Enums.AnimType.Scale)
                AnimScale();
            else
                AnimPosition();
        }

        [ContextMenu("AnimPosition")]
        private void AnimScale()
        {
            if (!play) return;
            TweenHandler.TransformScale(transform, endValue, duration, ease, () =>
            {
                TweenHandler.TransformScale(transform, startValue, duration, ease, () =>
                {
                    AnimScale();
                });
            });
        }

        [ContextMenu("AnimPosition")]
        private void AnimPosition()
        {
            if (!play) return;
            TweenHandler.TransformPosition(transform, endValue, duration, ease, () =>
            {
                TweenHandler.TransformPosition(transform, startValue, duration, ease, () =>
                {
                    AnimPosition();
                });
            });
        }

        public void Stop() => play = false;
    }

}