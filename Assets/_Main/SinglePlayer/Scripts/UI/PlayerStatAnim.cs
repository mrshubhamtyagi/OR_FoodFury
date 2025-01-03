using FoodFury;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatAnim : MonoBehaviour
{

    public void Init(Sprite _sprite, float _duration, Transform _parent)
    {
        GetComponent<Image>().sprite = _sprite;
        transform.SetParent(_parent);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinCollected();

        TweenHandler.UIPosition(GetComponent<RectTransform>(), Vector2.zero, _duration, ScreenManager.Instance.TweenEase,
            () => { Destroy(gameObject); });
    }



}
