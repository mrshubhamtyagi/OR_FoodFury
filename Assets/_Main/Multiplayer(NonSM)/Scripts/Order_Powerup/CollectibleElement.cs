using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public enum CollectibleType
    {
        ORDER = 0,
        BOOSTER = 1,
        BULLET = 2,
        HEALTH = 3,
        EMPTY = 4
    }
    
    [CreateAssetMenu(fileName = "CE_", menuName = "Scriptable Object/Special/CollectibleElement")]
    public class CollectibleElement : ScriptableObject
    {
        
        public WeaponManager.WeaponInstallationType weaponInstallationType;
        public CollectibleType powerupType;
        public Mesh powerupSpawnerMesh;
        public AudioClipData pickupSnd;
    }
}
