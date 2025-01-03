using FoodFury;

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterPrefab : MonoBehaviour
{
    public static event Action<Enums.LevelBoosterType> OnBoosterAdd;
    public static event Action<Enums.LevelBoosterType> OnBoosterRemove;

    [SerializeField] private Enums.LevelBoosterType type;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleTMP;
    [SerializeField] private TextMeshProUGUI costTMP;
    [SerializeField] private GameObject blockerObj;

    [SerializeField] private GameObject selected;

    private Image bg;
    // private Image addBtnImage;
    //private Image addBtnIcon;
    private Button addBtn;

    void Awake()
    {
        bg = GetComponent<Image>();
        addBtn = GetComponentInChildren<Button>();
        // addBtnImage = addBtn.GetComponent<Image>();
        //  addBtnIcon = addBtn.transform.GetChild(0).GetComponent<Image>();
        addBtn.onClick.AddListener(OnClick_Add);
    }

    public void SetCost(string _cost) => costTMP.text = _cost;

    private void OnEnable() => OnBoosterAdd += Event_OnAddBooster;
    private void OnDisable() => OnBoosterAdd -= Event_OnAddBooster;


    public void OnClick_Add()
    {
        if (GameController.Instance.LevelInfo.LevelBooster == type)
        {
            // Remove
            GameController.Instance.LevelInfo.LevelBooster = Enums.LevelBoosterType.None;
            // bg.sprite = notSelected;
            selected.SetActive(false);
            //  addBtnImage.color = Color.white;
            //  addBtnIcon.sprite = SpriteManager.Instance.plusBooster;
            OnBoosterRemove?.Invoke(type);
            return;
        }


        // Add
        selected.SetActive(true);
        // addBtnImage.color = ColorManager.Instance.Yellow;
        //  addBtnIcon.sprite = SpriteManager.Instance.tickBooster;
        OnBoosterAdd?.Invoke(type);

        GameController.Instance.LevelInfo.LevelBooster = type;
    }

    public void ResetButton()
    {
        addBtn.interactable = true;
        selected.SetActive(false);
        //  addBtnImage.color = Color.white;
        //  addBtnIcon.sprite = SpriteManager.Instance.plusBooster;
    }

    public void Event_OnAddBooster(Enums.LevelBoosterType _type)
    {
        if (_type == type) return;
        ResetButton();
    }

    public void DisableBooster(bool _disable) => blockerObj.SetActive(_disable);
}
