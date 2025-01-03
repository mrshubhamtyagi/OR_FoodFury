using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ScreenShake : MonoBehaviour
{
    // Intensity of the shake effect
    public float shakeIntensity = 0.1f;

    // Duration of the shake effect
    public float shakeDuration = 0.5f;

    private Vector3 originalPosition;
    private float currentShakeDuration = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (currentShakeDuration > 0)
        {
            // Generate a random offset within the specified intensity
            Vector3 shakeOffset = UnityEngine.Random.insideUnitSphere * shakeIntensity;

            // Apply the offset to the camera's position in screen space
            Vector3 newPosition = Camera.main.WorldToScreenPoint(transform.localPosition + shakeOffset);
            transform.localPosition = Camera.main.ScreenToWorldPoint(newPosition);

            // Reduce the shake duration over time
            currentShakeDuration -= Time.deltaTime;
        }
        //else
        //{
        //    // Reset the camera position when the shake duration is over
        //    currentShakeDuration = 0f;
        //    transform.localPosition = originalPosition;
        //}
    }

    // Call this function to initiate the screen shake effect
    public void ShakeScreen(float duration)
    {
        currentShakeDuration = duration;
    }
    public void ShakeScreen()
    {
        currentShakeDuration = shakeDuration;
    }
}
