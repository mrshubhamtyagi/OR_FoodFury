using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "FoodFury/Sound/SoundSO"), Serializable]
public class SoundSO : ScriptableObject
{
    public AudioClip Clip;
    public AudioMixerGroup AudioMixerGroup;
    [Header("Options")]
    public bool Loop;
    [Range(0f, 1f)]
    public float Volume = 0.5f;
    [Range(-3f, 3f)]
    public float Pitch = 1;
    public bool is3DSound;
    //[MinMax(1, 2000)]
    public Vector2 minMaxRangeOf3DEffect;
    [Header("Randomization Options")]
    public bool RandomizeVolume;
    //[MinMax(-1, 1)]
    public Vector2 RandomVolumeThreshold;
    public bool RandomizePitch;
    //[MinMax(-2, 2)]
    public Vector2 RandomPitchThreshold;
}
