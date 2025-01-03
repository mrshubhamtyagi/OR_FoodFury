using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    public class ObstacleManager : MonoBehaviour
    {
        public Action<Obstacle> OnObstalcleSpawn;

        //[SerializeField] private float wait = 2;
        [SerializeField] private int distance = 6;
        [SerializeField] private Obstacle[] obstacleList;
        [SerializeField] private List<Tile> obstacleTiles;

        public Obstacle obstacle;
        WaitForSeconds waitFor1Sec = new WaitForSeconds(1);

        public static ObstacleManager Instance;
        void Awake() => Instance = this;

        void Start()
        {
            if (GameData.Instance == null && Application.isEditor) SpawnObstacle();
            else obstacle = GetObstacle();
        }

        private void OnEnable() => GameplayScreen.OnGameReady += OnGameReady;
        private void OnDisable() => GameplayScreen.OnGameReady -= OnGameReady;

        private void OnGameReady()
        {
            if (obstacle != null)
            {
                obstacle.OnObstacleBecameInvisible -= Event_OnObstacleBecameInvisible;
                obstacle.gameObject.SetActive(false);
                obstacleTiles.Clear();
            }

            foreach (var item in obstacleList)
                item.gameObject.SetActive(false);

            obstacle = GetObstacle();
        }

        private void Event_OnObstacleBecameInvisible()
        {
            obstacle.OnObstacleBecameInvisible -= Event_OnObstacleBecameInvisible;
            obstacle.gameObject.SetActive(false);
            obstacleTiles.Clear();
            //Invoke("SpawnObstacle", GameController.Instance == null ? wait : GameController.Instance.LevelInfo.CurrentLevel.obstacleDetails.timeout);
        }


        public void SpawnObstacle()
        {
            obstacle = GetObstacle();
            obstacle.OnObstacleBecameInvisible += Event_OnObstacleBecameInvisible;
            StartCoroutine("Co_SpawnObstacle");
        }



        private IEnumerator Co_SpawnObstacle()
        {
            yield break;
            //while (true)
            //{
            //    if (TileManager.Instance.GetObstacleTiles(distance, ref obstacleTiles))
            //    {
            //        obstacle.Setup(obstacleTiles[obstacleTiles.Count - 1].transform.position, TileManager.Instance.GetRiderDirection());
            //        obstacle.gameObject.SetActive(true);
            //        Debug.Log("Obstacle Spawned at " + obstacleTiles[obstacleTiles.Count - 1].name);
            //        yield break;
            //    }

            //    yield return waitFor1Sec;
            //}
        }

        private Obstacle GetObstacle()
        {
            if (GameController.Instance == null) return obstacle;
            return obstacleList[UnityEngine.Random.Range(0, obstacleList.Length)];
        }


        public void DestroyInstance()
        {
            StopCoroutine("Co_SpawnObstacle");
            Destroy(gameObject);
        }

    }

}