using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace FoodFury
{
    [DefaultExecutionOrder(-11)]
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Mixer Groups")]
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup masterGroup;
        [Header("AudioMixer SnapShot")]
        [SerializeField] private AudioMixerSnapshot muteSnapShot;
        [SerializeField] private AudioMixerSnapshot normalSnapShot;

        [Header("Audio Source")]
        [SerializeField] private AudioSource musicAudioSource;

        [Header("Sound Collection ")]
        [SerializeField] private UISFXSoundCollectionSO uiSoundCollectionSO;

        [Header("Achievement Audio Sound")]
        [SerializeField] private SoundSO achievementSoundSO;

        [Header("UnityEvents")]
        public UnityEvent<AudioMixerGroup> OnStopAudioForMixerGroup;

        [SerializeField] private ChangeMusicTrackBasedInMusicTrackState changeMusicTrackBasedInMusicTrackState;

        public static AudioManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            SetMusicVol(PlayerPrefsManager.Instance.MusicVol);
            SetSfxVol(PlayerPrefsManager.Instance.SfxVol);
        }



        public void PlayUpgradeSound() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.UpgradeSoundSO);

        public void PlayCoinCollected() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.CoinCollectedSoundSO);

        public void PlayWarningSound() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.WarningSoundSO);

        public void PlayButtonClicked() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.ButtonClickSoundSO);

        public void PlayLevelSuccess() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.LevelSuccessFulSoundSO);

        public void PlayLevelFailed() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.LevelFailedSoundSO);
        public void PlayGarageButtonSound() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.GarageSoundSO);
        public void PlayAddFuelButtonSound() => AudioUtils.PlayOneShotAudio(uiSoundCollectionSO.FuelAddButtonSoundSO);

        public void SetGamePlayMusicAudio(AudioClip Clip) => changeMusicTrackBasedInMusicTrackState.SetGameplayMusic(Clip);
        public void PlayHomeScreenTrack()
        {
            if (changeMusicTrackBasedInMusicTrackState == null)
                changeMusicTrackBasedInMusicTrackState = gameObject.GetComponent<ChangeMusicTrackBasedInMusicTrackState>();

            changeMusicTrackBasedInMusicTrackState.Apply(true);
        }

        public void PlayGameplayTrack() => changeMusicTrackBasedInMusicTrackState.Apply(false);
        public void StopSFX() => OnStopAudioForMixerGroup.Invoke(sfxGroup);
        public void SetMusicVol(float _value) => musicGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp((_value / 10) * 0.7f, 0.0001f, 1f)) * 20f);
        public void SetSfxVol(float _value) => sfxGroup.audioMixer.SetFloat("SFXVolume", 5 + Mathf.Log10(Mathf.Clamp((_value / 10), 0.0001f, 1)) * 20f);
        public void PlayAchievementSound() => AudioUtils.PlayOneShotAudio(achievementSoundSO);

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                musicAudioSource.UnPause();
                normalSnapShot.TransitionTo(0);
            }
            else
            {
                musicAudioSource.Pause();
                muteSnapShot.TransitionTo(0);
            }
        }


    }

}