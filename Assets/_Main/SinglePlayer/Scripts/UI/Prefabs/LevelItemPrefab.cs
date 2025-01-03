using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class LevelItemPrefab : MonoBehaviour
    {
        //public static Action<int> OnLevelChanged;

        [SerializeField] private Image frameImage;
        [SerializeField] private Image starsBG;
        [SerializeField] private GameObject statsObj;
        [SerializeField] private GameObject lockObj;

        [Header("-----Level Info")]
        [SerializeField] private TextMeshProUGUI levelTMP;
        [SerializeField] private TextMeshProUGUI chipsTMP;
        [SerializeField] private TextMeshProUGUI munchesTMP;
        [SerializeField] private Image starsFilled;

        [Header("-----Others")]
        [SerializeField] private Sprite starsBlueBG;
        [SerializeField] private Sprite starsGreyBG;

        [field: SerializeField] public ModelClass.PlayerLevelStats Level { get; private set; }
        [field: SerializeField] public bool IsCurrentLevel { get; private set; }

        private Button frameBtn;
        private void Awake() => frameBtn = frameImage.GetComponent<Button>();


        public void OnClick_Select()
        {
            if (AddressableLoader.isMapLoading)
            {
                OverlayWarningPopup.Instance.ShowWarning("Map Already loading. Please wait!!!");
                return;
            }
            AddressableLoader.isMapLoading = true;
            GameData.Instance.SelectedLevelNumber = Level.level;
            LevelScreen.Instance.PlayerLevel();
        }

        public void SetLevelData(ModelClass.PlayerLevelStats _playerLevel, bool _isCurrentLevel, bool _hasPlayed)
        {
            Level = _playerLevel;
            IsCurrentLevel = _isCurrentLevel;
            levelTMP.text = _playerLevel.level.ToString();
            chipsTMP.text = _playerLevel.chips.ToString();
            munchesTMP.text = _playerLevel.munches.ToString();
            starsFilled.fillAmount = _playerLevel.stars;

            levelTMP.color = _isCurrentLevel ? ColorManager.Instance.Blue : Color.white;
            frameImage.color = _hasPlayed ? Color.black : _isCurrentLevel ? ColorManager.Instance.Cyan : ColorManager.Instance.DarkGrey;
            starsBG.sprite = _hasPlayed || _isCurrentLevel ? starsBlueBG : starsGreyBG;


            statsObj.SetActive(_hasPlayed);
            lockObj.SetActive(!_hasPlayed && !_isCurrentLevel);
            frameBtn.interactable = _hasPlayed || _isCurrentLevel;


            gameObject.name = _playerLevel.level.ToString();
        }





    }
}