using System;
using UnityEngine;

namespace FoodFury
{
    public class ExitGamePopup : PopupBehaviour
    {
        Action<bool> OnExitCallback;

        void Awake()
        {
            Init(GetComponent<CanvasGroup>());
        }

        public void OnClick_Yes()
        {
            OnExitCallback?.Invoke(true);
            OnExitCallback = null;
            Hide();
            GameController.IsGamePaused = false;
            GameData.Instance.EndGame();
        }

        public void OnClick_No()
        {
            OnExitCallback?.Invoke(false);
            OnExitCallback = null;
            Hide();
        }



        public void Show(Action<bool> _onExitCallback, Action _callback = null)
        {
            OnExitCallback = _onExitCallback;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, TweenDuration, TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();
            });
        }

    }

}