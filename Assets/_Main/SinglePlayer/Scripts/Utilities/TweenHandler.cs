using DG.Tweening;
using DG.Tweening.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TweenHandler
{
    public static void TransformScale(Transform _transform, Vector3 _value, float _duration, Ease _ease, Action _callback = null) => _transform.DOScale(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void TransformPosition(Transform _transform, Vector3 _value, float _duration, Ease _ease, Action _callback = null) => _transform.DOLocalMove(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void UIPosition(RectTransform _transform, Vector2 _value, float _duration, Ease _ease, Action _callback = null) => _transform.DOAnchorPos(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void UISize(RectTransform _transform, Vector2 _value, float _duration, Ease _ease, Action _callback = null) => _transform.DOSizeDelta(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void ImageFade(Image _image, float _value, float _duration, Ease _ease, Action _callback = null) => _image.DOFade(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void ImageFillAmount(Image _image, float _value, float _duration, Ease _ease, Action _callback = null) => _image.DOFillAmount(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void TextColor(TMPro.TextMeshProUGUI _text, Color _value, float _duration, Ease _ease, Action _callback = null) => _text.DOColor(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void ImageColor(Image _image, Color _value, float _duration, Ease _ease, Action _callback = null) => _image.DOColor(_value, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void FloatValue(float _from, float _to, float _duration, Ease _ease, TweenCallback<float> _OnUpdate = null, Action _callback = null) => DOVirtual.Float(_from, _to, _duration, _OnUpdate).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void Shake(Transform _transforms, float _duration, float _strength, int _vibrato, Ease _ease, Action _callback = null) => _transforms.DOShakePosition(_duration, _strength, _vibrato).SetEase(_ease).OnComplete(() => _callback?.Invoke());

    public static void CanvasGroupAlpha(CanvasGroup canvasGroup, float _endValue, float _duration, Ease _ease, Action _callback = null) => canvasGroup.DOFade(_endValue, _duration).SetEase(_ease).OnComplete(() => _callback?.Invoke());
    public static void StopTween() => DOTween.KillAll();
}
