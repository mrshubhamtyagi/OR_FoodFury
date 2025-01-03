using FoodFury;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [field: SerializeField] public List<Weapon> WeaponsList { get; private set; }
    [field: SerializeField] public List<WeaponItem> WeaponItemsList { get; private set; }
    [field: SerializeField] public List<WeaponItem> ActiveWeaponItems { get; private set; }
    [SerializeField] private ObjectPoolManager submissileObjectPool, PuddleObjectPool, ketchupObjectPool;
    public Dictionary<WeaponType, ObjectPoolManager> WeaponPools = new Dictionary<WeaponType, ObjectPoolManager>();
    [SerializeField] private int numberOfWeaponsAtATime = 5;
    private int currentWeaponSpawnIndex = 0;
    public static WeaponManager Instance;
    private Weapon lastWeaponGiven;
    void Awake() => Instance = this;


    private void OnEnable()
    {
        UnityEngine.Random.InitState(DateTime.Now.Second);
        GameController.OnLevelStart += SpawnStartingWeapons;
        WeaponItem.OnWeaponCollected += SpawnNewWeaponItem;
        WeaponPools.Add(WeaponType.subMissile, submissileObjectPool);
        WeaponPools.Add(WeaponType.ketchup, ketchupObjectPool);
        WeaponPools.Add(WeaponType.oilspill, PuddleObjectPool);
    }

    private void SpawnNewWeaponItem(Weapon arg1, bool arg2) => SpawnWeapon();

    private void OnDisable()
    {
        GameController.OnLevelStart -= SpawnStartingWeapons;
        WeaponItem.OnWeaponCollected -= SpawnNewWeaponItem;
    }
    public Weapon GetWeapon(WeaponType weaponType, Transform parent = null)
    {
        if (WeaponPools.ContainsKey(weaponType))
        {
            Weapon weapon = WeaponPools[weaponType].GetObjectFromPool().GetComponent<Weapon>();
            weapon.transform.parent = parent;
            weapon.transform.localPosition = Vector3.zero;
            return weapon;
        }
        else
        {
            return null;
        }
    }

    public void ReturnWeapon(Weapon weapon, WeaponType weaponType)
    {
        if (WeaponPools.ContainsKey(weaponType))
        {
            WeaponPools[weaponType].ReturnObjectToPool(weapon.gameObject);
        }

    }

    private void Add(WeaponItem _pickup)
    {
        if (ActiveWeaponItems.Contains(_pickup))
            return;

        ActiveWeaponItems.Add(_pickup);
    }
    public void Remove(WeaponItem _pickup)
    {
        if (ActiveWeaponItems.Contains(_pickup))
            ActiveWeaponItems.Remove(_pickup);

        Destroy(_pickup.gameObject);
    }
    public void SpawnWeapon()
    {
        Tile _tile = TileManager.Instance.GetRandomTile();
        SpawnUsingAddressable(_tile);
    }

    public void SpawnStartingWeapons()
    {
        ActiveWeaponItems = new();
        currentWeaponSpawnIndex = UnityEngine.Random.Range(0, WeaponItemsList.Count);
        int i = 0;
        for (i = 0; i < numberOfWeaponsAtATime; i++)
        {
            Tile _tile = TileManager.Instance.GetRandomTile();
            SpawnUsingAddressable(_tile);
        }
    }
    private void SpawnUsingAddressable(Tile _tile)
    {

        WeaponItem _weaponItemPrefab = WeaponItemsList[currentWeaponSpawnIndex];
        WeaponItem _weaponItem = AddressableLoader.SpawnObject<WeaponItem>(_weaponItemPrefab.gameObject, _tile.transform);
        _weaponItem.gameObject.name = $"{_weaponItem.name}";
        Add(_weaponItem);
        _tile.hasWeapon = true;
        currentWeaponSpawnIndex = (currentWeaponSpawnIndex + 1) % WeaponItemsList.Count;
        _weaponItem.transform.position = _tile.GetCenterTop();
        //Debug.Log("WeaponSpawned");
    }

    public void ClearWeapons()
    {
        for (int i = 0; i < ActiveWeaponItems.Count; i++)
            ActiveWeaponItems[i].DestroyWeapon();

        ActiveWeaponItems.Clear();
    }


    public void DestroyInstance()
    {
        // ClearWeapons();
        Destroy(gameObject);
    }
    public Weapon GetWeaponByWeaponType(WeaponType weaponType)
    {
        if (weaponType == WeaponType.chest)
        {
            return WeaponsList[UnityEngine.Random.Range(0, WeaponsList.Count)];
        }
        else
        {
            return WeaponsList.FirstOrDefault(x => x.WeaponType == weaponType);
        }
    }
    public Weapon GiveRandomWeapon()
    {
        Weapon weapon;
        do
        {
            weapon = WeaponsList[UnityEngine.Random.Range(0, WeaponsList.Count)];

        } while (weapon == lastWeaponGiven);
        lastWeaponGiven = weapon;
        return weapon;
    }

}
