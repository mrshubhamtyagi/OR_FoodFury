using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    [ExecuteInEditMode]
    public class Tile : MonoBehaviour
    {
        public bool hasPlayer;
        public bool hasOrder;
        public bool hasBooster;
        public bool hasWeapon;
        public bool hasChomp;
        public int Index { get; set; }
        [field: SerializeField] public bool IsTurnTile;
        [field: SerializeField] public Enums.Direction TileDirection { get; private set; }
        [field: SerializeField] public List<Tile> Neighbours { get; private set; }

        [Header("-----Debug")]
        public LayerMask roadTileLayer;
        [SerializeField] private float rayDistance = 10;
        public bool _draw = false;

        [SerializeField] private Vector3 center;
        [field: SerializeField] public Vector3 size { get; private set; }
        [ContextMenu("FindNeighbours")]
        public void FindNeighbours()
        {
            bool _temp = _draw;
            _draw = false;
            SetCenter();
            Neighbours = new List<Tile>();
            GetNeighbour(Vector3.forward);
            GetNeighbour(Vector3.right);
            GetNeighbour(Vector3.left);
            GetNeighbour(Vector3.back);
            _draw = _temp;
            if (!IsTurnTile && Neighbours.Count > 1)
                TileDirection = HelperFunctions.GetGlobalDirection((Neighbours[0].transform.position - Neighbours[1].transform.position).normalized, 0.9f);
        }

        public void SetCenter()
        {
            center = transform.position;
            if (TryGetComponent(out MeshRenderer _renderer))
            {
                center = _renderer.bounds.center;
                size = _renderer.bounds.size;
            }
            else if (transform.GetChild(0).TryGetComponent(out MeshRenderer _renderer1))
            {
                center = _renderer1.bounds.center;
                size = _renderer1.bounds.size;
            }
        }
        public Vector3 GetCenter()
        {
            SetCenter();
            return center;
        }
        public Vector3 GetCenterTop()
        {
            SetCenter();
            return new Vector3(center.x, center.y + (size.y * 0.5f), center.z);
        }
        public List<Tile> GetNeighbours() => Neighbours;


        private void GetNeighbour(Vector3 _direction)
        {
            int _safe = 100;
            Tile _tile = this;
            while (_safe > 0)
            {
                _safe--;
                _tile = FindNextTileInDirection(new Ray(_tile.center, _direction));
                if (_tile == null) return;

                //print(_tile.gameObject.name);
                if (_tile.IsTurnTile && !Neighbours.Contains(_tile))
                {
                    Neighbours.Add(_tile);
                    return;
                }
            }
        }

        private Tile FindNextTileInDirection(Ray _ray)
        {
            Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, Color.yellow, 10);
            if (Physics.Raycast(_ray, out RaycastHit hitInfo, rayDistance, roadTileLayer, QueryTriggerInteraction.Collide))
            {
                Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, Color.green, 10);
                return hitInfo.collider.GetComponent<Tile>();
            }
            else
            {
                Debug.DrawRay(_ray.origin, _ray.direction * rayDistance, Color.red, 10);
                return null;
            }
        }

        private Tile AssignTile(Vector3 _origin, Vector3 _direction)
        {
            Tile _result;
            if (Physics.Raycast(_origin, _direction, out RaycastHit hitInfo, 12))
            {
                _result = hitInfo.collider.GetComponent<Tile>();
                if (_result != null) Debug.DrawRay(_origin, _direction * hitInfo.distance, Color.green);
            }
            else
            {
                _result = null;
                Debug.DrawRay(_origin, _direction * 5, Color.red);
            }
            return _result;
        }




        public Tile GetClosestNeighbour(Tile _target)
        {
            if (Neighbours.Count == 0) FindNeighbours();
            if (Neighbours.Count == 0) return this;


            Tile _nearestTile = Neighbours[0];
            foreach (var item in Neighbours)
            {
                if (item == _nearestTile) continue;

                float _currentDis = Vector3.Distance(_target.transform.position, _nearestTile.transform.position);
                float _newDis = Vector3.Distance(_target.transform.position, item.transform.position);

                if (_newDis < _currentDis)
                    _nearestTile = item;
            }
            return _nearestTile;
        }


        public Tile GetClosestNeighbour(Tile _target, Tile _lastTile)
        {
            if (Neighbours.Count == 0) FindNeighbours();
            if (Neighbours.Count == 0) return this;

            //print($"Last Tile {_lastTile.name} | This tile {gameObject.name}");
            Tile _nearestTile = Neighbours[0] == _lastTile ? Neighbours[1] : Neighbours[0];
            foreach (var item in Neighbours)
            {
                if (item == _nearestTile || item == _lastTile) continue;

                float _currentDis = Vector3.Distance(_target.transform.position, _nearestTile.transform.position);
                float _newDis = Vector3.Distance(_target.transform.position, item.transform.position);

                if (_newDis < _currentDis)
                    _nearestTile = item;
            }
            return _nearestTile;
        }


        Vector3 _riderDirection;
        Vector3 _tileDirection;
        public Tile GetClosestNeighbour(Tile _target, Tile _last, Transform _rider, bool _isFirst = false)
        {
            if (Neighbours.Count == 0) FindNeighbours();
            if (Neighbours.Count == 0) return this;


            Tile _nearestTile = Neighbours[0];
            if (!_isFirst && _last.Neighbours.Contains(_nearestTile)) _nearestTile = Neighbours[1];

            foreach (var item in Neighbours)
            {
                if (item == _nearestTile) continue;

                if (_isFirst)
                {
                    _riderDirection = (_rider.forward);
                    _tileDirection = (item.transform.position - _rider.position).normalized;
                    //print($"DOT  {GetDotProduct(_riderDirection.normalized, _tileDirection.normalized)}");
                    if (GetDotProduct(_riderDirection.normalized, _tileDirection.normalized) > 0)
                        _nearestTile = item;
                }
                else
                {
                    //if (_target.Neighbours.Contains(item)) return null;

                    //print($"Called On {gameObject.name} | Current Loop {item.name} | Tile Before {_nearestTile.name}");

                    float _currentDis = Vector3.Distance(_target.transform.position, _nearestTile.transform.position);
                    float _newDis = Vector3.Distance(_target.transform.position, item.transform.position);

                    //if (_last != item) continue;
                    if (_newDis < _currentDis && !_last.Neighbours.Contains(item))
                    {
                        //print($"Tile After {_nearestTile.name}");
                        _nearestTile = item;
                    }
                }
            }
            return _nearestTile;
        }



        private void OnDrawGizmosSelected()
        {
            if (_draw)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(center, 0.25f);

                Vector3 finalPosition = center;
                LayerMask _layer = roadTileLayer;

                if (Physics.Raycast(finalPosition, Vector3.forward, out RaycastHit hitInfoF, rayDistance, _layer, QueryTriggerInteraction.Collide))
                    Gizmos.color = Color.green;
                else Gizmos.color = Color.red;
                Gizmos.DrawRay(finalPosition, Vector3.forward * rayDistance);


                if (Physics.Raycast(finalPosition, Vector3.back, out RaycastHit hitInfoB, rayDistance, _layer, QueryTriggerInteraction.Collide))
                    Gizmos.color = Color.green;
                else Gizmos.color = Color.red;
                Gizmos.DrawRay(finalPosition, Vector3.back * rayDistance);

                if (Physics.Raycast(finalPosition, Vector3.left, out RaycastHit hitInfoL, rayDistance, _layer, QueryTriggerInteraction.Collide))
                    Gizmos.color = Color.green;
                else Gizmos.color = Color.red;
                Gizmos.DrawRay(finalPosition, (Vector3.left * rayDistance));


                if (Physics.Raycast(finalPosition, Vector3.right, out RaycastHit hitInfoR, rayDistance, _layer, QueryTriggerInteraction.Collide))
                    Gizmos.color = Color.green;
                else Gizmos.color = Color.red;
                Gizmos.DrawRay(finalPosition, (Vector3.right * rayDistance));
            }


            if (Neighbours != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var _tile in Neighbours)
                    Gizmos.DrawWireCube(_tile.transform.position, Vector3.one * 5);
            }
        }

        private float GetDotProduct(Vector3 _origin, Vector3 _direction) => Vector3.Dot(_origin, _direction);
    }



}
