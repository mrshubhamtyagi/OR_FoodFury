using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class StarAnimation : MonoBehaviour
    {
        private Image image;
        private Vector2 startingPosition;
        private Vector2 startingScale;
        private Color startingColor;
        private Color fadedColor;
        [SerializeField] private Vector2 offsetDistance;
        Sequence mySequence;
        // Start is called before the first frame update
        RectTransform myRectTransform;
        private void OnEnable()
        {
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            if (image == null)
            {
                image = GetComponent<Image>();
                myRectTransform = GetComponent<RectTransform>();
                startingColor = image.color;
                fadedColor = startingColor;
                fadedColor.a = 0;
                startingPosition = myRectTransform.localPosition;
                startingScale = myRectTransform.localScale;
            }
            mySequence = DOTween.Sequence();
            float randomValue = AddSalt();
            mySequence.Append(image.DOColor(startingColor, 1f + randomValue).From(fadedColor));
            mySequence.Join(transform.DOLocalMove(new Vector3(0, 0, 360), 1f + randomValue).From(new Vector3(0, 0, 0)));
            mySequence.Join(transform.DOLocalMove(startingPosition + offsetDistance + new Vector2(0, AddSalt()), 1f + randomValue).From(startingPosition));
            mySequence.Join(transform.DOScale(Vector3.one, 1f + randomValue).From(Vector3.zero));
            randomValue = AddSalt();
            mySequence.Append(image.DOColor(fadedColor, 1f + randomValue).From(startingColor));
            mySequence.Join(transform.DOLocalMove(startingPosition + new Vector2(0, AddSalt()), 1f + randomValue).From(startingPosition + offsetDistance));
            mySequence.Join(transform.DOScale(Vector3.zero, 1 + randomValue).From(Vector3.one));
            mySequence.Join(transform.DORotate(new Vector3(0, 0, 0), 1f + randomValue).From(new Vector3(0, 0, 360)));

            myRectTransform.DORotate(new Vector3(0, 0, 360), 2, RotateMode.FastBeyond360)
                                 .SetLoops(-1, LoopType.Restart);
            // mySequence.Join(image.DOFade(0, 0));
            mySequence.SetLoops(-1);
            // mySequence.Play();
        }
        private void OnDisable()
        {
            if (mySequence != null && mySequence.IsPlaying())
            {
                mySequence.Kill();
                myRectTransform.DOKill();
            }
        }
        private float AddSalt()
        {
            return UnityEngine.Random.Range(-0.5f, 2f);
        }

    }
}
