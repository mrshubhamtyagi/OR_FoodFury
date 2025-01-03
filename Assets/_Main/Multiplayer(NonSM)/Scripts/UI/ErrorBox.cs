using System;
using DG.Tweening;
using FoodFury;
using FusionExamples.UIHelpers;
using FusionExamples.Utility;
using OneRare.FoodFury.Multiplayer;
using TMPro;
using UnityEngine;

namespace Tanknarok.UI
{
	[RequireComponent(typeof(Panel))]
	public class ErrorBox : MonoBehaviour
	{
		[SerializeField] private TMP_Text _header;
		[SerializeField] private TMP_Text _message;

		private Action _onClose;
		private Panel _panel;

		public static void Show(string header, string message, Action onclose)
		{
			/*if (header == "Disconnected!")
			{
				Singleton<ErrorBox>.Instance.ShowInternalAlt(header, message, onclose);
				
			}*/
			Singleton<ErrorBox>.Instance.ShowInternal(header, message, onclose);
		}

		private void ShowInternal(string header, string message, Action onclose)
		{
			_header.text = header;
			_message.text = message;
			_onClose = onclose;
			if(_panel==null)
				_panel = GetComponent<Panel>();
			_panel.SetVisible(true);
		}
		
		private void ShowInternalAlt(string header, string message, Action onclose)
		{
			_header.text = header;
			_message.text = message;
			_onClose = onclose;
			if(_panel==null)
				_panel = GetComponent<Panel>();
			_panel.SetVisible(false);
			
			CloseErrorBox();
		}

		public static void CloseErrorBox()
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
			Loader.Instance.HideLoader();
			//_panel.SetVisible(false);
		}
		private MobileInput mobileInput;
		public void OnClose()
		{
			/*if (Runner != null)
			{
				Runner.Shutdown(true);
			}*/

			// Load the HomeScene
			DOTween.KillAll();
			Loader.Instance.ShowLoader();
			SceneManager.SwitchToHomeScene(_callback: (op) =>
			{
				ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
			});
           

			App app = FindObjectOfType<App>();
			app.ShowRoomOptionUI();
			Destroy(app.gameObject, 0.1f);
			_panel.SetVisible(false);
			_onClose();
		}

		public void OnReplay()
		{
			DOTween.KillAll();
			Loader.Instance.ShowLoader();
			/*SceneManager.SwitchToHomeScene(_callback: (op) =>
			{
				ScreenManager.Instance.SwitchScreen(HomeScreen.Instance, null);
			});*/
			App app = FindObjectOfType<App>();
			app.ShowRoomOptionUI();
			Destroy(app.gameObject, 0.1f);
			_panel.SetVisible(false);
			_onClose();
		}
	}
}