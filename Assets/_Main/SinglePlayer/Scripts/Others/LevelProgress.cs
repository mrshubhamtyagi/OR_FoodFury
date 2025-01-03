using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class LevelProgress : MonoBehaviour
    {
        [SerializeField] private int duration = 2;
        [SerializeField] private DG.Tweening.Ease ease;


        [Header("-----Rider")]
        [SerializeField] private RectTransform rider;
        [SerializeField] private Image riderImage;
        [SerializeField] private Sprite[] riderL2R;
        [SerializeField] private Sprite[] riderT2B;

        [Header("-----Positions")]
        [SerializeField] private Vector2 rider1_2Start;
        [SerializeField] private Vector2 rider1_2End;
        [Space(10)]
        [SerializeField] private Vector2 rider2_3Start;
        [SerializeField] private Vector2 rider2_3End;
        [Space(10)]
        [SerializeField] private Vector2 rider3_4Start;
        [SerializeField] private Vector2 rider3_4End;

        [Space(20)]
        [SerializeField] private TextMeshProUGUI[] numbers;

        [Header("-----Debug")]
        public int _levelIndex = 1;
        public int _mod = 1;
        public int vehicleId = 1;

        //void Start()
        //{
        //    gameObject.SetActive(false);
        //}

        [ContextMenu("PlayLevelAnimation")]
        public void PlayLevelAnimation()
        {
            vehicleId = GameData.Instance.PlayerData.Data.currentVehicle;

            _mod = GameController.Instance ? GameData.Instance.SelectedLevelNumber % 3 : _levelIndex % 3;

            SetRiderPosition(_mod);
            SetNumbers(GameController.Instance ? GameData.Instance.SelectedLevelNumber : _levelIndex);

            gameObject.SetActive(true);

            switch (_mod)
            {
                case 1:
                    TweenHandler.UIPosition(rider, rider1_2End, duration, ease, () => { gameObject.SetActive(false); });
                    break;

                case 2:
                    TweenHandler.UIPosition(rider, rider2_3End, duration, ease, () => { gameObject.SetActive(false); });
                    break;

                case 0:
                    TweenHandler.UIPosition(rider, rider3_4End, duration, ease, () => { gameObject.SetActive(false); });
                    break;
            }
        }


        private void SetRiderPosition(int _level)
        {
            if (riderImage == null) riderImage = rider.GetComponent<Image>();
            switch (_level)
            {
                case 1:
                    rider.anchoredPosition = rider1_2Start;
                    riderImage.sprite = riderL2R[vehicleId - 1];
                    break;

                case 2:
                    rider.anchoredPosition = rider2_3Start;
                    riderImage.sprite = riderT2B[vehicleId - 1];
                    break;

                case 0:
                    rider.anchoredPosition = rider3_4Start;
                    riderImage.sprite = riderL2R[vehicleId - 1];
                    break;
            }
        }


        public int _startingNum = 1;
        [ContextMenu("SetNumbers")]
        private void SetNumbers(int _level)
        {
            if (_level % 3 == 1) _startingNum = _level;
            else if (_level % 3 == 2) _startingNum = _level - 1;
            else if (_level % 3 == 0) _startingNum = _level - 2;

            for (int i = 0; i < numbers.Length; i++)
                numbers[i].text = (_startingNum++).ToString();
        }
    }

}