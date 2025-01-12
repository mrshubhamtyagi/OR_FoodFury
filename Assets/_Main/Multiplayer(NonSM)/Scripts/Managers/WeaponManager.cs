
using FoodFury;
using Fusion;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;
using UnityEngine.Serialization;

namespace OneRare.FoodFury.Multiplayer
{
	public class WeaponManager : NetworkBehaviour
	{
		public enum WeaponInstallationType
		{
			SUBMISSILE,
			KETCHUP,
			OILSPILL,
			NONE
		};

		[SerializeField] private Weapon[] _weapons;
		[SerializeField] private Player _player;
		public RocketHandler submissilePrefab;
		public RocketHandler ketchupPrefab;
		public OilSpill oilSpillPrefab;
		[Networked] public byte selectedPrimaryWeapon { get; set; }
		[Networked] public byte selectedSecondaryWeapon { get; set; }
		[Networked] public TickTimer primaryFireDelay { get; set; }
		[Networked] public TickTimer secondaryFireDelay { get; set; }
		[Networked] public byte primaryAmmo { get; set; }
		[Networked] public byte secondaryAmmo { get; set; }

		private byte _activePrimaryWeapon;
		private byte _activeSecondaryWeapon;
		NetworkObject networkObject;

		public override void Spawned()
		{
			networkObject = GetComponent<NetworkObject>();
		}

		public override void Render()
		{
			//ShowAndHideWeapons();
		}

		private void ShowAndHideWeapons()
		{
			// Animates the scale of the weapon based on its active status
			for (int i = 0; i < _weapons.Length; i++)
			{
				_weapons[i].Show(i == selectedPrimaryWeapon || i == selectedSecondaryWeapon);
			}

			// Whenever the weapon visual is fully visible, set the weapon to be active - prevents shooting when changing weapon
			SetWeaponActive(selectedPrimaryWeapon, ref _activePrimaryWeapon);
			SetWeaponActive(selectedSecondaryWeapon, ref _activeSecondaryWeapon);
		}

		void SetWeaponActive(byte selectedWeapon, ref byte _activeWeapon)
		{
			if (_weapons[selectedWeapon].isShowing)
				_activeWeapon = selectedWeapon;
		}

		public void ActivateWeapon(WeaponInstallationType weaponType, int weaponIndex)
		{
			byte selectedWeapon = weaponType == WeaponInstallationType.SUBMISSILE
				? selectedPrimaryWeapon
				: selectedSecondaryWeapon;
			byte activeWeapon = weaponType == WeaponInstallationType.SUBMISSILE
				? _activePrimaryWeapon
				: _activeSecondaryWeapon;
			
			if (!_player.IsActivated || selectedWeapon != activeWeapon)
				return;

			// Fail safe, clamp the weapon index within weapons list bounds
			weaponIndex = Mathf.Clamp(weaponIndex, 0, _weapons.Length - 1);

			if (weaponType == WeaponInstallationType.SUBMISSILE)
			{
				selectedPrimaryWeapon = (byte)weaponIndex;
				primaryAmmo = _weapons[(byte)weaponIndex].ammo;
			}

		}

		public void FireWeapon(WeaponInstallationType weaponType)
		{
			if (!IsWeaponFireAllowed(weaponType))
				return;

			byte ammo = weaponType == WeaponInstallationType.SUBMISSILE ? primaryAmmo : secondaryAmmo;

			TickTimer tickTimer = weaponType == WeaponInstallationType.SUBMISSILE ? primaryFireDelay : secondaryFireDelay;
			if (tickTimer.ExpiredOrNotRunning(Runner) && ammo > 0)
			{
				byte weaponIndex = weaponType == WeaponInstallationType.SUBMISSILE
					? _activePrimaryWeapon
					: _activeSecondaryWeapon;
				Weapon weapon = _weapons[weaponIndex];

				weapon.Fire(Runner, Object.InputAuthority, _player.Velocity);

				if (!weapon.infiniteAmmo)
					ammo--;

				if (weaponType == WeaponInstallationType.SUBMISSILE)
				{
					primaryFireDelay = TickTimer.CreateFromSeconds(Runner, weapon.delay);
					primaryAmmo = ammo;
				}
				else
				{
					secondaryFireDelay = TickTimer.CreateFromSeconds(Runner, weapon.delay);
					secondaryAmmo = ammo;
				}

				if ( /*Object.HasStateAuthority &&*/ ammo == 0)
				{
					ResetWeapon(weaponType);
				}
			}
		}

		TickTimer rocketFireDelay = TickTimer.None;
		[Header("Aim")] public Transform aimPoint;

		public void FireRocket(WeaponInstallationType weaponType)
		{
			if (weaponType == WeaponInstallationType.SUBMISSILE)
			{
				if (rocketFireDelay.ExpiredOrNotRunning(Runner))
				{
					Runner.Spawn(submissilePrefab, transform.position + transform.up * 0.2f + transform.forward * 1.5f,
						transform.rotation, Object.InputAuthority,
						(Runner, spawnedRocket) => { spawnedRocket.GetComponent<RocketHandler>().Fire(_player); });

					rocketFireDelay = TickTimer.CreateFromSeconds(Runner, 0.1f);
				}
			}
			else if (weaponType == WeaponInstallationType.OILSPILL)
			{
				if (rocketFireDelay.ExpiredOrNotRunning(Runner))
				{
					Runner.Spawn(oilSpillPrefab, transform.position + transform.up * 0.2f - transform.forward * 2f,
						transform.rotation, Object.InputAuthority,
						(Runner, spawnedRocket) => { spawnedRocket.GetComponent<OilSpill>().ShowDebug(); });

					rocketFireDelay = TickTimer.CreateFromSeconds(Runner, 0.1f);
				}
			}
			else
			{
				if (rocketFireDelay.ExpiredOrNotRunning(Runner))
				{
					Runner.Spawn(ketchupPrefab, transform.position + transform.up * 0.2f + transform.forward * 1.5f,
						transform.rotation, Object.InputAuthority,
						(Runner, spawnedRocket) => { spawnedRocket.GetComponent<RocketHandler>().Fire(_player); });

					rocketFireDelay = TickTimer.CreateFromSeconds(Runner, 0.1f);
				}
			}

			
		}

		//float maxHitDistance = 200;
		private bool IsWeaponFireAllowed(WeaponInstallationType weaponType)
		{
			if (!_player.IsActivated)
				return false;

			// Has the selected weapon become fully visible yet? If not, don't allow shooting
			if (weaponType == WeaponInstallationType.SUBMISSILE && _activePrimaryWeapon != selectedPrimaryWeapon)
				return false;
			if (weaponType == WeaponInstallationType.KETCHUP && _activeSecondaryWeapon != selectedSecondaryWeapon)
				return false;
			if(weaponType == WeaponInstallationType.OILSPILL && _activeSecondaryWeapon != selectedSecondaryWeapon )
				return false;
			else 
				return true;
		}

		public void ResetAllWeapons()
		{
			ResetWeapon(WeaponInstallationType.SUBMISSILE);
			ResetWeapon(WeaponInstallationType.KETCHUP);
		}

		void ResetWeapon(WeaponInstallationType weaponType)
		{
			if (weaponType == WeaponInstallationType.SUBMISSILE)
			{
				ActivateWeapon(weaponType, 0);
			}
			else if (weaponType == WeaponInstallationType.KETCHUP)
			{
				ActivateWeapon(weaponType, 4);
			}
		}

	}
}
