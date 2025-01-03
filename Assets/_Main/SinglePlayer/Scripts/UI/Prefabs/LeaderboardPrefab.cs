using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class LeaderboardPrefab : MonoBehaviour
    {
        [SerializeField] private Image bg;
        [SerializeField] private Image frame;
        [SerializeField] private Image rankImage;
        [SerializeField] private RawImage profilePic;
        [SerializeField] private TextMeshProUGUI rankTMP;
        [SerializeField] private TextMeshProUGUI usernameTMP;
        [SerializeField] private TextMeshProUGUI munchesTMP;
        //[SerializeField] private TextMeshProUGUI brownieTMP;
        // [SerializeField] private TextMeshProUGUI dishPtsTMP;
        [SerializeField] private LeaderboardRewardsUIHandler leaderboardRewardsUIHandler;
        [SerializeField] private Sprite defaultProfile;
        [SerializeField] private Sprite rank1, rank2, rank3, rank1Border, rank2Border, rank3Border, baseBorder;


        public void SetData(ModelClass.LeaderboardPlayer _data, bool _isPlayer)
        {
            profilePic.gameObject.SetActive(false);
            rankTMP.text = $"{_data.rank}";
            usernameTMP.text = _isPlayer ? $"{_data.username} (You)" : _data.username;
            munchesTMP.text = _data.munch;
            SetRankIcon(_data.rank, _isPlayer);
            leaderboardRewardsUIHandler.SetData(chips: _data.chips, _isPlayer, cards: _data.dish);
            gameObject.SetActive(true);
            StartCoroutine(APIManager.GetTexture(_data.profilePic, _result =>
            {
                if (_result == null)
                {
                    Debug.Log($"[{_data.username}] Could not get profile pic!");
                    profilePic.texture = defaultProfile.texture;
                    profilePic.gameObject.SetActive(true);
                }
                else
                {
                    profilePic.texture = _result;
                    profilePic.gameObject.SetActive(true);
                }
            }));

            //  frame.color = _isPlayer ? ColorManager.Instance.Yellow : ColorManager.Instance.Cyan;

            gameObject.name = rankTMP.text;
        }

        private void SetRankIcon(int _rank, bool isplayer)
        {
            ResetLeaderBoardPrefab();

            switch (_rank)
            {
                case 1:
                    rankTMP.color = Color.black;
                    rankImage.sprite = rank1;
                    rankImage.enabled = true;
                    bg.sprite = rank1Border;
                    bg.color = Color.white;
                    usernameTMP.color = Color.white;
                    break;

                case 2:
                    rankTMP.color = Color.black;
                    rankImage.sprite = rank2;
                    rankImage.enabled = true;
                    bg.sprite = rank2Border;
                    break;

                case 3:
                    rankTMP.color = Color.black;
                    rankImage.sprite = rank3;
                    rankImage.enabled = true;
                    bg.sprite = rank3Border;
                    break;

                default:
                    bg.color = ColorManager.Instance.DarkerGrey;
                    break;
            }
            if (isplayer)
            {
                bg.sprite = baseBorder;
                bg.color = ColorManager.Instance.Cyan;
                rankTMP.color = Color.black;
                usernameTMP.color = Color.black;
                munchesTMP.color = Color.black;
                //  dishPtsTMP.color = Color.black;

            }
        }

        private void ResetLeaderBoardPrefab()
        {
            rankImage.enabled = false;
            bg.color = Color.white;
            rankTMP.color = Color.white;
            usernameTMP.color = Color.white;
            munchesTMP.color = Color.yellow;
            //  dishPtsTMP.color = Color.white;
            bg.sprite = baseBorder;
        }

        private void OnDisable() => gameObject.SetActive(false);

    }

}