using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class VolumeModulationBasedOnFloatVariableModule : AudioModule<float>
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
    public override void Apply(float value)
    {
        currentValue = modulationCurve.Evaluate(value);
        audioSource.volume += (currentValue - previousModulationValue);
        previousModulationValue = currentValue;
    }

}
