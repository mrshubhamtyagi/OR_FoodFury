using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "FoodFury/Sound/UISoundCollection")]
public class UISFXSoundCollectionSO : ScriptableObject
{
    public SoundSO ButtonClickSoundSO;
    public SoundSO UpgradeSoundSO;
    public SoundSO WarningSoundSO;
    public SoundSO CoinCollectedSoundSO;
    public SoundSO LevelSuccessFulSoundSO;
    public SoundSO LevelFailedSoundSO;
    public SoundSO GarageSoundSO;
    public SoundSO FuelAddButtonSoundSO;
}
