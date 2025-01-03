using FoodFury;
using UnityEngine;

public class PlayerWeaponTriggerComponent : MonoBehaviour
{
    [field: SerializeField] public Weapon Weapon { get; private set; }
    [SerializeField] private Transform hotDogFirePosition;
    [SerializeField] private Transform puddleFirePosition;
    private Vehicle vehicle;

    private void OnEnable()
    {
        Weapon = null;
        vehicle = GetComponent<Vehicle>();
        GameplayScreen.OnWeaponButtonActivated += AddWeapon;
        InputManager.OnShootButtonPressed += FireWeapon;
        GameController.OnLevelStart += SetupWeapon;
    }

    private void OnDisable()
    {
        GameplayScreen.OnWeaponButtonActivated -= AddWeapon;
        InputManager.OnShootButtonPressed -= FireWeapon;
        GameController.OnLevelStart -= SetupWeapon;
    }

    private void SetupWeapon() => Weapon = null;

    private void AddWeapon(Weapon obj)
    {
        if (Weapon == null || GameData.Instance.releaseMode == Enums.ReleaseMode.Debug)
            Weapon = obj;
    }


    public void FireWeapon()
    {
        Transform firePosition = null;
        if (Weapon == null)
            return;


        if (Weapon.WeaponType == WeaponType.subMissile || Weapon.WeaponType == WeaponType.ketchup)
        {
            firePosition = hotDogFirePosition;
        }
        else if (Weapon.WeaponType == WeaponType.oilspill)
        {
            firePosition = puddleFirePosition;
        }

        // Weapon weapon = Instantiate(Weapon, firePosition);
        Weapon weapon = WeaponManager.Instance.GetWeapon(Weapon.WeaponType, firePosition);
        weapon.transform.forward = firePosition.transform.forward;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.parent = null;
        weapon.gameObject.SetActive(true);
        weapon.Trigger(vehicle.RiderType);
        AnalyticsManager.Instance.FireWeaponFiredEvent(weapon.WeaponType.ToString());
        if (GameData.Instance.releaseMode == Enums.ReleaseMode.Release) Weapon = null;
    }

}
//distance measure 
//in forward direction
//player hai toh trigger it 


