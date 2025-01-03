using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace FoodFury
{
    public class TileManager : MonoBehaviour
    {
        [SerializeField] private Transform tilesParent;
        [SerializeField] private GameObject tileCanvasPrefab;
        [SerializeField] private bool showCanvas;
        [SerializeField] private Tile[] tiles;
        public Rider rider;
        //[field: SerializeField] public Tile RiderTile;

        [Header("-----Debug")]
        public int obstacleDistance = 1;
        private Enums.Direction RiderDirection = Enums.Direction.None;

        public static TileManager Instance;
        public List<Vector3> spawnedOrderPositions = new List<Vector3>();
        public List<Vector3> spawnedPowerupsPositions = new List<Vector3>();
        void Awake()
        {
            Instance = this;
            //showCanvas = Application.isEditor;
            //RenameTiles();
        }

        private void Start()
        {
            if (Application.isEditor)
            {
                var renderers = FindObjectsOfType<MeshRenderer>();
                foreach (var item in renderers)
                {
                    foreach (var material in item.materials)
                    {
                        material.shader = Shader.Find(material.shader.name);
                    }
                }
            }

            AssignTileIndices();
        }
        private void AssignTileIndices()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].Index = i; // Assign index based on position in the array
            }
        }
        public static Tile FindNextTileInDirection(Vector3 _origin, Vector3 _direction)
        {
            if (Physics.Raycast(_origin, _direction, out RaycastHit hitInfo, 10, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
                return hitInfo.collider.GetComponent<Tile>();
            else
                return null;
        }

        public Tile GetRandomTile()
        {
            Tile[] _tiles = tiles.Where(t => IsValid(t)).ToArray();
            int random = Random.Range(0, _tiles.Length);
            Tile tile = _tiles[random];

            return tile;
        }

        private Vector3 lastSpawnedTilePosition;
        private int initialSpawnCount = 3;
        int maxRetries = 10;
        float minDistance = 50f;

        Tile[] validTiles;
        public Tile GetRandomTileInRange(int _min, int _max)
        {

            int retryCount = 0;
            // Determine the reference position: use last spawned tile's position if more than initial tiles are spawned
            Vector3 referencePosition = spawnedOrderPositions.Count > initialSpawnCount ? lastSpawnedTilePosition : transform.position;

            do
            {
                // Filter out tiles that are in range and valid
                validTiles = tiles.Where(t => IsInTange(t.transform.position, _min, _max) && IsValid(t))
                    .Where(t => spawnedOrderPositions.All(pos => Vector3.Distance(pos, t.transform.position) >= minDistance))
                    .ToArray();

                retryCount++;
                minDistance -= 2f; // Reduce distance threshold with each retry
            }
            while (validTiles.Length == 0 && retryCount < maxRetries);

            if (validTiles.Length == 0)
            {
                Debug.LogError("No valid tiles found after retries.");
                return null;
            }

            // Randomly select a valid tile from the filtered list
            Tile selectedTile = validTiles[Random.Range(0, validTiles.Length)];

            // Add selected tile's position to the list
            spawnedOrderPositions.Add(selectedTile.transform.position);

            return selectedTile;
        }

        public Tile GetRandomTileInRangeForPoweups(int _min, int _max)
        {

            Tile selectedTile = null;
            int maxAttempts = 5; // Limit the number of attempts to find a suitable tile

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Pick a random tile directly from valid tiles
                Tile tile = tiles.Where(t => IsInTange(t.transform.position, _min, _max) && IsValid(t))
                    .OrderBy(t => Random.value) // Shuffle once per attempt
                    .FirstOrDefault(t =>
                        spawnedPowerupsPositions.All(pos => Vector3.Distance(pos, t.transform.position) >= 50));

                if (tile != null)
                {
                    selectedTile = tile;
                    break;
                }
            }

            if (selectedTile != null)
            {
                // Limit `spawnedOrderPositions` to the 5 most recent positions
                if (spawnedPowerupsPositions.Count >= 6)
                {
                    spawnedPowerupsPositions.RemoveAt(0); // Remove the oldest position
                }

                spawnedPowerupsPositions.Add(selectedTile.transform.position);
            }

            return selectedTile;
        }



        private bool IsValid(Tile _tile) => !_tile.hasOrder && !_tile.hasBooster && !_tile.hasPlayer && !_tile.IsTurnTile && !_tile.hasWeapon && !_tile.hasChomp && _tile.GetNeighbours().Count > 1;
        private float _distance;
        private Vector3 centrePos = new Vector3(-90, 1, -15);
        private bool IsInTange(Vector3 _position, int _min, int _max)
        {
            if (rider == null)
            {

                _distance = Vector3.Distance(_position, centrePos);
            }
            else
            {
                _distance = Vector3.Distance(_position, rider.transform.position);
            }

            return _distance > _min && _distance < _max;
        }


        //private Tile GetRiderTile() => rider.Vehicle.GetRiderTile();
        public Enums.Direction GetRiderDirection() => HelperFunctions.GetGlobalDirection(rider.transform.forward.normalized, 0.9f);


        //public bool GetObstacleTiles(int _distance, ref List<Tile> _obstacleTiles)
        //{
        //    _obstacleTiles.Clear();

        //    if (Application.isPlaying) RiderTile = GetRiderTile();
        //    RiderDirection = GetRiderDirection();

        //    if (RiderDirection == Enums.Direction.None)
        //        return false;

        //    Vector3 _directionVector = RiderDirection.ToVector3();
        //    Tile _startTile = RiderTile;

        //    for (int i = 0; i < _distance;)
        //    {
        //        Vector3 _startPos = _startTile.transform.position;
        //        Vector3 _endPos = _startPos + _directionVector;
        //        _startTile = FindNextTileInDirection(_startPos, HelperFunctions.GetGlobalVectorDirection((_endPos - _startPos).normalized, 0.965f));

        //        if (_startTile == null) return false;
        //        if (!_startTile.IsTurnTile)
        //        {
        //            _obstacleTiles.Add(_startTile);
        //            i++;
        //        }
        //    }

        //    return true;
        //}


        [ContextMenu("FindNeighbours")]
        public void FindNeighbours()
        {
            if (tiles.Count() == 0) tiles = tilesParent.GetComponentsInChildren<Tile>();
            foreach (var _tile in tiles)
                _tile.FindNeighbours();
        }
        public Tile GetTileByIndex(int index)
        {
            if (index < 0 || index >= tiles.Length)
            {
                Debug.LogError("Invalid tile index");
                return null;
            }
            return tiles[index];
        }

        [ContextMenu("RenameTiles")]
        private void RenameTiles()
        {
            int _index = 0;
            tiles = tilesParent.GetComponentsInChildren<Tile>();
            foreach (Tile tile in tiles)
            {
                tile.gameObject.name = "Tile " + ++_index;
                tile.FindNeighbours();

                tile.Index = _index - 1; // Set the index of the tile

                if (tile.GetComponentInChildren<Canvas>(true))
                {
                    if (tileCanvasPrefab)
                        tile.GetComponentInChildren<TMPro.TextMeshProUGUI>(true).text = _index.ToString();
                    else
                        DestroyImmediate(tile.GetComponentInChildren<Canvas>(true).gameObject);
                }
                else if (tileCanvasPrefab)
                    Instantiate(tileCanvasPrefab, tile.transform).GetComponentInChildren<TMPro.TextMeshProUGUI>(true).text = _index.ToString();

                if (tile.GetComponentInChildren<Canvas>(true))
                    tile.GetComponentInChildren<Canvas>(true).gameObject.SetActive(showCanvas);
            }
        }

    }

}