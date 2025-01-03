using System.Collections;
using System.Collections.Generic;
using FoodFury;
using Fusion;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using Tile = FoodFury.Tile;

namespace OneRare.FoodFury.Multiplayer
{
    [System.Serializable]
    public struct PowerupItem
    {
        public GameObject powerupPrefab;
        public int weight;  // Higher weight = higher chance of spawning
    }
    public class PowerUpSpawnManager : NetworkBehaviour
    {
        [Header("Powerup Settings")] [SerializeField]
        private PowerupItem[] powerupItems;
    //[SerializeField] private GameObject[] powerupPrefabs;  // Array of powerup prefabs
    [Networked] public Vector3 PowerupPosition { get; set; }
    [Networked] public TickTimer SpawnTimer { get; set; }
    [Networked] public bool IsMatchOver { get; set; } = false;
    [Networked] public int Counter  { get; set; }
    
    private ChangeDetector _changeDetector;


    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if (Runner.IsSharedModeMasterClient)
        {
            Counter = 0;
            SpawnTimer = TickTimer.CreateFromSeconds(Runner,   GameData.Instance.MultiplayerGameConfig.powerupSpawnInterval);
        }
        
        ChallengeManager.OnGameOver += OnGameOver;
    }

    void OnGameOver()
    {
        IsMatchOver = true;
    }

    GameObject GetRandomWeightedPowerup()
    {
        int totalWeight = 0;

        // Calculate total weight
        foreach (var item in powerupItems)
        {
            totalWeight += item.weight;
        }

        // Generate random value
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        // Select powerup based on cumulative weight
        foreach (var item in powerupItems)
        {
            currentWeight += item.weight;
            if (randomValue < currentWeight)
            {
                return item.powerupPrefab;
            }
        }

        // Fallback (should never hit this)
        return powerupItems[0].powerupPrefab;
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(PowerupPosition):
                {
                    SetTargetObject(PowerupPosition);
                    break;
                }
                case nameof(IsMatchOver):
                {
                    SetGameOver(IsMatchOver);
                    break;
                }
            }
        }
    }

    public void SetTargetObject(Vector3 newTarget)
    {
        PowerupPosition = newTarget;
    }

    private void SetGameOver(bool isGameOver)
    {
        IsMatchOver = isGameOver;
        //UIManager.Instance.ShowResultScreen(Player.Local);
    }

    public override void FixedUpdateNetwork()
    {
        if (SpawnTimer.Expired(Runner) && Counter < 12)
        {
            SpawnTimer = TickTimer.None;
            SpawnTimer = TickTimer.CreateFromSeconds(Runner,   GameData.Instance.MultiplayerGameConfig.powerupSpawnInterval);

            StartCoroutine("SpawnNextPowerup");
        }
    }

    IEnumerator SpawnNextPowerup()
    {
        yield return new WaitForSeconds(2f);
        SpawnPowerup();
        yield return new WaitForSeconds(3f);
        SpawnPowerup();
    }

    void SpawnPowerup()
    {
        if (Runner.IsSharedModeMasterClient && !IsMatchOver)
        {
            Counter++;
            Tile tileOne = TileManager.Instance.GetRandomTileInRangeForPoweups(30, 250);
            tileOne.hasBooster = true;
            PowerupPosition = tileOne.transform.position;

            // Get a weighted powerup
            GameObject chosenPowerup = GetRandomWeightedPowerup();
            chosenPowerup.GetComponent<Powerup>().tile = tileOne;
            Runner.Spawn(chosenPowerup, PowerupPosition, Quaternion.identity);
        }
    }


    public void OnPowerUpCollected()
    {
        Counter--;
    }
    
    private void OnDisable()
    {
        ChallengeManager.OnGameOver -= OnGameOver;
    }
}

}