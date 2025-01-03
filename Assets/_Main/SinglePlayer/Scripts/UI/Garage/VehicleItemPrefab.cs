using System;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class VehicleItemPrefab : MonoBehaviour
    {
        [SerializeField] private GameObject lockIcon, tickIcon;
        [SerializeField] private GameObject status;
        [SerializeField] private RawImage thumbnail;

        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public bool IsPlayerVehicle { get; private set; }
        [field: SerializeField] public bool IsSelected { get; private set; }
        [field: SerializeField] public bool IsLocked { get; private set; }

        private RectTransform rectTransform;
        private Image bg;

        private Vector2 selectedSize => new Vector2(391, 230);
        private Vector2 deselectedSize => new Vector2(306, 180);


        void Awake()
        {
            bg = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }

        //private void Start()
        //{
        //    if (transform.GetSiblingIndex() == 0) GarageScreen.Instance.OnVehiclePreview(this);
        //}

        private void OnEnable() => GarageScreen.OnVehicleSelected += OnVehicleChanged;

        private void OnDisable() => GarageScreen.OnVehicleSelected -= OnVehicleChanged;


        private void OnVehicleChanged(int _id)
        {
            tickIcon.SetActive(_id == Id);
            IsSelected = _id == Id;
            IsPlayerVehicle = GameData.Instance.PlayerData.Data.currentVehicle == Id;
        }

        public void SetDetails(ModelClass.GarageVehicleData _data, bool _isPlayerVehicle, bool _isSelected, bool _isLocked, Action _callback = null)
        {
            Id = _data.id;
            IsPlayerVehicle = _isPlayerVehicle;
            IsSelected = _isSelected;
            IsLocked = _isLocked;

            lockIcon.SetActive(_isLocked);
            tickIcon.SetActive(_isSelected);

            bg.color = IsPlayerVehicle ? ColorManager.Instance.Cyan : IsLocked ? ColorManager.Instance.DarkGrey : Color.white;
            rectTransform.sizeDelta = _isSelected ? selectedSize : deselectedSize;
            if (GarageScreen.Instance.hasVehicleThumbnail(_data.id))
            {
                var vehicleTexture = GarageScreen.Instance.VehicleThumbnailDictionary[_data.id];
                _data.thumbnail = vehicleTexture;
                thumbnail.texture = vehicleTexture;
                thumbnail.enabled = true;
                _callback?.Invoke();
            }
            else
            {
                StartCoroutine(APIManager.GetVehicleTexture(_data.thumbnailUrl, (_success, _texture) =>
                {
                    if (_success)
                    {
                        GarageScreen.Instance.AddVehicleImage(_data.id, _texture);
                        _data.thumbnail = _texture;
                        thumbnail.texture = _texture;
                        thumbnail.enabled = true;
                        _callback?.Invoke();
                    }
                    else Debug.Log($"Could not load thumbnail [{_data.thumbnailUrl}]");
                }));
            }

        }


        public void OnClick_Item() => GarageScreen.Instance.OnVehiclePreview(this);


        public void Unlock()
        {
            IsLocked = false;
            lockIcon.SetActive(false);
        }


        public void Select()
        {
            bg.color = ColorManager.Instance.Cyan;
            rectTransform.sizeDelta = selectedSize;
        }


        public void Deselect()
        {
            bg.color = IsLocked ? ColorManager.Instance.DarkGrey : Color.white;
            rectTransform.sizeDelta = deselectedSize;
        }

    }

}