using FoodFury;
using System.Collections.Generic;
using UnityEngine;

public class MascotSpawnManager : MonoBehaviour
{
    [SerializeField] private Mascot mascotPrefab;
    public static MascotSpawnManager Instance;
    private Mascot mascot;
    [SerializeField] private List<int> levelsInWhichMascotCanBeSpawned;
    private void Awake()
    {

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void OnEnable() => GameController.OnLevelStart += CheckAndSpawnMascot;
    private void OnDisable() => GameController.OnLevelStart -= CheckAndSpawnMascot;
    public void CheckAndSpawnMascot()
    {
        DeleteOlderMascot();
        if (levelsInWhichMascotCanBeSpawned.Contains(GameData.Instance.SelectedLevelNumber))
            SpawnUsingAddressable(GameController.Instance.SpawnPositions.Mascot[Random.Range(0, GameController.Instance.SpawnPositions.Mascot.Length)]);


    }

    private void DeleteOlderMascot()
    {
        if (mascot != null)
            mascot.DestroyMascot();
    }

    private void SpawnUsingAddressable(PositionAndRotation positionAndRotation)
    {
        mascot = AddressableLoader.SpawnObject<Mascot>(mascotPrefab.gameObject);
        mascot.transform.parent = GameController.Instance.SpawnPositions.transform;
        mascot.transform.SetPositionAndRotation(positionAndRotation.position, Quaternion.Euler(positionAndRotation.rotation));
    }

    [ContextMenu("Spawn Mascot Regardless")]
    public void CheckAndSpawnMascot_Debug() => SpawnUsingAddressable(GameController.Instance.SpawnPositions.Mascot[Random.Range(0, GameController.Instance.SpawnPositions.Mascot.Length)]);
    public void DestroyInstance()
    {
        if (mascot != null)
            mascot.DestroyMascot();

        Destroy(gameObject);
    }
    public void ResetMascot() => mascot = null;

}
