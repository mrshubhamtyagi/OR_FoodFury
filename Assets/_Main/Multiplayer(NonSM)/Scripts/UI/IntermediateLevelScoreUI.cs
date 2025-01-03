 using System.Collections;

using UnityEngine;
using TMPro;

namespace OneRare.FoodFury.Multiplayer
{
	public class IntermediateLevelScoreUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _roundNumber;
		//[SerializeField] private AudioEmitter _audioEmitter;

		private bool _active;

		public void Initialize()
		{
			/*Color scoreColor = player.PlayerMaterial.GetColor("_SilhouetteColor");
			_score.color = scoreColor;*/
		}
		public void ShowRoundMessage(string message)
		{
			_roundNumber.text = message;
			Invoke("HideInterimResultScreen", 3f);
		}
		public void ShowRoundStart()
		{
			_roundNumber.text = "Initiation Round!";
			Invoke("HideInterimResultScreen",3f);
		}
		
		public void ShowRoundOver(int roundNumber)
		{
			_roundNumber.text = $"Round {roundNumber} Over!";
			Invoke("HideInterimResultScreen",3f);
		}
		void HideInterimResultScreen()
		{
			Destroy(gameObject);
		}
	}
}