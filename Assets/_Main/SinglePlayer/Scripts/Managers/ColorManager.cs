using UnityEngine;

namespace FoodFury
{
    public class ColorManager : MonoBehaviour
    {
        public static ColorManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null) Instance = this;
        }

        [field: SerializeField] public Color Yellow { get; private set; }
        [field: SerializeField] public Color Red { get; private set; }
        [field: SerializeField] public Color Blue { get; private set; }
        [field: SerializeField] public Color Cyan { get; private set; }
        [field: SerializeField] public Color Orange { get; private set; }
        [field: SerializeField] public Color LightGreen { get; private set; }
        [field: SerializeField] public Color DarkGreen { get; private set; }
        [field: SerializeField] public Color LightGrey { get; private set; }
        [field: SerializeField] public Color DarkGrey { get; private set; }
        [field: SerializeField] public Color DarkerGrey { get; private set; }
        [field: SerializeField] public Color DarkestGrey { get; private set; }
        [field: SerializeField] public Color Pink { get; private set; }
        [field: SerializeField] public Color Violet { get; private set; }
    }
}
