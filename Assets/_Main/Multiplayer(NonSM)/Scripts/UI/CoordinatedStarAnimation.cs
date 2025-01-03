using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
public class CoordinatedStarAnimation : MonoBehaviour
{
    public Transform[] stars; // Array to hold the star Transforms
    public float animationDuration = 1f; // Duration of one grow/shrink cycle
    public float scaleMultiplier = 1.1f; // Target scale for the animation
    public float delayBetweenAnimations = 0.5f; // Delay between the start of animations for different stars

    private Vector3[] initialScales; // Array to store the initial scales of the stars
    private List<Sequence> starSequences = new List<Sequence>();

    void Start()
    {
        if (stars.Length > 0)
        {
            initialScales = new Vector3[stars.Length];

            // Store the initial scale of each star
            for (int i = 0; i < stars.Length; i++)
            {
                initialScales[i] = stars[i].localScale;
            }

            AnimateStars();
        }
    }

    private void AnimateStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            Transform star = stars[i];
            Vector3 initialScale = initialScales[i];

            // Create the grow and shrink animations
            Sequence starSequence = DOTween.Sequence();

            // Add a delay so the stars don't animate at the same time
            starSequence.AppendInterval(i * delayBetweenAnimations);

            // Start the grow animation
            starSequence.Append(star.DOScale(initialScale * scaleMultiplier, animationDuration).SetEase(Ease.InOutQuad));

            // Then the shrink animation
            starSequence.Append(star.DOScale(initialScale, animationDuration).SetEase(Ease.InOutQuad));

            // Loop the sequence indefinitely
            starSequence.SetLoops(-1, LoopType.Restart);

            // Add the sequence to the list
            starSequences.Add(starSequence);
        }
    }

    private void OnDestroy()
    {
        // Safely kill all sequences
        foreach (var sequence in starSequences)
        {
            sequence.Kill();
        }

        starSequences.Clear();
    }

    private void OnDisable()
    {
        // Safely kill all sequences
        foreach (var sequence in starSequences)
        {
            sequence.Kill();
        }

        starSequences.Clear();
    }
}

}
