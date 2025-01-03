
using System;
using System.Linq;
using FoodFury;
using Fusion;
using Newtonsoft.Json;
using OneRare.FoodFury.Multiplayer;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.CoreLibrary;


namespace OneRare.FoodFury.Multiplayer
{
    public class OrderMP : NetworkBehaviour, ICollidable
    {
        [Header("---General Settings")]
        [SerializeField] private Material orderMaterial;
        [SerializeField] private Image loadingIndicator;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private GameObject locatorParent;
        [SerializeField] private Renderer locatorMaterial;
        [SerializeField] private Renderer nftMaterial;
        [SerializeField] private SpriteRenderer minimapIcon;
        [SerializeField] private SoundSO collectSound;
        public GameObject orderUIItemPrefab;
        [Header("---Customer")]
        [SerializeField] public Transform customer;
        //[SerializeField] public Renderer customerMaterial;
        //[SerializeField] private Texture2D[] customerTextures;
        
        public static event Action<OrderMP> OnOrderFailed;
        public event Action<OrderMP> OnOrderCollected;
        public event Action<int> OnOrderTimeUpdate;
        public event Action<float> OnOrderDeliveringTimeUpdate;
        
        public OrderNFTDetails OrderNFTDetails { get; private set; }
        public ModelClass.Dish Dish { get; private set; }
        public Texture2D Thumbnail { get; private set; }
        public bool IsNFTOrder => OrderNFTDetails.isNFT;
        public int NFTPoint => OrderNFTDetails.nftPoint;
        public int NFTOrderTokenId => OrderNFTDetails.tokenId;
        private Animation customerAnimator;
        [field: SerializeField] public Tile Tile { get; private set; }
        
        [Header("---Debugs")]
        public bool pauseCountdown = true;
        [SerializeField] float _deliveringTime = 0;
        [SerializeField] float _deliveryTimeout = 1f;
        public Enums.OrderCollectingBy collectingBy = Enums.OrderCollectingBy.None;
        private WaitForSeconds waitFor1Sec = new WaitForSeconds(1);
        private float totalWaitTime = 0.02f;
        private float currentWaitTime;
        private ChangeDetector _changeDetector;
        private GameManager gameManager;
        [Networked] public bool IsCollecting { get; set; }
        [Networked] public float TimeInsideTrigger { get; set; } 
        [Networked] public TickTimer RemainingTime { get; private set; }
        [Networked] public int RemainingTimeInSeconds { get; set; } = 0;
        [Networked] public int ContainerIndex { get; set; }
        [Networked] public bool IsEventRaised { get; set; }
        public bool IsSpawned { get; private set; }

        
        public override void Spawned()
        {
            base.Spawned();
            IsSpawned = true;
            IsCollecting = false;
            IsEventRaised = false;
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            currentWaitTime = totalWaitTime;
            customerAnimator = GetComponentInChildren<Animation>();
            RemainingTime = TickTimer.CreateFromSeconds(Runner, 60);
        }
        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(IsCollecting):
                    {
                        SetIsCollectingFlag(IsCollecting);
                        break;
                    }
                    case nameof(TimeInsideTrigger):
                    {
                        SetTimeInsideTrigger(TimeInsideTrigger);
                        break;
                    }
                    case nameof(RemainingTimeInSeconds):
                    {
                        SetRemainingTimeInSeconds(RemainingTimeInSeconds);
                        OnOrderTimeUpdate?.Invoke(RemainingTimeInSeconds);
                        break;
                    }
                    case nameof(ContainerIndex):
                    {
                        SetContainerIndex(ContainerIndex);
                        break;
                    }
                    case nameof(IsEventRaised):
                    {
                        SetIsEventRaised(IsEventRaised);
                        break;
                    }
                        
                }
            }
            
        }
        
        public static int GetPlayerRank(Player player)
        {
            var orderedPlayers = Player.Players
                .OrderByDescending(p => p.OrderCount)
                .ToList();

            // Rank is the index in the ordered list + 1 (since index is 0-based)
            int rank = orderedPlayers.IndexOf(player) + 1;

            return rank;
        }
        
        public int remainingTimeForOrder;
        public override void FixedUpdateNetwork()
        {
            if (RemainingTime.Expired(Runner) == false && RemainingTime.RemainingTime(Runner).HasValue)
            {
                var timeSpan = TimeSpan.FromSeconds(RemainingTime.RemainingTime(Runner).Value);
                var outPut = $"{timeSpan.Seconds:D2}";
                remainingTimeForOrder = Convert.ToInt32(outPut);
                RemainingTimeInSeconds = remainingTimeForOrder;
            }
            else if(!pauseCountdown)
            {
                pauseCountdown = true;
                OnOrderFailed?.Invoke(this);
                if(Object.HasStateAuthority)
                    ChallengeManager.Instance.HandleOrderExpiration(this);
            }
        }
        
        public void SetTileData(Tile tile, int dishTokenId)
        {
            Tile = tile;
            Tile.hasOrder = this;
            transform.parent = null;
            int _randomNum = dishTokenId % 2;
            Enums.Direction _direction = HelperFunctions.GetGlobalDirection((Tile.Neighbours[1 - _randomNum].transform.position - Tile.Neighbours[0 + _randomNum].transform.position).normalized, 0.95f);
            transform.SetPositionAndRotation(tile.transform.position + GetOrderOffset(_direction), Quaternion.identity);
        }

        public void SetDishData(ModelClass.Dish dish)
        {
            Dish = dish;
            OrderNFTDetails = new();
            OrderNFTDetails.isNFT = GameData.Instance.CheckUserHasDish(Dish.tokenId, out OrderNFTDetails.tokenId);
            OrderNFTDetails.nftLevel = IsNFTOrder ? GameData.Instance.GetUserDishByToken(OrderNFTDetails.tokenId).level : dish.level;
    
            pauseCountdown = false;
            ToggleLocator(true);
    
            Texture2D dishImage = DishImages.LoadDishImage(dish.tokenId);
            this.SetThumbnail(dishImage);
            if (Object.HasStateAuthority)
            {
                UIManager.Instance.RPC_OnNewOrderGenerated(this);
                UIManager.Instance.RPC_RedrawLeaderboardList();
            }
            
            /*StartCoroutine(APIManager.GetDishTexture(dish.tokenId, (_status, _texture) =>
            {
                if (_status) this.SetThumbnail(_texture);
                else if (PopUpManager.Instance != null)
                {
                    Debug.Log("Could not load dish image!");
                    this.SetThumbnail(ChallengeManager.Instance.defaultDishImage);
                }

                if (Object.HasStateAuthority)
                {
                    UIManager.Instance.RPC_OnNewOrderGenerated(this);
                    UIManager.Instance.RPC_RedrawLeaderboardList();
                }
            }));*/
            OrderUIItem cardUI = UIManager.Instance.orderCardContainer[ContainerIndex];
            cardUI.SetOrder(this);
            gameObject.name = dish.name;
           // customerMaterial.material.mainTexture = customerTextures[dish.tokenId % customerTextures.Length];
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
        
        public void ToggleLocator(bool _isActive)
        {
            Color _color = locatorMaterial.material.color;
            _color.a = _isActive ? 1 : 0.4f;
            locatorMaterial.material.color = _color;

            _color = nftMaterial.material.color;
            _color.a = _isActive ? 1 : 0.4f;
            nftMaterial.material.color = _color;
        }
        
        [ContextMenu("PlayIdle")]
        private void PlayIdle() => customerAnimator.Play("CustomerIdle");
        [ContextMenu("PlayHappy")]
        private void PlayHappy() => customerAnimator.Play("CustomerClapping");

        public void PlayerCustomerHappyAnimation()
        {
            customerAnimator.Play("CustomerClapping");
            locatorParent.SetActive(false);
            //OrderManager.Instance.RemoveOrder(this);
            //Invoke("DestroyOrder", 3f);
        }

        public void SetThumbnail(Texture2D _thumb)
        {
            Thumbnail = _thumb;
            if (nftMaterial) nftMaterial.material.mainTexture = _thumb;
        }
        
        void SetTimeInsideTrigger(float time)
        {
            TimeInsideTrigger = time;
        }
        
        void SetRemainingTimeInSeconds(int time)
        {
            RemainingTimeInSeconds = time;
        }
        
        private void SetIsCollectingFlag(bool value)
        {
            IsCollecting = value;
        }

        private void SetIsEventRaised(bool value)
        {
            IsEventRaised = value;
        }
        
        public void SetTileAndDish(int tileIndex, int dishTokenId)
        {
            Tile tile = TileManager.Instance.GetTileByIndex(tileIndex);
            //ModelClass.Dish receivedDish = null;
            SetTileData(tile, dishTokenId);
            if (GameData.Instance.CheckUserHasDish(dishTokenId, out int finalToken))
            {
                SetDishData (GameData.Instance.GetUserDishByToken(finalToken));
                return;
            }
           
            SetDishData (ChallengeManager.Instance.GetDishData(dishTokenId));
        }
        
        private void SetNextOrder()
        {
            ChallengeManager.Instance.HandleOrderExpiration(this);
        }
        
        public ModelClass.Dish DeserializeDish(string json)
        {
            return JsonConvert.DeserializeObject<ModelClass.Dish>(json);
        }

        public void SetMinimapIconColor( Color iconColor)
        {
            minimapIcon.color = iconColor;
        }
        
        public void SetContainerIndex(int containerIndex)
        {
            ContainerIndex = containerIndex;
        }

        private Color GetOrderColor()
        {
            Color orderColor = Color.white;
            int colorIndex = ContainerIndex % 3;
            switch (colorIndex)
            {
                case 0:
                    orderColor = ColorManager.Instance.Violet;
                    break;
                case 1:
                    orderColor = ColorManager.Instance.Orange;
                    break;
                case 2:
                    orderColor = ColorManager.Instance.Pink;
                    break;
            }

            return orderColor;
        }

        public void Collide(Player player)
        {
            Debug.Log($"Player{player.Username}");
            gameObject.SetActive(false);
            Player collidedplayer = player;
            if (!collidedplayer)
                return;
            
            IsCollecting = true;

            int powerup = 0;
            
            Color materialColor = orderMaterial.color;
            materialColor.a = 0.5f; 
            orderMaterial.color = materialColor;
            currentWaitTime = totalWaitTime;
            if (loadingIndicator != null)
            {
                loadingIndicator.fillAmount = 1; 
            }
            
            if (!IsEventRaised)
            {
                IsEventRaised = true;
               
                if (!collidedplayer.IsBot)
                {
                    collidedplayer.RaiseEvent(new Player.PickupEvent { elapsedTime = RemainingTimeInSeconds , nftLevel = OrderNFTDetails.nftLevel, nftPoint = OrderNFTDetails.nftPoint});
                }
                else
                {
                    collidedplayer.OnOrderCollectByBots(RemainingTimeInSeconds, OrderNFTDetails.nftLevel, OrderNFTDetails.nftPoint);
                }
            }
            UIManager.Instance.ShowOrderCollected(collidedplayer.Username.ToString(), GetOrderColor());
            SetNextOrder();
        }


        public void DespawnAfterDelay()
        {
            Invoke("DespawnOrder",1f);
        }
        void DespawnOrder()
        {
            if (HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            IsSpawned = false;
        }
    }
    
    
}

