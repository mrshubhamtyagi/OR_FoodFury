using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace OneRare.FoodFury.Multiplayer
{

    public class BounceUI : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _bounceCurve;
        [SerializeField] private float _animCurveWeight = 1f;
        private Vector3 _defaultScale;
        private Transform _rect;

        private float _timer = 0;
        [SerializeField] private float _bounceDuration = 0.5f;
        private float _targetDuration;

        [SerializeField] private float _minDelay = 1f;
        [SerializeField] private float _randomOffset = 0.5f;

        [SerializeField] private bool _bounceWithTime = false;
        private bool _caughtUpWithTime = false;

        private float _randomDelay = 0;

        // New variables for animation toggles
        [SerializeField] private bool useBounce = true;
        [SerializeField] private bool useRotate = false;

        // Use this for initialization
        void Start()
        {
            _rect = GetComponent<RectTransform>();
            if (_rect == null)
            {
                Debug.LogError("RectTransform component is missing. Please ensure this script is attached to a UI element.");
                return;
            }

            _defaultScale = _rect.localScale;
            _timer = _bounceDuration;
            _caughtUpWithTime = false;
            StartCoroutine(Wait());

            if (useRotate && _rect != null)
            {
                Rotate();
            }
        }



        void UpdateTimer()
        {
            _timer += Time.deltaTime;
            _timer = Mathf.Min(_timer, _bounceDuration);
        }

        void Bounce()
        {
            if (_rect == null) return;

            float t = _timer / _bounceDuration;
            float multiplier = _bounceCurve.Evaluate(t) * _animCurveWeight;

            _rect.localScale = _defaultScale + Vector3.one * multiplier;
        }

        void SetRandomDelay()
        {
            _randomDelay = _minDelay + Random.Range(0, _randomOffset);
        }

        IEnumerator Wait()
        {
            while (gameObject.activeSelf)
            {
                SetRandomDelay();
                yield return new WaitForSeconds(_randomDelay);
                if (_bounceWithTime && !_caughtUpWithTime)
                {
                    _timer = _bounceDuration;
                    float currentTime = Time.time;
                    int ceiledCurrentTime = Mathf.CeilToInt(currentTime);
                    float fractionTime = ceiledCurrentTime - currentTime;

                    float modFracTime = fractionTime % _bounceDuration;
                    _timer = modFracTime;
                    _caughtUpWithTime = true;
                }
                else
                {
                    _timer = 0;
                }

                yield return new WaitForSeconds(_bounceDuration);
            }
        }

        void Update()
        {
            if (useBounce)
            {
                UpdateTimer();
                Bounce();
            }
        }

        void Rotate()
        {
            //Debug.Log("Rotate called");
            if (_rect != null)
            {
                _rect.DORotate(new Vector3(0, 0, 15), 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            }
            else
            {
                Debug.LogError("RectTransform is null!");
            }
        }

    }

}