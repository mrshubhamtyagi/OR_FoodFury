using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using static FoodFury.ModelClass;

namespace FoodFury
{
    public class LeaderboardPopup : PopupBehaviour
    {
        [SerializeField] private LeaderboardPrefab leaderboardPrefab;
        [SerializeField] private RectTransform contentParent;
        [SerializeField] private RectTransform scrollView;
        [SerializeField] private Enums.SortBy sortBy = Enums.SortBy.Munches;
        [SerializeField] private ScrollToIndex scrollToIndex;
        // [SerializeField] private Image[] sorts;
        [SerializeField] private LeaderboardPlayer[] leaderbardData;


        void Awake() => Init(GetComponent<CanvasGroup>());

        public override void Init()
        {
            base.Init();
            MenuBar.Instance.BlockRaycasts(false);
            Loader.Instance.ShowLoader();

            sortBy = Enums.SortBy.Munches;
            //  for (int i = 0; i < sorts.Length; i++) sorts[i].enabled = i == 0 ? true : false;
            GetLeaderboardData_API();
        }

        public void OnClick_SortBy(int _index)
        {
            //  for (int i = 0; i < sorts.Length; i++)
            //   sorts[i].enabled = i == _index ? true : false;

            sortBy = (Enums.SortBy)_index;
            SetLeaderBoardData();
        }


        [ContextMenu("SetLeaderBoardData")]
        public void SetLeaderBoardData()
        {
            int _childCount = contentParent.childCount;
            for (int i = 0; i < _childCount; i++)
                contentParent.GetChild(i).gameObject.SetActive(false);


            List<ModelClass.LeaderboardPlayer> _data = leaderbardData.ToList();
            bool isPlayer = false;
            int playerIndex = -1;
            for (int i = 0; i < _data.Count; i++)
            {

                isPlayer = _data[i].userId.Equals(GameData.Instance.PlayerId);
                if (isPlayer)
                {
                    playerIndex = i;
                }

                if (i < _childCount)
                    contentParent.GetChild(i).GetComponent<LeaderboardPrefab>().SetData(_data[i], isPlayer);
                else
                    Instantiate(leaderboardPrefab, contentParent).SetData(_data[i], isPlayer);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent);
            // scrollView.GetComponent<ScrollRect>().StopMovement();
            //  contentParent.anchoredPosition = Vector2.zero;
            scrollToIndex.ScrollToLevelIndex(playerIndex);
            Loader.Instance.HideLoader();
        }

        private List<ModelClass.LeaderboardPlayer> GetLeaderboardSortedData(Enums.SortBy _order)
        {
            //return _order switch
            //{
            //    //Enums.SortBy.Brownie => leaderbardData.BROWNIE,
            //    Enums.SortBy.Munches => leaderbardData.MUNCH,
            //    Enums.SortBy.DishPTS => leaderbardData.DISH,
            //    _ => leaderbardData.MUNCH
            //};
            return null;
        }



        [ContextMenu("GetLeaderboardData")]
        private async void GetLeaderboardData_API()
        {
            var _response = await APIManager.GetLeaderboardDataAsync(GameData.Instance.PlayerId);
            if (_response.error)
            {
                PopUpManager.Instance.ShowWarningPopup("Could not fetch leaderboard!");
                Loader.Instance.HideLoader();
            }

            Debug.Log(_response.result);
            ErrorAndResultResponse<LeaderboardPlayer[]> _data = JsonConvert.DeserializeObject<ErrorAndResultResponse<LeaderboardPlayer[]>>(_response.result);
            if (_data.error)
            {
                PopUpManager.Instance.ShowWarningPopup("Could not fetch leaderboard!");
                Loader.Instance.HideLoader();
            }
            else
            {
                leaderbardData = _data.result;
                SetLeaderBoardData();
            }
        }



        public override void Close()
        {
            MenuBar.Instance.BlockRaycasts(true);
            base.Close();
        }
    }
}