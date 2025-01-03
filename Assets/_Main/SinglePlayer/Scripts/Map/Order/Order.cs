using FoodFury;
using System;
using System.Collections;
using UnityEngine;

public class Order : MonoBehaviour
{
    public static event Action<Order> OnOrderFailed;
    public event Action<Order> OnOrderCollected;
    public event Action<int> OnOrderTimeUpdate;
    public event Action<float> OnOrderDeliveringTimeUpdate;

    [field: SerializeField] public OrderNFTDetails OrderNFTDetails { get; private set; }
    [field: SerializeField] public ModelClass.Dish Dish { get; private set; }
    [field: SerializeField] public Texture2D Thumbnail { get; private set; }

    public bool IsNFTOrder => OrderNFTDetails.isNFT;
    public int NFTPoint => OrderNFTDetails.nftPoint;
    public int NFTOrderTokenId => OrderNFTDetails.tokenId;

    [SerializeField] private ParticleSystem particles;
    [SerializeField] private GameObject locatorParent;
    [SerializeField] private Renderer locatorMaterial;
    [SerializeField] private Renderer nftMaterial;
    [SerializeField] private Texture2D defaultDishTexture;

    [Header("---Customer")]
    [SerializeField] public Transform customer;
    [SerializeField] public Renderer customerMaterial;
    [SerializeField] private Texture2D[] customerTextures;
    private Animation customerAnimator;


    [field: SerializeField] public Tile Tile { get; private set; }
    [field: SerializeField] public int RemainingTime { get; private set; }

    [Header("---Debugs")]
    public bool pauseCountdown = true;
    [SerializeField] float _deliveringTime = 0;
    [SerializeField] float _deliveryTimeout = 1f;
    public Enums.OrderCollectingBy collectingBy = Enums.OrderCollectingBy.None;

    private WaitForSeconds waitFor1Sec = new WaitForSeconds(1);
    private float deliveryingDelay = -.1f;

    // private void Awake() => customerAnimator = GetComponentInChildren<Animation>();

    private void OnEnable()
    {
        GameController.OnLevelFailed += OnLevelFailed;
        GameController.OnLevelComplete += OnLevelFailed;
    }

    private void OnDisable()
    {
        GameController.OnLevelFailed -= OnLevelFailed;
        GameController.OnLevelComplete -= OnLevelFailed;
    }

    private void OnLevelFailed() => Invoke("DestroyOrder", 0.1f);

    public void SetData(int _time, Tile _tile, ModelClass.Dish _dish)
    {
        Tile = _tile;
        if (Tile != null) Tile.hasOrder = this;

        Dish = _dish;
        OrderNFTDetails = new();
        OrderNFTDetails.isNFT = GameData.Instance.CheckUserHasDish(Dish.baseId, out OrderNFTDetails.tokenId);
        OrderNFTDetails.nftLevel = IsNFTOrder ? GameData.Instance.GetUserDishByToken(OrderNFTDetails.tokenId).level : _dish.level;
        int _nonNFTChips = GameController.Instance.CalculationData.chipsPoints[1] - GameController.Instance.CalculationData.chipsPoints[0];
        int _nftChips = GameController.Instance.CalculationData.chipsPoints[OrderNFTDetails.nftLevel] - GameController.Instance.CalculationData.chipsPoints[0];
        OrderNFTDetails.nftPoint = _dish.level < 1 ? _nonNFTChips : _nftChips;

        RemainingTime = _time;

        transform.parent = null;
        int _randomNum = UnityEngine.Random.Range(0, 2);
        Enums.Direction _direction = Tile == null ? Enums.Direction.Forward : HelperFunctions.GetGlobalDirection((Tile.Neighbours[1 - _randomNum].transform.position - Tile.Neighbours[0 + _randomNum].transform.position).normalized, 0.95f);
        //Debug.Log($"RandomNum -> {_randomNum} | Order Direction -> {_direction}");
        transform.localScale = Vector3.one;
        countdown = 1;
        pauseCountdown = false;
        ToggleLocator(true);
        customerMaterial.material.mainTexture = customerTextures[UnityEngine.Random.Range(0, customerTextures.Length)];
        transform.SetPositionAndRotation(_tile.GetCenter() + GetOrderOffsetUsingBounds(_direction), Quaternion.identity);
    }

    private Vector3 GetOrderOffset(Enums.Direction _direction)
    {
        Vector3 _offset = Vector2.zero;
        switch (_direction)
        {
            case Enums.Direction.Forward:
                _offset = new Vector3(-5, 0, 0);
                customer.SetLocalPositionAndRotation(new Vector3(-2.8f, 0, 0), Quaternion.Euler(0, 90, 0));
                break;

            case Enums.Direction.Backward:
                _offset = new Vector3(5, 0, 0);
                customer.SetLocalPositionAndRotation(new Vector3(2.8f, 0, 0), Quaternion.Euler(0, -90, 0));
                break;

            case Enums.Direction.Left:
                _offset = new Vector3(0, 0, 5);
                customer.SetLocalPositionAndRotation(new Vector3(0, 0, 2.8f), Quaternion.Euler(0, 180, 0));
                break;

            case Enums.Direction.Right:
                _offset = new Vector3(0, 0, -5);
                customer.SetLocalPositionAndRotation(new Vector3(0, 0, -2.8f), Quaternion.Euler(0, 0, 0));
                break;
        }
        return _offset;
    }
    [field: SerializeField] public Vector3 center { get; private set; }
    [field: SerializeField] public Vector3 size { get; private set; }
    private Vector3 GetOrderOffsetUsingBounds(Enums.Direction _direction)
    {
        //center = Tile.transform.position;
        //if (Tile.TryGetComponent(out MeshRenderer _renderer))
        //{
        //    center = _renderer.bounds.center;
        //    size = _renderer.bounds.size;
        //}
        center = Tile.GetCenter();
        size = Tile.size;
        Vector3 _offset = Vector2.zero;
        float _yPos = center.y + (size.y);
        switch (_direction)
        {
            case Enums.Direction.Forward:
                _offset = new Vector3(size.x * -0.4f, _yPos, 0);
                customer.SetLocalPositionAndRotation(new Vector3(-2.8f, 0, 0), Quaternion.Euler(0, 90, 0));
                break;

            case Enums.Direction.Backward:
                _offset = new Vector3(size.x * 0.4f, _yPos, 0);
                customer.SetLocalPositionAndRotation(new Vector3(2.8f, 0, 0), Quaternion.Euler(0, -90, 0));
                break;

            case Enums.Direction.Left:
                _offset = new Vector3(0, _yPos, size.z * 0.4f);
                customer.SetLocalPositionAndRotation(new Vector3(0, 0, 2.8f), Quaternion.Euler(0, 180, 0));
                break;

            case Enums.Direction.Right:
                _offset = new Vector3(0, _yPos, size.z * -0.4f);
                customer.SetLocalPositionAndRotation(new Vector3(0, 0, -2.8f), Quaternion.Euler(0, 0, 0));
                break;
        }
        return _offset;
    }
    public void ToggleLocator(bool _isActive)
    {
        Color _color = locatorMaterial.material.color;
        _color.a = _isActive ? 1 : 0.4f;
        locatorMaterial.material.color = _color;

        _color = nftMaterial.material.color;
        _color.a = _isActive ? 1 : 0.4f;
        nftMaterial.material.color = _color;
        //locator.gameObject.SetActive(_flag);

        //if (!_isActive) characterAnimator.Play("CustomerClapping");
        //else characterAnimator.Stop("CustomerIdle");
    }


    [ContextMenu("PlayIdle")]
    private void PlayIdle() => customerAnimator.Play("CustomerIdle");
    [ContextMenu("PlayHappy")]
    private void PlayHappy() => customerAnimator.Play("CustomerClapping");

    public void PlayerCustomerHappyAnimation()
    {
        //customerAnimator.Play("CustomerClapping");
        locatorParent.SetActive(false);
        //OrderManager.Instance.RemoveOrder(this);
        Invoke("DestroyOrder", 0.1f);
    }

    public void SetThumbnail(Texture2D _thumb)
    {
        Thumbnail = _thumb;
        if (nftMaterial)
        {
            if(_thumb != null)
                nftMaterial.material.mainTexture = _thumb;
            else
            {
                nftMaterial.material.mainTexture = defaultDishTexture;
            }
        }
        
    }



    public float countdown;
    void Update()
    {
        if (GameData.Instance.IsGamePaused || pauseCountdown)
            return;


        if (collectingBy == Enums.OrderCollectingBy.None)
        {
            _deliveringTime = 0;
            if (countdown < 0)
            {
                RemainingTime--;
                OnOrderTimeUpdate?.Invoke(RemainingTime);
                if (RemainingTime < 1)
                {
                    pauseCountdown = true;
                    AnalyticsManager.Instance.FireOrderMissedEvent(GameController.Instance.LevelInfo.CurrentLevel.level, GameData.Instance.GetSelectedMapData().mapName, IsNFTOrder, Dish.name, Enums.OrderFailedReasonType.Time);
                    OnOrderFailed?.Invoke(this);

                    //OrderManager.Instance.RemoveOrder(this);
                    Invoke("DestroyOrder", 0.1f);
                }
                countdown = 1;
            }
            else countdown -= Time.deltaTime;
        }
        else
        {
            _deliveringTime += Time.deltaTime;
            OnOrderDeliveringTimeUpdate?.Invoke(_deliveringTime);
            if (_deliveringTime > deliveryingDelay)
            {
                pauseCountdown = true;
                // particles.gameObject.SetActive(false);
                GetComponent<Collider>().enabled = false;

                if (collectingBy == Enums.OrderCollectingBy.Player)
                    OnOrderCollected?.Invoke(this);
                else
                {
                    AnalyticsManager.Instance.FireOrderMissedEvent(GameController.Instance.LevelInfo.CurrentLevel.level, GameData.Instance.GetSelectedMapData().mapName, IsNFTOrder, Dish.name, Enums.OrderFailedReasonType.Rival);
                    OnOrderFailed?.Invoke(this);
                }

                //OrderManager.Instance.RemoveOrder(this);

                PlayerCustomerHappyAnimation();
            }
        }
    }

    public void DestroyOrder()
    {
        if (Tile != null) Tile.hasOrder = false;
        OrderManager.Instance.RemoveOrder(this);
        Destroy(gameObject);
    }




    [ContextMenu("GetTile_Debug")]
    private void GetTile_Debug()
    {
        if (Physics.Raycast(transform.position + Vector3.up, -transform.up, out RaycastHit _tileHit, 2, LayerAndTagManager.Instance.LayerRoadTile, QueryTriggerInteraction.Collide))
            Tile = _tileHit.collider.GetComponent<Tile>();
        else
            Tile = null;
    }
    [ContextMenu("SetOrderPosition")]
    private void SetOrderPosition()
    {
        int _randomNum = UnityEngine.Random.Range(0, 2);
        Enums.Direction _direction = HelperFunctions.GetGlobalDirection((Tile.Neighbours[1 - _randomNum].transform.position - Tile.Neighbours[0 + _randomNum].transform.position).normalized, 0.95f);
        transform.SetPositionAndRotation(Tile.GetCenter() + GetOrderOffsetUsingBounds(_direction), Quaternion.identity);
    }



}

[Serializable]
public class OrderNFTDetails
{
    public bool isNFT;
    public int nftLevel;
    public int nftPoint;
    public int tokenId;
}