using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class Booster : MonoBehaviour
    {
        public static event Action<Booster> OnBoosterCollected;

        [SerializeField] private ParticleSystem paricleCircle;
        [SerializeField] private ParticleSystem pariclesSphere;

        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI description;

        [field: SerializeField] public Tile Tile { get; private set; }
        [field: SerializeField] public ModelClass.BoosterData BoosterData { get; private set; }

        [SerializeField] private SoundSO collectSound;
        [SerializeField] private float _incrementValue;
        private WaitForSeconds waitForSec;

        void Start()
        {
            float _wait = 0.1f;
            waitForSec = new WaitForSeconds(_wait);
            _incrementValue = BoosterData.delay <= 0 ? 0 : 1 / (BoosterData.delay / _wait);

            //  Invoke("PauseParticles", 2);
        }
        void PauseParticles()
        {
            paricleCircle.Pause();
            pariclesSphere.Pause();
        }

        private void OnEnable()
        {
            GameController.OnLevelFailed += OnLevelFailed;
            GameController.OnLevelComplete += OnLevelFailed;
        }

        private void OnDisable()
        {
            GameController.OnLevelFailed -= OnLevelFailed;
            GameController.OnLevelComplete -= OnLevelFailed;
        }

        private void OnLevelFailed() => Invoke("DestroyBooster", 0.1f);

        public void SetData(ModelClass.BoosterData _booster, Tile _tile)
        {
            BoosterData = _booster;
            Tile = _tile;
            if (Tile != null) Tile.hasBooster = true;

            description.text = _booster.description;

            transform.parent = null;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            transform.position = _tile.GetCenterTop();
        }

        public void Collect()
        {
            AudioUtils.PlayOneShotAudio(collectSound, transform.position);
            OnBoosterCollected?.Invoke(this);
            GetComponent<Collider>().enabled = false;
            BoosterManager.Instance.SpawnBooster();
            Invoke("DestroyBooster", 0.1f);
        }

        public void StopTimer()
        {
            StopCoroutine("Co_Timer");
            fillImage.fillAmount = 0;
        }

        private IEnumerator Co_Timer()
        {
            while (true)
            {
                yield return waitForSec;
                fillImage.fillAmount += _incrementValue;

                if (fillImage.fillAmount >= 1)
                {
                    OnBoosterCollected?.Invoke(this);
                    Invoke("DestroyBooster", 0.1f);
                    yield break;
                }
            }
        }

        public void DestroyBooster()
        {
            StopCoroutine("Co_Timer");
            if (Tile != null) Tile.hasBooster = false;
            BoosterManager.Instance.Remove(this);
            Destroy(gameObject);
        }

    }

}