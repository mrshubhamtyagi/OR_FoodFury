using FoodFury;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[RequireComponent(typeof(AudioSource))]
public class StopAudioModule : AudioModule<AudioMixerGroup>
{
    private AudioSource audioSource;
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        AudioManager.Instance.OnStopAudioForMixerGroup.AddListener(Apply);
    }
    private void OnDisable()
    {
        AudioManager.Instance.OnStopAudioForMixerGroup.RemoveListener(Apply);
    }
    public override void Apply(AudioMixerGroup value)
    {
        if (audioSource.outputAudioMixerGroup == value ||
            value.audioMixer.FindMatchingGroups(audioSource.outputAudioMixerGroup.name).Length > 0)

        {
            audioSource.Stop();
        }
    }
}
