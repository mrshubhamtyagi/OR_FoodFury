
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneRare.FoodFury.Multiplayer
{
    public class OrderCollectedItem : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public Image orderIcon;
        public void SetResult(string name, Color orderColor)
        {
            nameText.text = name;
            orderIcon.color = orderColor;
            Destroy(gameObject, 2f);
        }
    }
}
