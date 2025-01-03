using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FoodFury
{
    public class BoosterManager : MonoBehaviour
    {
        public static event Action<Booster> OnBoosterGenerated;
        [SerializeField] private Booster boosterPrefab;

        [Header("---Debug")]
        public bool spawnOnStart = true;
        [SerializeField] private ModelClass.BoosterDetail boosterData;
        [SerializeField] private ModelClass.BoosterDetail dummyBooster;
        [field: SerializeField] public List<Booster> ActiveBoosters { get; private set; }

        public static BoosterManager Instance;
        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            ActiveBoosters = new List<Booster>();
            if (GameData.Instance == null && Application.isEditor) SpawnBooster();
        }

        private void Add(Booster _pickup)
        {
            if (ActiveBoosters.Contains(_pickup))
                return;

            ActiveBoosters.Add(_pickup);
        }


        public void Remove(Booster _pickup)
        {
            if (!ActiveBoosters.Contains(_pickup))
                return;


            Addressables.ReleaseInstance(_pickup.gameObject);
            ActiveBoosters.Remove(_pickup);
            //SpawnBooster();
        }

        public void SpawnBooster()
        {
            Tile _tile = TileManager.Instance.GetRandomTile();
            boosterData = GameController.Instance != null ? GameController.Instance.LevelInfo.CurrentLevel.boosterDetails[UnityEngine.Random.Range(0, GameController.Instance.LevelInfo.CurrentLevel.boosterDetails.Count)] : dummyBooster;

            SpawnUsingAddressable(_tile);
        }

        //private void SpawnUsingPrefab(Tile _tile)
        //{
        //    if (ActiveBoosters.Count > 0) return;

        //    Booster _booster = Instantiate(boosterPrefab, _tile.transform);
        //    int _value = boosterData.values[UnityEngine.Random.Range(0, boosterData.values.Count)];
        //    string _percentage = boosterData.type == Enums.BoosterType.Speed ? "%" : "";
        //    _booster.SetData(new ModelClass.BoosterData()
        //    {
        //        type = boosterData.type,
        //        value = _value,
        //        description = $"+{_value}{_percentage}", //\n{boosterData.type}
        //        delay = 0
        //    }, _tile);

        //    _booster.gameObject.name = $"{boosterData.type}_{_value}";
        //    Add(_booster);
        //    OnBoosterGenerated?.Invoke(_booster);
        //}

        private void SpawnUsingAddressable(Tile _tile)
        {
            if (ActiveBoosters.Count > 1) return;

            Booster _booster = AddressableLoader.SpawnObject<Booster>(boosterPrefab.gameObject, _tile == null ? transform : _tile.transform);
            int _value = boosterData.values[UnityEngine.Random.Range(0, boosterData.values.Count)];
            string _percentage = boosterData.type == Enums.BoosterType.Speed ? "%" : "";
            _booster.SetData(new ModelClass.BoosterData()
            {
                type = boosterData.type,
                value = _value,
                description = $"+{_value}{_percentage}", //\n{boosterData.type}
                delay = 0
            }, _tile);

            _booster.gameObject.name = $"{boosterData.type}_{_value}";
            Add(_booster);
            OnBoosterGenerated?.Invoke(_booster);
        }

        public void ClearBoosters()
        {
            for (int i = 0; i < ActiveBoosters.Count; i++)
                ActiveBoosters[i].DestroyBooster();

            ActiveBoosters.Clear();
        }

        public void DestroyInstance()
        {
            //ClearBoosters();
            Destroy(gameObject);
        }

    }
}
