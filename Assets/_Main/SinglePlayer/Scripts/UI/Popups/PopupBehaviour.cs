using System;
using UnityEngine;

namespace FoodFury
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PopupBehaviour : MonoBehaviour
    {
        public float TweenDuration => 0.5f;
        public DG.Tweening.Ease TweenEase => DG.Tweening.Ease.InOutExpo;

        public CanvasGroup canvasGroup { get; private set; }

        protected void Init(CanvasGroup _canvasGroup, bool _defaultState = false)
        {
            canvasGroup = _canvasGroup;
            canvasGroup.blocksRaycasts = _defaultState;
            canvasGroup.alpha = _defaultState ? 1 : 0;
        }

        public virtual void Init() { }

        public virtual void Close() => Hide();

        public virtual void Show(Action _callback = null)
        {
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, TweenDuration, TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();
            });
        }

        public virtual void Hide(Action _callback = null)
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, TweenDuration, TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }
    }
}
