using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private List<WeaponIconData> weaponIcons;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private float maxYPosition = 600.2f;
    [SerializeField] private WeaponIconData weaponIcon;
    [SerializeField] private AudioButton weaponButton;
    [SerializeField] private SoundSO slotSpinSO;
    [SerializeField] private AudioSource slotAudiosource;
    private Tween contentTween;
    private bool isSoundFaded;

    public async Task SpinAsync(WeaponType weaponType, Action onComplete)
    {
        WeaponIconData destination = weaponIcons.FirstOrDefault(x => x.weaponType == weaponType);
        await SpinCoroutineAsync(4, destination);
        onComplete?.Invoke();
    }

    private async Task SpinCoroutineAsync(int loopCount, WeaponIconData destination)
    {
        contentTransform.anchoredPosition = Vector2.zero;
        weaponButton.interactable = false;
        isSoundFaded = false;
        slotAudiosource = AudioUtils.SetAudioSourceDataUsingSoundSO(slotAudiosource, slotSpinSO);
        slotAudiosource.Play();

        for (int i = 0; i < loopCount; i++)
        {
            await contentTransform.DOAnchorPosY(maxYPosition, 0.5f).SetEase(Ease.Linear).AsyncWaitForCompletion();
            contentTransform.anchoredPosition = Vector2.zero;
        }

        contentTween = contentTransform.DOAnchorPosY(destination.contentYPosition, 0.5f).SetEase(Ease.OutCirc).OnUpdate(UpdateVolumeFade);
        await contentTween.AsyncWaitForCompletion();
        MakeWeaponButtonInteractable();
    }
    void UpdateVolumeFade()
    {
        if (contentTween == null)
        {
            return;
        }
        // Calculate the completion percentage of the tween
        float completionPercentage = contentTween.ElapsedPercentage();
        Debug.Log("compleletion percentage " + completionPercentage);
        // Check if it's time to start fading the volume
        if (completionPercentage >= 0.5f && isSoundFaded == false)
        {
            AudioUtils.FadeOut(slotAudiosource, 0.25f);
            isSoundFaded = true;
        }
    }

    private void MakeWeaponButtonInteractable()
    {
        weaponButton.interactable = true;

    }
    [ContextMenu("Spin")]
    public async void Spin()
    {
        //  WeaponIconData destination = weaponIcons.FirstOrDefault(x => x.weaponType == weaponType);
        await SpinCoroutineAsync(4, weaponIcon);
    }
}