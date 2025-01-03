using FoodFury;
using Fusion;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    public class Powerup : NetworkBehaviour, ICollidable
    {
        private ICollidable _collidableImplementation;
        private PowerUpSpawnManager _powerUpSpawnManager;
        [SerializeField] private WeaponManager.WeaponInstallationType type;
      
        public Tile tile;
        public override void Spawned()
        {
            _powerUpSpawnManager = FindObjectOfType<PowerUpSpawnManager>();
            transform.position = _powerUpSpawnManager.PowerupPosition;
        }
        

        private void OnTriggerEnter(Collider other)
        {
            if (!Object.HasStateAuthority) return;

            if (other.gameObject.TryGetComponent(out Player player))
            {
                Collide(player);
            }
        }

        public void Collide(Player player)
        {
            if (player.IsBot)
                return;
            if (type == WeaponManager.WeaponInstallationType.NONE)
            {
                if (player.playerMovementHandler.BoostEndTick > 0)
                {
                    return;
                }
                player.playerMovementHandler.GiveBoost();
            }
            else
            {
                //if ((player.BulletCount > 0))
                //    return;
                player.WeaponCollected(type);
            }
            
            gameObject.SetActive(false);
            
            if (HasStateAuthority)
            {
                tile.hasBooster = false;
                _powerUpSpawnManager.OnPowerUpCollected();
                Runner.Despawn(Object);
            }
   
        }
    }
}