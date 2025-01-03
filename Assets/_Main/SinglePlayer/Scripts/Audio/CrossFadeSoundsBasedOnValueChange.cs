using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CrossFadeSoundsBasedOnValueChange : AudioModule<float>
{
    private AudioSource firstAudioSource;
    [SerializeField] private AudioSource secondAudioSound;
    [SerializeField] private AnimationCurve crossFadeCurve;
    private float originalFirstVolume = 0.5f;
    private float secondVolume = 0.5f;

    private void OnEnable()
    {
        firstAudioSource = GetComponent<AudioSource>();
    }


    public override void Apply(float value)
    {
        float blendFactor = crossFadeCurve.Evaluate(value);
        firstAudioSource.volume = originalFirstVolume * blendFactor;
        secondAudioSound.volume = secondVolume * (1.0f - blendFactor);
    }


}
