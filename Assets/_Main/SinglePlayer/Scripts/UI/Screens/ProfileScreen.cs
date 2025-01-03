using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FoodFury
{
    public class ProfileScreen : MonoBehaviour, IScreen
    {
        [Header("-----My Dishes")]
        [SerializeField] private DishDetailsPrefab dishDetailsPrefab;
        [SerializeField] private RectTransform dishesContent;
        [SerializeField] private GameObject comingSoonObj;
        private CanvasGroup canvasGroup;

        public Dictionary<int, Texture> CountryFlagDictionary { get; private set; }
        public Dictionary<int, Texture> dishDictionary { get; private set; }
        public static ProfileScreen Instance { get; private set; }
        void Awake()
        {
            if (Instance != null) return;

            Instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            CountryFlagDictionary = new();
            dishDictionary = new();
        }

        public void Sell()
        {
            comingSoonObj.SetActive(true);
            Invoke("HideComingSoon", 2);
        }

        private void HideComingSoon() => comingSoonObj.SetActive(false);

        public async Task Init()
        {
            comingSoonObj.SetActive(false);
            var _success = await DishAPIs.GetUserDishes_API();
            if (_success)
            {
                int _childCount = dishesContent.childCount;
                for (int i = 0; i < GameData.Instance.UserDishData.Data.Count; i++)
                {
                    if (i < _childCount)
                        await dishesContent.GetChild(i).GetComponent<DishDetailsPrefab>().SetData(GameData.Instance.UserDishData.Data[i]);
                    else
                        await Instantiate(dishDetailsPrefab, dishesContent).SetData(GameData.Instance.UserDishData.Data[i]);
                }

                for (int i = GameData.Instance.UserDishData.Data.Count; i < dishesContent.childCount; i++)
                    dishesContent.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                OverlayWarningPopup.Instance.ShowWarning("Could not fetch dishes!");
            }

            dishesContent.anchoredPosition = new Vector2(0, 0);
        }
        public void AddCountryImage(int countryHashName, Texture image)
        {
            CountryFlagDictionary.Add(countryHashName, image);
        }
        public bool HasCountryImage(int countryHashName)
        {
            return CountryFlagDictionary.ContainsKey(countryHashName);
        }
        public bool HasDishImage(int tokenId)
        {
            return dishDictionary.ContainsKey(tokenId);
        }
        public void AddDishImage(int tokenId, Texture image)
        {
            Debug.Log(tokenId);
            dishDictionary.Add(tokenId, image);
        }
        public async void Show(Action _callback = null)
        {
            await Init();

            ScreenManager.Instance.CurrentScreen = Enums.Screen.Profile;
            MenuBar.Instance.Show();
            TweenHandler.CanvasGroupAlpha(canvasGroup, 1, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            {
                canvasGroup.blocksRaycasts = true;
                _callback?.Invoke();
                Loader.Instance.HideLoader();
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
