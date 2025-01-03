using UnityEngine;

[CreateAssetMenu(menuName = "FoodFury/Sound/VehicleSoundCollections")]
public class VehicleSoundCollectionSO : ScriptableObject
{
    public SoundSO Crash;
    public SoundSO Drift;
    public SoundSO LowFuel;
    public SoundSO LowHealth;
    public SoundSO DeliveryComplete;
    public SoundSO DeliveryFailed;
    public SoundSO BoosterCollected;
}
