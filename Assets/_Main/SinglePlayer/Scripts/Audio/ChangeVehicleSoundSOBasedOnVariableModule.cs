using UnityEngine;

public class ChangeVehicleSoundSOBasedOnVariableModule : AudioModule<bool>
{
    [SerializeField] private EngineSoundSO normalEngineSound;
    [SerializeField] private EngineSoundSO brokenEngineSound;
    private EngineSoundSO currentEngineSoundSO;
    [SerializeField] private AudioSource engineStartingSoundAudioSource;
    [SerializeField] private AudioSource engineAcceleratingSoundAudioSource;
    private bool isVehicleBroken;

    private void OnEnable()
    {
        currentEngineSoundSO = null;
        isVehicleBroken = false;
    }
    // Start is called before the first frame update
    public override void Apply(bool value)
    {
        //if (currentEngineSoundSO == ((value ? brokenEngineSound : normalEngineSound)))
        //{
        //    return;
        //}
        isVehicleBroken = value;
        UpdateEngineSounds();
        engineAcceleratingSoundAudioSource.Play();
        engineStartingSoundAudioSource.Play();

    }

    private void UpdateEngineSounds()
    {
        currentEngineSoundSO = isVehicleBroken ? brokenEngineSound : normalEngineSound;
        AudioUtils.SetAudioSourceDataUsingSoundSO(engineStartingSoundAudioSource, currentEngineSoundSO.StartingEngineSound);
        AudioUtils.SetAudioSourceDataUsingSoundSO(engineAcceleratingSoundAudioSource, currentEngineSoundSO.InBetweenEngineSound);
    }
    public void SetNormalEngineSound(EngineSoundSO normalEngineSound)
    {
        this.normalEngineSound = normalEngineSound;
        UpdateEngineSounds();
    }
    public void SetBrokenEngineSound(EngineSoundSO brokenEngineSound)
    {
        this.brokenEngineSound = brokenEngineSound;
        UpdateEngineSounds();
    }
}
