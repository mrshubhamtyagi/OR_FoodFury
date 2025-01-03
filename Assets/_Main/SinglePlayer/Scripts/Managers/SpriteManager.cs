using UnityEngine;

namespace FoodFury
{
    public class SpriteManager : MonoBehaviour
    {
        //public Sprite DefaultDish;
        //public Sprite DefaultProfile;

        public Sprite chips;
        public Sprite deliver;

        [Header("---Emojis")]
        public Sprite greenEmoji;
        public Sprite yellowEmoji;
        public Sprite redEmoji;


        [Header("---Leaderboard")]
        public Sprite orare;
        public Sprite coupon;
        public Sprite dish;
        public Sprite ingredient;

        public Sprite plus;
        public Sprite minus;

        public Sprite plusBooster;
        public Sprite tickBooster;


        [Header("---Boosters")]
        public Sprite speedBooster;
        public Sprite engineBooster;
        public Sprite shieldBooster;
        public Sprite noBotBooster;

        [Header("---Others")]
        public Sprite check;
        public Sprite lockSprite;
        public Sprite unlockSprite;

        [Header("---Sequences")]
        public Sprite[] startAnimSeq;
        public Sprite[] confettiAnimSeq;


        public static SpriteManager Instance;
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }
    }
}
