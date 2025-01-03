using FoodFury;
using Fusion;
using FusionExamples.UIHelpers;
using FusionHelpers;
using Tanknarok.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace OneRare.FoodFury.Multiplayer
{
	/// <summary>
	/// App entry point and main UI flow management.
	/// </summary>
	public class App : MonoBehaviour
	{
		[SerializeField] private LevelManager levelManager;
		[SerializeField] private GameManager gameManagerPrefab;
		[SerializeField] private TMP_InputField room;
		[SerializeField] private TextMeshProUGUI progress;
		[SerializeField] private Panel uiCurtain;
		[SerializeField] private Panel uiStart;
		[SerializeField] private Panel uiProgress;
		[SerializeField] private Panel uiRoom;
		[SerializeField] private GameObject uiGame;
		
		//[SerializeField] private MenuBar menuBar;
		
		private FusionLauncher.ConnectionStatus _status = FusionLauncher.ConnectionStatus.Disconnected;
		private GameMode _gameMode;
		private int _nextPlayerIndex;
		private string _roomName;
		private void Awake()
		{
			DontDestroyOnLoad(this);
			levelManager.onStatusUpdate = OnConnectionStatusUpdate;
		}
 
		private void Start()
		{
			OnConnectionStatusUpdate( null, FusionLauncher.ConnectionStatus.Disconnected, "");
			GameData.Invoke_OnPlayerDataUpdate();
			
			Invoke("OnSharedOptions",0.05f);
			
			//Invoke("SetGameMode",0.3f);
		}

		private void Update()
		{
			if (uiProgress.isShowing)
			{
				if (Keyboard.current[Key.Escape].wasPressedThisFrame)
				{
					NetworkRunner runner = FindObjectOfType<NetworkRunner>();
					if (runner != null && !runner.IsShutdown)
					{
						// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
						runner.Shutdown(false);
					}
				}
				UpdateUI();
			}
		}

		// What mode to play - Called from the start menu
		public void OnHostOptions()
		{
			SetGameMode(GameMode.Host);
		}

		public void OnJoinOptions()
		{
			SetGameMode(GameMode.Client);
		}

		public void OnSharedOptions()
		{
			SetGameMode(GameMode.Shared);
		}

		private void SetGameMode(GameMode gamemode)
		{
			_gameMode = gamemode;
			FusionLauncher.Launch(_gameMode,null, gameManagerPrefab, levelManager, OnConnectionStatusUpdate);
			Loader.Instance.ShowLoader();
		}

		public void OnEnterRoom()
		{
			/*if (GateUI(uiRoom))
			{
				_roomName = room.text;
				FusionLauncher.Launch(_gameMode,null, gameManagerPrefab, levelManager, OnConnectionStatusUpdate);
			}*/
			FusionLauncher.Launch(_gameMode,null, gameManagerPrefab, levelManager, OnConnectionStatusUpdate);
			//menuBar.Hide();
		
		}

		public void ShowRoomOptionUI()
		{
			uiRoom.SetVisible(true);
		}
		
		
		public void ReEnterRoom()
		{
			NetworkRunner runner = FindObjectOfType<NetworkRunner>();
			if (runner != null && !runner.IsShutdown)
			{
				// Calling with destroyGameObject false because we do this in the OnShutdown callback on FusionLauncher
				runner.Shutdown(false);
			}
			//UpdateUI();
			//FusionLauncher.Launch(_gameMode, _roomName, gameManagerPrefab, levelManager, OnConnectionStatusUpdate);
		}

		/// <summary>
		/// Call this method from button events to close the current UI panel and check the return value to decide
		/// if it's ok to proceed with handling the button events. Prevents double-actions and makes sure UI panels are closed. 
		/// </summary>
		/// <param name="ui">Currently visible UI that should be closed</param>
		/// <returns>True if UI is in fact visible and action should proceed</returns>
		private bool GateUI(Panel ui)
		{ 
			if (!ui.isShowing)
				return false;
			ui.SetVisible(false);
			return true;
		}

		private void OnConnectionStatusUpdate(NetworkRunner runner, FusionLauncher.ConnectionStatus status,
			string reason)
		{
			if (!this)
				return;
			
			if (status != _status)
			{
				switch (status)
				{
					case FusionLauncher.ConnectionStatus.Disconnected:
						ErrorBox.Show("Disconnected!", reason, () => { });
						break;
					case FusionLauncher.ConnectionStatus.Failed:
						ErrorBox.Show("Error!", reason, () => { });
						break;
				}
			}
			_status = status;
			UpdateUI();
			
		}

		private void UpdateUI()
		{
			bool intro = false;
			bool progress = false;
			bool running = false;

			switch (_status)
			{
				case FusionLauncher.ConnectionStatus.Disconnected:
					this.progress.text = "Disconnected!";
					intro = true;
					break;
				case FusionLauncher.ConnectionStatus.Failed:
					this.progress.text = "Failed!";
					intro = true;
					break;
				case FusionLauncher.ConnectionStatus.Connecting:
					this.progress.text = "Connecting...";
					progress = true;
					break;
				case FusionLauncher.ConnectionStatus.Connected:
					this.progress.text = "Connected!";
					progress = true;
					Loader.Instance.HideLoader();
					break;
				case FusionLauncher.ConnectionStatus.Loading:
					this.progress.text = "Loading...";
					progress = true;
					break;
				case FusionLauncher.ConnectionStatus.Loaded:
					running = true;
					break;
			}

			//uiCurtain.SetVisible(!running);
			uiStart.SetVisible(intro);
			uiProgress.SetVisible(progress);
			uiGame.SetActive(running);
            //Adding a comment to test somethihng
            
			
		}
	}
}