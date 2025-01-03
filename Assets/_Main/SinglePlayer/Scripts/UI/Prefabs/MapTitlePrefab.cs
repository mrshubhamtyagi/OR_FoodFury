using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class MapTitlePrefab : MonoBehaviour
    {
        //private static Action<int> OnMapSwitch; // Used to Select and Deselect Item

        [SerializeField] private TextMeshProUGUI mapNameTMP;
        [SerializeField] private RawImage thumbnail;
        [SerializeField] private GameObject frame;
        [field: SerializeField] public ModelClass.MapDetail Data { get; private set; }
        [Header("Lock Parameters")]
        [SerializeField] private Vector2 widthHeightOfLock;
        [SerializeField] private Vector3 positionOffset;
        public Texture2D defaultMapIcon;
        private Button btn;

        private void Awake() => btn = mapNameTMP.GetComponent<Button>();

        public void SetData(ModelClass.MapDetail _data, bool _isLocked)
        {
            Data = _data;
            mapNameTMP.text = _data.mapName;

            if (_isLocked)
                thumbnail.texture = SpriteManager.Instance.lockSprite.texture;
            else
                StartCoroutine(APIManager.GetMapTexture(_data.thumbnailUrl, true, (_status, _texture) =>
                {
                    if (_status) thumbnail.texture = _texture;
                    else if (PopUpManager.Instance != null)
                    {
                        //OverlayWarningPopup.Instance.SetWarning($"Could not load Map Icon! [{_data.mapName}]");
                        Debug.Log($"Could not load Map Icon! [{_data.mapName}]");
                        thumbnail.texture = defaultMapIcon;
                    }
                }));

            thumbnail.color = _isLocked ? ColorManager.Instance.DarkGrey : Color.white;
            if (_isLocked)
            {
                RectTransform rectTransform = thumbnail.GetComponent<RectTransform>();
                rectTransform.sizeDelta = widthHeightOfLock;
                Vector3 position = rectTransform.localPosition + positionOffset;
                rectTransform.localPosition = position;
            }
            btn.interactable = !_isLocked;
            gameObject.name = _data.mapName;
        }

        [ContextMenu("Select")]
        public void OnClick_Select()
        {
            btn.enabled = false;
            frame.SetActive(true);
            mapNameTMP.color = Color.black;
            GameData.Instance.SelectedMapId = Data.mapId;
            LevelScreen.Instance.SetMapInfo(this);
        }

        public void Deselect()
        {
            btn.enabled = true;
            frame.SetActive(false);
            mapNameTMP.color = Color.white;
        }
        public Texture GetThumnailTexture() => thumbnail.texture;

    }

}