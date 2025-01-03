using System.Collections.Generic;
using UnityEngine;

namespace FoodFury
{
    public class PathGenerator
    {
        public List<Vector3> GenerateQuickPathPositions(Tile _startTile, Tile _endTile, out float _distance, int _count = 100)
        {
            List<Vector3> _positions = new List<Vector3>();
            List<Tile> _tiles = GenerateQuickPath(_startTile, _endTile, out _distance, _count);
            foreach (var item in _tiles) _positions.Add(item.transform.position);
            return _positions;
        }


        private List<Tile> GenerateQuickPath(Tile _startTile, Tile _endTile, out float _distance, int _count = 100)
        {
            List<Tile> _tiles = new List<Tile> { _startTile };
            Tile _tile = _startTile;

            _distance = 0;
            while (true)
            {
                if (--_count < 0) break;

                Vector3 _lastPos = _tile.transform.position;
                _tile = _tile.GetClosestNeighbour(_endTile, _tiles.Count > 1 ? _tiles[_tiles.Count - 2] : _tiles[_tiles.Count - 1]);
                _tiles.Add(_tile);
                _distance += Vector3.Distance(_tile.transform.position, _lastPos);


                if (_endTile.GetNeighbours().Contains(_tile))
                    break;
            }


            // Add End Tile
            //_tiles.Add(_endTile);
            //_distance += Vector3.Distance(_tile.transform.position, _endTile.transform.position);
            return _tiles;
        }


        private List<Vector3> GenerateSmoothPath(Tile _startTile, Tile _endTile, int _count = 100)
        {
            List<Vector3> _tiles = new List<Vector3> { _startTile.transform.position };
            Tile _tile = _startTile;

            int _safe = 0;
            while (true)
            {
                if (++_safe > _count) break;

                _tile = _tile.GetClosestNeighbour(_endTile);


                Vector3 _addPos = new Vector3(_tile.transform.position.x + 2f, _tile.transform.position.y, _tile.transform.position.z + 2f);
                _tiles.Add(_addPos);
                Vector3 _addPos2 = new Vector3(_tile.transform.position.x - 2f, _tile.transform.position.y, _tile.transform.position.z - 2f);
                _tiles.Add(_addPos2);
                //_tiles.Add(_tile.transform.position);

                if (_endTile.GetNeighbours().Contains(_tile))
                    break;
            }

            if (_tiles.Count < _count) _tiles.Add(_endTile.transform.position);
            return _tiles;
        }


        private List<Tile> GeneratePath(Tile _startTile, Tile _endTile, Transform _rider, out float _distance, int _count = 100)
        {
            List<Tile> _tiles = new List<Tile> { _startTile };
            _distance = 0;

            //Add First Tile
            Tile _tile = _startTile.GetClosestNeighbour(_endTile, _startTile, _rider, true);
            _tiles.Add(_tile);
            _distance += Vector3.Distance(_tile.transform.position, _startTile.transform.position);

            int _safe = 0;
            int _index = 0;
            while (++_safe < _count)
            {
                Vector3 _lastPos = _tile.transform.position;
                _tile = _tile.GetClosestNeighbour(_endTile, _tiles[_index++], _rider);
                //if (_tile == null) return _tiles;
                _tiles.Add(_tile);
                _distance += Vector3.Distance(_tile.transform.position, _lastPos);


                if (_endTile.GetNeighbours().Contains(_tile))
                    break;
            }

            _tiles.Add(_endTile);
            _distance += Vector3.Distance(_tile.transform.position, _endTile.transform.position);

            return _tiles;
        }

    }




}
