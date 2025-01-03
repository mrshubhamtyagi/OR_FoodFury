using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhoneOption : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject selected;
    [SerializeField] private GameObject deSelected;
    [SerializeField] private GameObject OtpScreen;

    public void OnDeselect(BaseEventData eventData)
    {
        ToggleGameObjectSelection(false);

    }

    public void OnSelect(BaseEventData eventData)
    {
        ToggleGameObjectSelection(true);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
        ToggleGameObjectSelection(true);
        OtpScreen.gameObject.SetActive(true);
    }

    private void ToggleGameObjectSelection(bool selected)
    {
        this.selected.SetActive(selected);
        //    OtpScreen.SetActive(selected);
        this.deSelected.SetActive(!selected);
    }
}
