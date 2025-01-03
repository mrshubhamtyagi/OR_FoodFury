using System;
using UnityEngine;

namespace FoodFury
{
    public class LobbyScreen : MonoBehaviour, IScreen
    {
        private CanvasGroup canvasGroup;

        public static LobbyScreen Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
        }








        public void Init()
        {

        }



        public void Show(Action _callback = null)
        {
            Init();

            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                ScreenManager.Instance.CurrentScreen = Enums.Screen.Lobby;
                Loader.Instance.HideLoader();
                _callback?.Invoke();
            });
        }

        public void Hide(Action _callback = null)
        {
            Loader.Instance.ShowLoader();
            canvasGroup.blocksRaycasts = false;
            TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                _callback?.Invoke();
            });
        }
    }

}