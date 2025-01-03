using FoodFury;
using System;
using UnityEngine;
namespace FoodFury
{
    public abstract class Weapon : MonoBehaviour
    {
        //  public WeaponData weaponData;//weapon Prefab
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public Sprite WeaponIcon { get; private set; }
        [field: SerializeField] public Sprite WeaponCircle { get; private set; }
        public static Action<WeaponType> OnHitPlayer;
        public static Action<WeaponType, Enums.RiderType> OnHitAI;
        public static Action<WeaponType, Enums.RiderType> OnWeaponTriggered;
        [HideInInspector] public float timer;
        public Enums.RiderType weaponFiredBy;
        public abstract void Trigger(Enums.RiderType _riderType);
        public abstract void OnHitVehicle(RiderBehaviour rider);
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.transform.TryGetComponent<RiderBehaviour>(out RiderBehaviour riderBehaviour))
        //    {
        //        riderBehaviour.OnWeaponCollected?.Invoke(this);
        //    }
        //}


        private void OnBecameInvisible() => Invoke("DestroyWeapon", 0.5f);

        private void DestroyWeapon() => DestroyInstance();
        public abstract void DestroyInstance();
    }

}
