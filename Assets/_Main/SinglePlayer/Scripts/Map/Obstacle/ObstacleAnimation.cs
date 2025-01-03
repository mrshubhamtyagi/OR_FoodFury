using System;
using UnityEngine;

public class ObstacleAnimation : MonoBehaviour
{
    [SerializeField] private float from;
    [SerializeField] private float to;

    [Header("-----Tween")]
    [SerializeField] private float duration = 2;
    [SerializeField] private DG.Tweening.Ease ease;

    private void OnDisable()
    {
        transform.localRotation = Quaternion.Euler(from, 0, 0);
    }


    [ContextMenu("PlaySingleAnimation")]
    public void PlaySingleAnimation()
    {
        TweenHandler.FloatValue(from, to, duration, ease, (_value) => transform.localRotation = Quaternion.Euler(_value, 0, 0));
    }
}
