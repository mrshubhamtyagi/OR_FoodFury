using FoodFury;
using UnityEngine;

public class AiWeaponTriggerComponent : MonoBehaviour
{
    [SerializeField] private float minTimeToGetAWeapon;
    [SerializeField] private float maxTimeToGetAWeapon;

    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform hotDogFirePosition;
    [SerializeField] private Transform puddleFirePosition;
    [SerializeField] private LayerMask layerMaskToCheck;
    [SerializeField] private Transform frontRaycastPosition;
    [SerializeField] private Transform backRaycastPosition;
    [SerializeField] private float raycastDistance;
    private Vehicle vehicle;
    private bool startWeaponTimer;
    private float timer;

    private void OnEnable()
    {
        vehicle = GetComponent<Vehicle>();
        startWeaponTimer = false;

        if (GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
        {
            minTimeToGetAWeapon = 3;
            maxTimeToGetAWeapon = 5;
        }

    }

    private void Update()
    {
        if (weapon != null || !WeaponManager.Instance) return;

        if (startWeaponTimer)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                weapon = WeaponManager.Instance.GiveRandomWeapon();
                startWeaponTimer = false;
            }
        }
    }


    void FixedUpdate()
    {
        if (weapon == null)
        {
            if (startWeaponTimer == false)
            {
                timer = UnityEngine.Random.Range(minTimeToGetAWeapon, maxTimeToGetAWeapon);
                startWeaponTimer = true;
            }
            return;
        }
        else if (weapon.WeaponType == WeaponType.subMissile || weapon.WeaponType == WeaponType.ketchup)
        {
            if (Physics.Raycast(frontRaycastPosition.position, transform.forward, out RaycastHit hit, raycastDistance, layerMaskToCheck))
            {
                if (hit.collider == null)
                {
                    Debug.Log("null collider");
                    return;
                }
                else
                {
                    if (hit.transform.TryGetComponent(out RiderBehaviour rider))
                    {
                        if (rider.Vehicle.RiderType == Enums.RiderType.Player)
                        {
                            Debug.Log("Fired at :" + rider.gameObject.name);
                            FireWeapon();

                        }
                    }
                }
            }
        }
        else if (weapon.WeaponType == WeaponType.oilspill)
        {
            if (Physics.Raycast(backRaycastPosition.position, -transform.forward, out RaycastHit hit, raycastDistance, layerMaskToCheck))
            {
                if (hit.collider == null)
                {
                    Debug.Log("null collider");
                    return;
                }
                else
                {
                    if (hit.transform.TryGetComponent(out RiderBehaviour rider))
                    {
                        if (rider.Vehicle.RiderType == Enums.RiderType.Player)
                        {
                            Debug.Log("Fired at :" + rider.gameObject.name);
                            FireWeapon();
                        }
                    }
                }
            }
        }


    }

    public void FireWeapon()
    {
        Transform firePosition = null;
        if (weapon.WeaponType == WeaponType.subMissile || weapon.WeaponType == WeaponType.ketchup)
        {
            firePosition = hotDogFirePosition;
        }
        else if (weapon.WeaponType == WeaponType.oilspill)
        {
            firePosition = puddleFirePosition;
        }
        Weapon weaponSpawned = WeaponManager.Instance.GetWeapon(weapon.WeaponType, firePosition);
        weaponSpawned.transform.forward = firePosition.transform.forward;
        weaponSpawned.transform.localPosition = Vector3.zero;
        weaponSpawned.transform.parent = null;
        weaponSpawned.gameObject.SetActive(true);
        weaponSpawned.Trigger(vehicle.RiderType);
        weapon = null;
    }


}
