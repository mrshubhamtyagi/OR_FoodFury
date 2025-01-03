using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class WeaponItem : MonoBehaviour
    {
        public static event Action<Weapon, bool> OnWeaponCollected;
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [SerializeField] private ParticleSystem paricleCircle;
        [SerializeField] private ParticleSystem pariclesSphere;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private SoundSO collectSoundSO;

        private Weapon weapon;
        private bool weaponRemovedAlready;
        [field: SerializeField] public Tile Tile { get; private set; }

        private void Start()
        {
            Invoke("PauseParticles", 2);
            transform.parent = null;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        private void OnEnable()
        {
            GameController.OnLevelFailed += OnLevelFinished;
            GameController.OnLevelComplete += OnLevelFinished;
            weaponRemovedAlready = false;
        }
        private void OnDisable()
        {
            GameController.OnLevelFailed -= OnLevelFinished;
            GameController.OnLevelComplete -= OnLevelFinished;
        }

        void PauseParticles()
        {
            paricleCircle.Pause();
            pariclesSphere.Pause();
        }
        public void Collect()
        {
            HapticsManager.LightHaptic();
            weapon = WeaponManager.Instance.GetWeaponByWeaponType(WeaponType);
            if (weaponRemovedAlready == false)
            {
                OnWeaponCollected?.Invoke(weapon, (WeaponType == WeaponType.chest) ? true : false);
                weaponRemovedAlready = true;
            }
            if (collectSoundSO != null) AudioUtils.PlayOneShotAudio(collectSoundSO, transform.position);
            GetComponent<Collider>().enabled = false;
            DestroyWeapon();
        }


        public void DestroyWeapon()
        {
            if (Tile != null) Tile.hasWeapon = false;
            WeaponManager.Instance.Remove(this);


        }

        private void OnLevelFinished() => DestroyWeapon();
    }
}
