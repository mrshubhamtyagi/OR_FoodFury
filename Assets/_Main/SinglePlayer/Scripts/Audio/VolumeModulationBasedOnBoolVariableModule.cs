using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeModulationBasedOnBoolVariableModule : AudioModule<bool>
{
    private AudioSource audioSource;
    private float previousModulationValue;
    private float currentValue;
    [SerializeField] private AnimationCurve modulationCurve;
    private void OnEnable()
    {
        previousModulationValue = 0;
        audioSource = GetComponent<AudioSource>();
    }
    public override void Apply(bool value)
    {
        currentValue = modulationCurve.Evaluate(value ? 1f : 0f);
        audioSource.volume += (currentValue - previousModulationValue);
        previousModulationValue = currentValue;
    }
}
