using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KetchupStains : MonoBehaviour
{
    [SerializeField] private Image centerStain;
    [SerializeField] private Image leftStain;
    [SerializeField] private Image rightStain;
    [SerializeField] private Image rightDownStain;
    [SerializeField] private float maxDuration = 2f;

    private void OnEnable()
    {
        InitializeStains();
        StartCoroutine(AnimateStains());
    }

    private void InitializeStains()
    {
        SetScale(centerStain.rectTransform, Vector3.zero);
        SetScale(leftStain.rectTransform, Vector3.zero);
        SetScale(rightStain.rectTransform, Vector3.zero);
        SetScale(rightDownStain.rectTransform, Vector3.zero);
    }

    private void SetScale(RectTransform rectTransform, Vector3 scale)
    {
        rectTransform.localScale = scale;
    }

    private IEnumerator AnimateStains()
    {
        TweenStain(centerStain.rectTransform, Vector3.one, 0.1f, 0.0f, Ease.OutBack);
        yield return new WaitForSeconds(0.05f);
        TweenStain(leftStain.rectTransform, Vector3.one, 0.1f, 0.05f, Ease.OutBack);
        yield return new WaitForSeconds(0.05f);
        TweenStain(rightStain.rectTransform, Vector3.one, 0.1f, 0.1f, Ease.OutBack);
        yield return new WaitForSeconds(0.05f);
        TweenStain(rightDownStain.rectTransform, Vector3.one, 0.1f, 0.15f, Ease.OutBack);

        yield return new WaitForSeconds(maxDuration);

        // Scale down stains
        TweenStain(centerStain.rectTransform, Vector3.zero, 0.1f, 0.0f, Ease.OutBack);
        yield return new WaitForSeconds(0.05f);
        TweenStain(leftStain.rectTransform, Vector3.zero, 0.1f, 0.0f, Ease.OutBack);
        yield return new WaitForSeconds(0.05f);
        TweenStain(rightStain.rectTransform, Vector3.zero, 0.1f, 0.0f, Ease.OutBack);
        yield return new WaitForSeconds(0.05f);
        TweenStain(rightDownStain.rectTransform, Vector3.zero, 0.1f, 0.0f, Ease.OutBack);
        gameObject.SetActive(false);
    }

    private void TweenStain(RectTransform rectTransform, Vector3 targetScale, float duration, float delay, Ease ease)
    {
        rectTransform.DOScale(targetScale, duration).SetDelay(delay).SetEase(ease);
    }
}
