using FoodFury;
using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
public class AudioButton : Button
{
    //public override void OnPointerUp(PointerEventData eventData)
    //{
    //    UnityEngine.Debug.Log("Up " + eventData.selectedObject);

    //}
    //public override void OnPointerDown(PointerEventData eventData)
    //{
    //    UnityEngine.Debug.Log("Down " + eventData.selectedObject);

    //}

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (interactable) AudioManager.Instance.PlayButtonClicked();
        base.OnPointerClick(eventData);
    }

}
