using System.Collections;
using DG.Tweening;
using FoodFury;
using Fusion;
using OneRare.FoodFury.Multiplayer;
using Tanknarok.UI;
using UnityEngine;
using UnityEngine.UI;
using SceneManagerUnity = UnityEngine.SceneManagement;

namespace OneRare.FoodFury.Multiplayer
{
    using UnityEngine;
    using UnityEngine.UI;

    public class MobileInput : MonoBehaviour
    {
        [SerializeField] private Button shootButton;
        [SerializeField] private Image shootButtonSprite;
        [SerializeField] private Sprite ketchupSprite;  // Image for the ketchup weapon
        [SerializeField] private Sprite submissileSprite;
        [SerializeField] private Sprite oilSpillSprite;// Image for the submissile weapon

        private Transform _canvas;
        private NetworkRunner Runner;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>().transform;
            Runner = FindObjectOfType<NetworkRunner>();
            EnableShootButton(false);
        }

        public void OnToggleReady()
        {
            foreach (InputController ic in FindObjectsOfType<InputController>())
            {
                if (ic.Object.HasInputAuthority)
                    ic.ToggleReady();
            }
        }

        public void OnDisconnect()
        {
            DOTween.KillAll();
            if (Runner != null)
            {
                Runner.Shutdown(true);
            }
            ErrorBox eb = FindObjectOfType<ErrorBox>();
            eb.OnClose();
            Invoke("GoBackToHomeScreen", 1f);
            Destroy(eb.gameObject, 0.1f);
            // Load the HomeScene

            /*
            Loader.Instance.ShowLoader();
            SceneManager.SwitchToHomeScene(_callback: (op) =>
            {
                ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
            });
            */

            //App app = FindObjectOfType<App>();
            //app.ShowRoomOptionUI();
            //app.gameObject.SetActive(false);
            //Destroy(app.gameObject, 5f);

        }

        public void DisconnectAndRestartMPGame()
        {

            if (Runner != null)
            {
                Runner.Shutdown(true);
            }
            ErrorBox eb = FindObjectOfType<ErrorBox>();
            eb.OnReplay();
            Destroy(eb.gameObject, 0.1f);
            Loader.Instance.ShowLoader();
            /*SceneManager.SwitchToHomeScene(_callback: (op) =>
            {
                ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
            });*/

            App app = FindObjectOfType<App>();
            app.ShowRoomOptionUI();
            Destroy(app.gameObject, 0.1f);

            DOTween.KillAll();
            ScreenManager.Instance.LoadMultiplayerScene();//ScreenManager.Instance.SwitchScreen(LobbyScreen.Instance, this);
            GameData.Instance.gameMode = global::Enums.GameModeType.Multiplayer;
        }

        void GoBackToHomeScreen()
        {

            DOTween.KillAll();
            Loader.Instance.ShowLoader();
            SceneManager.SwitchToHomeScene(_callback: (op) =>
            {
                ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
            });

            App app = FindObjectOfType<App>();
            app.ShowRoomOptionUI();
            Destroy(app.gameObject, 0.1f);
        }

        public void EnableShootButton(bool isAllowed)
        {
            //shootButton.gameObject.SetActive(isAllowed);
            //Debug.Log($"EnableShootButton {isAllowed}");
            shootButton.interactable = isAllowed;
            if (isAllowed)
            {
                shootButtonSprite.color = Color.white;
            }
            else
            {
                shootButtonSprite.color = Color.grey;
            }
        }

        // Method to update the weapon image on the shoot button
        public void UpdateWeaponImage(WeaponManager.WeaponInstallationType collectedWeapon)
        {
            if (collectedWeapon == WeaponManager.WeaponInstallationType.KETCHUP)
            {
                shootButtonSprite.sprite = ketchupSprite;
            }
            else if (collectedWeapon == WeaponManager.WeaponInstallationType.SUBMISSILE)
            {
                shootButtonSprite.sprite = submissileSprite;
            }
            else if (collectedWeapon == WeaponManager.WeaponInstallationType.OILSPILL)
            {
                shootButtonSprite.sprite = oilSpillSprite;
            }
        }

        public void LoadHomeScene()
        {
            StartCoroutine(LoadHomeSceneCoroutine());
        }

        private IEnumerator LoadHomeSceneCoroutine()
        {
            AsyncOperation asyncLoad = SceneManagerUnity.SceneManager.LoadSceneAsync("HomeScene", SceneManagerUnity.LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
            UnloadOtherScenes("HomeScene");
        }

        private void UnloadOtherScenes(string sceneToKeep)
        {
            for (int i = 0; i < SceneManagerUnity.SceneManager.sceneCount; i++)
            {
                SceneManagerUnity.Scene scene = SceneManagerUnity.SceneManager.GetSceneAt(i);
                if (scene.name != sceneToKeep)
                {
                    SceneManagerUnity.SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        private void SetJoy(RectTransform joy, RectTransform knob, bool active, Vector2 center, Vector2 current)
        {
            center /= _canvas.localScale.x;
            current /= _canvas.localScale.x;

            joy.gameObject.SetActive(active);
            joy.anchoredPosition = center;

            current -= center;
            if (current.magnitude > knob.rect.width / 2)
                current = current.normalized * knob.rect.width / 2;

            knob.anchoredPosition = current;
        }
    }

}