using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FoodFury
{
    public class MunchOnlyPrize : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI munchText;
        // Start is called before the first frame update
        public void SetData(int munches) => munchText.text = munches.ToString();
    }
}
