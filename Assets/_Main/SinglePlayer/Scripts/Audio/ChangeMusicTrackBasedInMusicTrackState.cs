using FoodFury;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ChangeMusicTrackBasedInMusicTrackState : AudioModule<bool>
{
    [SerializeField] private ModelClass.AudioData HomeSceneMusic;
    [SerializeField] private ModelClass.AudioData GameplaySceneMusic;
    private AudioSource audioSource;
    //associate track with enum
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void SetGameplayMusic(AudioClip GameplayMusic)
    {
        GameplaySceneMusic.AudioClip = GameplayMusic;
    }
    public override void Apply(bool _playHomescreenMusic)
    {
        ModelClass.AudioData track = (_playHomescreenMusic) ? HomeSceneMusic : GameplaySceneMusic;

        if (track.AudioClip == null || audioSource == null)
        {
            audioSource = AudioUtils.SetAudioSourceDataUsingAudioData(audioSource, HomeSceneMusic);
            audioSource.Play();
            Debug.Log("No Track Found");
            return;
        }

        if (audioSource.clip == track.AudioClip)
        {
            // Debug.Log("Already Playing");
            return;
        }

        audioSource = AudioUtils.SetAudioSourceDataUsingAudioData(audioSource, track);
        audioSource.Play();

    }
}

[Serializable]
public enum MusicTrackState
{
    HomeScreen,
    Gameplay
}

[Serializable]
public class MusicTrackInfo
{
    public SoundSO Audio;
    public MusicTrackState state;
}