using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
	public class CameraScreenFXBehaviour : MonoBehaviour
	{
		Kino.DigitalGlitch _glitchEffect;
		[SerializeField] private float durationToTarget = 0.3f;
		float _timer = 0;

		bool _active = false;
		private float _maxGlitch = 1;

		

		void Start()
		{
			_glitchEffect = GetComponent<Kino.DigitalGlitch>();
			_glitchEffect.enabled = false;
		}

		public void ToggleGlitch(bool value)
		{
			_active = value;
		}
		
		void Update()
		{
			float direction = _active ? 1 : -1;
			if ((_timer > 0 && direction == -1) || (_timer < durationToTarget && direction == 1))
			{
				_timer = Mathf.Clamp(_timer + Time.deltaTime * direction, 0, durationToTarget);
				float t = _timer / durationToTarget;
				_glitchEffect.intensity = Mathf.Lerp(0, _maxGlitch, t);

				if (_timer == 0 && direction == -1 && _glitchEffect.enabled)
				{
					_glitchEffect.enabled = false;
				}
				else if (direction == 1 && !_glitchEffect.enabled)
				{
					_glitchEffect.enabled = true;
				}
			}
		}
	}
}