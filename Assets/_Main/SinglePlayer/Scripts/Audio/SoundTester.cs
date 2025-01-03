using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTester : MonoBehaviour
{
    [SerializeField] private SoundSO soundSOToTest;
    [SerializeField] private AudioSource audioSource;


    private void Awake() => gameObject.SetActive(Application.isEditor);

    [ContextMenu("Play Audio")]
    public void PlaySound()
    {
        audioSource.Stop();
        audioSource = AudioUtils.SetAudioSourceDataUsingSoundSO(audioSource, soundSOToTest);
        audioSource.Play();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlaySound();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            StopAudio();
        }
    }
    [ContextMenu("Stop Audio")]
    public void StopAudio()
    {
        audioSource.Stop();
    }
}
