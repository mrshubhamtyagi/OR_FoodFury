using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FoodFury
{
    public class OrderManager : MonoBehaviour
    {
        public static event Action<Order> OnNewOrderGenerated;
        [SerializeField] private Order orderPrefab;
        [SerializeField] private Texture2D defaultDishImage;

        [SerializeField] private List<Order> orders = new List<Order>();

        [SerializeField] private ModelClass.Dish dishData;
        [SerializeField] private ModelClass.Dish dummyOrder;

        public static OrderManager Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            //if (GameData.Instance == null && Application.isEditor)
            //    SpawnOrder();
        }

        public void AddOrder(Order _order)
        {
            if (orders.Contains(_order)) return;
            orders.Add(_order);
        }

        public bool RemoveOrder(Order _order)
        {
            if (_order == null) return false;
            if (!orders.Contains(_order)) return false;

            Addressables.ReleaseInstance(_order.gameObject);
            orders.Remove(_order);
            return true;
        }

        [ContextMenu("SpawnOrder")]
        public void SpawnOrder()
        {
            //Debug.Log("Order Count -> " + orders.Count);
            if (orders.Count > 1)
            {
                Debug.Log("Order is already present in the MAP.");
                return;
            }

            if (GameController.Instance.LevelInfo.FilteredDishData.Count == 0 && !Application.isEditor)
            {
                PopUpManager.Instance.ShowWarningPopup("No Filtered Dishes Found!");
                return;
            }

            Tile _tile = TileManager.Instance.GetRandomTileInRange(GameController.Instance.LevelInfo.CurrentLevel.orderMinRange, GameController.Instance.LevelInfo.CurrentLevel.orderMaxRange);

            dishData = GetDishData();
            print($"Dish Selected - {dishData.name}");

            SpawnUsingAddressable(_tile);
        }


        private ModelClass.Dish GetDishData()
        {
            ModelClass.Dish _dish = GameController.Instance.GetRandomDishData();
            return _dish == null ? dummyOrder : _dish;
        }


        //private void SpawnOrderUsingPrefab(Tile _tile)
        //{
        //    Order _order = Instantiate(orderPrefab, _tile.transform);
        //    _order.SetData(GameData.Instance == null ? 60 : GameData.Instance.GetCurrentLevel().orderTime, _tile, dishData);
        //    _order.gameObject.name = $"{dishData.name}_{dishData.level}";
        //    AddOrder(_order);
        //    StartCoroutine(APIManager.GetTexture(dishData.tokenId, (_status, _texture) =>
        //    {
        //        if (_status)
        //        {
        //            _order.SetThumbnail(_texture);
        //        }
        //        else if (PopUpManager.Instance != null)
        //        {
        //            PopUpManager.Instance.WarningPopup.SetWarning("Could not load dish image!");
        //            PopUpManager.Instance.ShowPopup(PopUpManager.Instance.WarningPopup);
        //        }

        //        OnNewOrderGenerated?.Invoke(_order);
        //    }));
        //}


        private void SpawnUsingAddressable(Tile _tile)
        {
            Order _order = AddressableLoader.SpawnObject<Order>(orderPrefab.gameObject, _tile == null ? transform : _tile.transform);
            _order.SetData(GameController.Instance == null ? 60 : GameController.Instance.LevelInfo.CurrentLevel.orderTime, _tile, dishData);
            _order.gameObject.name = $"{dishData.name}";
            AddOrder(_order);
            Texture2D dishImage = DishImages.LoadDishImage(dishData.tokenId);
            if (dishImage != null)
            {
                _order.SetThumbnail(dishImage);
            }
            else if (PopUpManager.Instance != null)
            {
                Debug.Log("Could not load dish image!");
                _order.SetThumbnail(defaultDishImage);
            }
            OnNewOrderGenerated?.Invoke(_order);
            //StartCoroutine(APIManager.GetDishTexture(dishData.tokenId, (_status, _texture) =>
            //{
            //    if (_status) _order.SetThumbnail(dishImage);
            //    else if (PopUpManager.Instance != null)
            //    {
            //        Debug.Log("Could not load dish image!");
            //        _order.SetThumbnail(defaultDishImage);
            //    }


            //}));
        }



        //public Order GetNearestOrder(Vector3 _riderPosition)
        //{
        //    return orders[0];

        //    float _distance = float.MaxValue;
        //    Order _nearestOrder = null;
        //    foreach (var _order in orders)
        //    {
        //        if (Vector3.Distance(_riderPosition, _order.transform.position) < _distance)
        //        {
        //            _distance = Vector3.Distance(_riderPosition, _order.transform.position);
        //            _nearestOrder = _order;
        //        }
        //    }

        //    return _nearestOrder;
        //}

        //private void OnDestroy() => orderAddressable.ReleaseAsset();


        public void ClearOrders()
        {
            for (int i = 0; i < orders.Count; i++)
                orders[i].DestroyOrder();

            orders.Clear();
        }

        public void DestroyInstance()
        {
            //ClearOrders();
            Destroy(gameObject);
        }
    }
}