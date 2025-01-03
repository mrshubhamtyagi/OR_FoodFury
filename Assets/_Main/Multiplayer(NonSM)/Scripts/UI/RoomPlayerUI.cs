using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OneRare.FoodFury.Multiplayer
{
    public class RoomPlayerUI : MonoBehaviour
    {
	    public Image playerProfilePic;
	    public TextMeshProUGUI nameText;
	    public Image namePlateImage;
	    public RectTransform profilePicRect;
	    
	    private Tween animationTween;
	    
	    private readonly Color[] _playerHelmetColors = new Color[]
	    {
		    new Color32(0xFF, 0xBF, 0xC2, 0xFF), // Red (FFBFC2)
		    new Color32(0x94, 0xD5, 0xFF, 0xFF), // Blue (94D5FF)
		    new Color32(0xFF, 0xF8, 0xA8, 0xFF), // Yellow (FFF8A8)
		    new Color32(0x8E, 0xF1, 0xBE, 0xFF)  // Green (8EF1BE)
	    };
	    
	    private readonly Color[] _namePlateUIColors = new Color[]
	    {
		    new Color32(0xEF, 0x5A, 0x5F, 0xFF), // Red (FFBFC2)
		    new Color32(0x45, 0xB4, 0xFF, 0xFF), // Blue (94D5FF)
		    new Color32(0xDA, 0xCF, 0x4B, 0xFF), // Yellow (FFF8A8)
		    new Color32(0x38, 0xC7, 0x7A, 0xFF)  // Green (8EF1BE)
	    };
	    
	
	    public void SetPlayerProfile(string name, int index)//, Texture profilePic)
	    {
		    nameText.text = name;
		    
		    if (index >= 0 && index < _playerHelmetColors.Length)
		    {
			    playerProfilePic.color = _playerHelmetColors[index];
			    namePlateImage.color = _namePlateUIColors[index];
			    nameText.color = _namePlateUIColors[index];
		    }
		    else
		    {
			    Debug.LogWarning("Invalid index. Assigning default color.");
			    playerProfilePic.color = Color.white; // Default color
		    }
		    //playerProfilePic.texture = profilePic;
		    AnimateProfilePic();
	    }
	    
	    private void AnimateProfilePic()
	    {
		    // Store the initial Y position
		    float initialY = profilePicRect.anchoredPosition.y;

		    // Animate the image to move up and down
		    animationTween = profilePicRect.DOAnchorPosY(initialY + 25f, 1f) // Move up by 20 units in 1 second
			    .SetEase(Ease.InOutSine) // Smooth easing
			    .SetLoops(-1, LoopType.Yoyo); // Infinite up and down loop*/
	    }

	    private void OnDestroy()
	    {
		    // Kill the tween to prevent errors
		    if (animationTween != null)
		    {
			    animationTween.Kill();
		    }
	    }

	    private void OnDisable()
	    {
		    // Kill the tween to prevent errors
		    if (animationTween != null)
		    {
			    animationTween.Kill();
		    }
	    }
    }
}
