using System;
using System.Collections.Generic;
using System.Linq;
using FoodFury;
using Fusion;
using FusionHelpers;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class ReadyUpManager : MonoBehaviour
    {
        [SerializeField] private GameObject _disconnectInfoText;
        [SerializeField] private GameObject _readyupInfoText;
        [SerializeField] private Transform _readyUIParent;
        [SerializeField] private ReadyupIndicator _readyPrefab;
        [SerializeField] private AudioEmitter _audioEmitter;

        private Dictionary<PlayerRef, ReadyupIndicator> _readyUIs = new Dictionary<PlayerRef, ReadyupIndicator>();
        private float _delay;
        private IEnumerable<FusionPlayer> _allPlayers;

        [SerializeField] private bool startingMatch = false;
        public void UpdateUI(GameManager.PlayState playState, IEnumerable<FusionPlayer> allPlayers, Action onAllPlayersReady)
        {
            if (_delay > 0)
            {
                _delay -= Time.deltaTime;
                return;
            }

            if (startingMatch)
            {
                _delay = 1f;
                foreach (FusionPlayer fusionPlayer in allPlayers)
                {
                    Player player = (Player)fusionPlayer;
                    if (!player.IsMapLoaded && !player.IsBot) return;
                }
                onAllPlayersReady();
                return;
            }

            _readyUIParent.gameObject.SetActive(AddressableLoader.MapLoadStatus == AddressableLoader.LoadSceneResult.Success);

            if (playState != GameManager.PlayState.LOBBY)
            {
                foreach (ReadyupIndicator ui in _readyUIs.Values)
                    LocalObjectPool.Release(ui);
                _readyUIs.Clear();
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            int playerCount = 0, readyCount = 0;
            if (!pressStartGame)
            {
                foreach (FusionPlayer fusionPlayer in allPlayers)
                {
                    Player player = (Player)fusionPlayer;
                    if (player.Ready) readyCount++;

                    playerCount++;
                    if (playerCount == 1)
                    {
                        PressStartGameButton();
                    }
                }
            }


            foreach (ReadyupIndicator ui in _readyUIs.Values)
            {
                ui.Dirty();
            }

            foreach (FusionPlayer fusionPlayer in allPlayers)
            {
                Player player = (Player)fusionPlayer;

                ReadyupIndicator indicator;
                if (!_readyUIs.TryGetValue(player.PlayerId, out indicator))
                {
                    indicator = LocalObjectPool.Acquire(_readyPrefab, Vector3.zero, Quaternion.identity, _readyUIParent);
                    _readyUIs.Add(player.PlayerId, indicator);
                }

                if (indicator.Refresh(player))
                {

                }
                //_audioEmitter.PlayOneShot();
            }

            if (pressStartGame)
            {
                if (allPlayers.Count() == 4)
                {
                    _delay = 0.1f;
                    startingMatch = true;
                    //Player.Local.HideRoomPlayerUI();

                    //Loader.Instance.ShowLoader();
                    //_delay = 0.7f;
                    //foreach (FusionPlayer fusionPlayer in allPlayers)
                    //{
                    //    Player player = (Player)fusionPlayer;
                    //    Player.Local.HideRoomPlayerUI();
                    //    //player.HideRoomPlayerUI();
                    //}
                    //onAllPlayersReady();
                }
            }
            bool allPlayersReady = readyCount == playerCount;

            _disconnectInfoText.SetActive(!allPlayersReady);
            _readyupInfoText.SetActive(!allPlayersReady);


            //AllPlayerReadyCase(allPlayersReady);

        }


        private void AllPlayerReadyCase(bool _allPlayersReady)
        {
            if (_allPlayersReady)
            {
                _delay = 0.4f;
                Player.Local.HideRoomPlayerUI();
                startingMatch = true;
                //foreach (FusionPlayer fusionPlayer in allPlayers)
                //{
                //    Player player = (Player)fusionPlayer;
                //    Player.Local.HideRoomPlayerUI();
                //    //player.HideRoomPlayerUI();
                //}
            }

        }



        bool pressStartGame = false;
        public void PressStartGameButton()
        {
            pressStartGame = true;
        }
        int _playerCount = 0;
        /*private void Update()
		{
			if (!pressStartGame)
			{
				return;
			}

			var b = _playerCount == _allPlayers.Count();

			if(_allPlayers.Count() < 3)
				return;
			_delay = 0.2f;
			foreach (FusionPlayer fusionPlayer in _allPlayers)
			{
				Player player = (Player) fusionPlayer;
				Player.Local.HideRoomPlayerUI();
				player.Ready = true;
				//player.HideRoomPlayerUI();
			}
			//onAllPlayersReady();
		}*/
    }
}