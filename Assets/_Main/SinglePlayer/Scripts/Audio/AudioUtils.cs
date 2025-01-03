using DG.Tweening;
using FoodFury;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class AudioUtils
{
    public static AudioSource SetAudioSourceDataUsingSoundSO(AudioSource audioSource, SoundSO soundSO)
    {
        audioSource.clip = soundSO.Clip;
        audioSource.volume = soundSO.Volume;
        audioSource.pitch = soundSO.Pitch;
        audioSource.outputAudioMixerGroup = soundSO.AudioMixerGroup;
        audioSource.spatialBlend = soundSO.is3DSound ? 1 : 0;
        if (soundSO.is3DSound)
        {
            audioSource.minDistance = soundSO.minMaxRangeOf3DEffect.x;
            audioSource.maxDistance = soundSO.minMaxRangeOf3DEffect.y;
        }
        audioSource.volume += soundSO.RandomizeVolume ? UnityEngine.Random.Range(soundSO.RandomVolumeThreshold.x, soundSO.RandomVolumeThreshold.y) : 0;
        audioSource.pitch += soundSO.RandomizePitch ? UnityEngine.Random.Range(soundSO.RandomPitchThreshold.x, soundSO.RandomPitchThreshold.y) : 0;
        audioSource.loop = soundSO.Loop;
        return audioSource;
    }

    public static void PlayOneShotAudio(SoundSO sound, Vector3 position = default)
    {
        AudioSource audioSource = CreateAudioSource(sound, position);
        //audioSource.volume = 0.6f;
        audioSource.Play();
        GameObject.Destroy(audioSource.gameObject, sound.Clip.length);
    }

    public static AudioSource CreateAudioSource(SoundSO sound, Vector3 position)
    {
        GameObject go = new GameObject(sound.Clip.name);
        if (position != default) go.transform.position = position;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource = SetAudioSourceDataUsingSoundSO(audioSource, sound);
        return audioSource;
    }

    public static void AddModulesToAction<T>(List<AudioModuleWithHint<T>> audioModules, ref Action<T> action)
    {
        foreach (AudioModuleWithHint<T> module in audioModules)
        {
            action += module.module.Apply;
        }
    }
    public static void RemoveModulesFromAction<T>(List<AudioModuleWithHint<T>> audioModules, ref Action<T> action)
    {
        foreach (AudioModuleWithHint<T> module in audioModules)
        {
            action -= module.module.Apply;
        }
    }
    public static void FadeOut(AudioSource audioSource, float fadeDuration)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource . Cannot fade audio.");
            return;
        }

        // Fade in the audio clip using DOTween
        audioSource.DOFade(0f, fadeDuration);
    }
    public static AudioSource SetAudioSourceDataUsingAudioData(AudioSource audioSource, ModelClass.AudioData audioData)
    {
        audioSource.clip = audioData.AudioClip;
        audioSource.volume = audioData.Volume;
        audioSource.pitch = audioData.Pitch;
        audioSource.outputAudioMixerGroup = audioData.AudioMixerGroup;
        audioSource.spatialBlend = audioData.Is3DSound ? 1 : 0;
        if (audioData.Is3DSound)
        {
            audioSource.minDistance = audioData.MinMaxRangeOf3DEffect.x;
            audioSource.maxDistance = audioData.MinMaxRangeOf3DEffect.y;
        }
        audioSource.volume += audioData.RandomizeVolume ? UnityEngine.Random.Range(audioData.RandomVolumeThreshold.x, audioData.RandomVolumeThreshold.y) : 0;
        audioSource.pitch += audioData.RandomizePitch ? UnityEngine.Random.Range(audioData.RandomPitchThreshold.x, audioData.RandomPitchThreshold.y) : 0;
        audioSource.loop = audioData.Loop;
        return audioSource;
    }
}
