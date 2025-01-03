using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    [DefaultExecutionOrder(-10)]
    public class Loader : MonoBehaviour
    {
        [Header("-----Loader")]
        [SerializeField] private Image loadingImageMain;
        [SerializeField] private Image loadingImageTips;
        [SerializeField] private TMPro.TextMeshProUGUI loadingPercentageTMP;
        [SerializeField] private float loadingSpeed = 0.01f;
        [SerializeField] private Sprite[] sequence;
        private float _loadingTimer;
        private int seqIndex;


        [Header("-----Tips")]
        [SerializeField] private float duration = 5;
        [SerializeField] private Transform tipsParent, layoutParent;
        [SerializeField] private Button nextBtn, prevBtn;
        [SerializeField] private Vector2[] positions;
        [SerializeField] private RectTransform[] tipsRect;
        [SerializeField] private List<Tip> tipsData;

        [Header("-----Debug")]
        [SerializeField] private bool showTips = false;
        [SerializeField] private int initialTipIndex = 0;

        private int currentTipIndex;
        private float _tipsTimer;


        private CanvasGroup canvasGroup;

        public static Loader Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                canvasGroup = GetComponent<CanvasGroup>();
            }
            else
            {
                enabled = false;
            }
        }

        private void Start()
        {
            InitTips();
            InvokeHideLoader();
        }

        void Update()
        {
            if (canvasGroup.alpha > 0.1)
            {
                LoadingProgress();

                SwitchTips();
            }
        }

        private void LoadingProgress()
        {
            if (_loadingTimer > 0)
                _loadingTimer -= Time.deltaTime;
            else
            {
                seqIndex = (seqIndex + 1) % sequence.Length;
                if (showTips) loadingImageTips.sprite = sequence[seqIndex];
                else loadingImageMain.sprite = sequence[seqIndex];
                _loadingTimer = loadingSpeed;
            }
        }

        private void SwitchTips()
        {
            if (showTips && tipsParent.gameObject.activeSelf)
            {
                if (_tipsTimer > 0) _tipsTimer -= Time.deltaTime;
                else
                {
                    OnClick_Next();
                    _tipsTimer = duration;
                }
            }
        }

        [ContextMenu("ShowLoader")]
        private void Debug_ShowLoader() => ShowLoader(showTips);


        public void ShowLoader(bool _showTips = false)
        {
            _tipsTimer = duration;
            _loadingTimer = loadingSpeed;

            showTips = _showTips;
            tipsParent.gameObject.SetActive(_showTips);
            loadingImageTips.gameObject.SetActive(_showTips);
            loadingImageMain.gameObject.SetActive(!_showTips);
            loadingPercentageTMP.gameObject.SetActive(_showTips);

            loadingImageMain.sprite = loadingImageTips.sprite = sequence[0];

            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(true);
        }

        public void HideLoader()
        {
            showTips = false;
            Invoke("InvokeHideLoader", 0.5f);
        }

        private void InvokeHideLoader()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            //TweenHandler.CanvasGroupAlpha(canvasGroup, 0, ScreenManager.Instance.TweenDuration, ScreenManager.Instance.TweenEase, () =>
            //{
            //    gameObject.SetActive(false);
            //});
        }


        public void UpdateLoadingPercentage(float _value) => loadingPercentageTMP.text = $"Loading game <color=#BDFF00>{Mathf.FloorToInt(_value * 100)}%</color>";


        #region Tips
        [ContextMenu("InitTips")]
        private void InitTips()
        {
            initialTipIndex = Random.Range(0, layoutParent.childCount);
            for (int i = 0; i < layoutParent.childCount; i++)
            {
                tipsRect[i].GetComponent<TipPrefab>().SetTip(tipsData[i + initialTipIndex], true);
                tipsRect[i].anchoredPosition = positions[i];
            }
            currentTipIndex = initialTipIndex + 2;
        }

        [ContextMenu("OnClick_Next")]
        public void OnClick_Next()
        {
            //showTips = true;
            _tipsTimer = duration;
            nextBtn.interactable = prevBtn.interactable = false;

            // Next Tip
            layoutParent.GetChild(layoutParent.childCount - 1).GetComponent<TipPrefab>().SetTip(tipsData[(currentTipIndex + 2) % tipsData.Count]);


            // Child Index
            for (int i = 1; i < layoutParent.childCount; i++)
                layoutParent.GetChild(i).SetSiblingIndex(i - 1);


            // Transition
            for (int i = 0; i < layoutParent.childCount; i++)
            {
                layoutParent.GetChild(i).GetComponent<TipPrefab>().Refresh(i == 2);
                if (i == layoutParent.childCount - 1) layoutParent.GetChild(i).gameObject.SetActive(false);

                TweenHandler.UIPosition
                (
                    layoutParent.GetChild(i).GetComponent<RectTransform>(), positions[i], 1, DG.Tweening.Ease.InOutExpo, () =>
                     {
                         layoutParent.GetChild(layoutParent.childCount - 1).gameObject.SetActive(true);
                         nextBtn.interactable = prevBtn.interactable = true;
                         //showTips = false;
                     }
                );
            }


            currentTipIndex++;
            if (currentTipIndex == tipsData.Count) currentTipIndex = 0;
        }


        public void OnClick_Prev()
        {
            //showTips = true;
            _tipsTimer = duration;
            nextBtn.interactable = prevBtn.interactable = false;

            // Next Tip
            int _index = currentTipIndex == 1 ? _index = tipsData.Count - 1 : currentTipIndex == 0 ? _index = tipsData.Count - 2 : _index = currentTipIndex - 2;
            layoutParent.GetChild(0).GetComponent<TipPrefab>().SetTip(tipsData[_index]);


            // Child Index
            for (int i = layoutParent.childCount - 1; i > 0; i--)
                layoutParent.GetChild(i).SetSiblingIndex(i - 1);

            // Transition
            for (int i = 0; i < layoutParent.childCount; i++)
            {
                layoutParent.GetChild(i).GetComponent<TipPrefab>().Refresh(i == 2);
                if (i == 0) layoutParent.GetChild(i).gameObject.SetActive(false);

                TweenHandler.UIPosition
                (
                    layoutParent.GetChild(i).GetComponent<RectTransform>(), positions[i], 1, DG.Tweening.Ease.InOutExpo, () =>
                     {
                         layoutParent.GetChild(0).gameObject.SetActive(true);
                         nextBtn.interactable = prevBtn.interactable = true;
                         //showTips = false;
                     }
                );
            }


            currentTipIndex--;
            if (currentTipIndex == -1) currentTipIndex = tipsData.Count - 1;

        }


        public void OnClick_Hold() => showTips = true;
        #endregion



        //private void OnDisable() => CancelInvoke("InvokeHideLoader");
    }


    [System.Serializable]
    public class Tip
    {
        public Sprite thumbnail;
        [TextArea(1, 3)] public string info;
    }

}