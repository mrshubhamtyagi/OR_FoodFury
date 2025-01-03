using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OneRare.FoodFury.Multiplayer
{
    
public class TextScroller : MonoBehaviour
{
    public RectTransform textContainer;
    public float fadeDuration = 0.5f;  // Time for fade-in and fade-out
    public float stayDuration = 1.5f;  // Time the text stays fully visible
    public float moveDuration = 1f;    // Time to move the text upward

    private TextMeshProUGUI[] texts;   
    private int currentIndex = 0;     

    void Start()
    {
        if (textContainer == null)
        {
            Debug.LogError("Text Container is not assigned!");
            return;
        }
        
        texts = textContainer.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length == 0)
        {
            Debug.LogError("No TextMeshProUGUI components found in the container!");
            return;
        }

        StartCoroutine(ScrollText());
    }

    private IEnumerator ScrollText()
    {
        yield return null;
        /*while (true)
        {

            TextMeshProUGUI currentText = texts[currentIndex];
            RectTransform currentRect = currentText.GetComponent<RectTransform>();

            currentText.alpha = 0;
            currentRect.anchoredPosition = Vector2.zero;
            
            float elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                currentText.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                yield return null;
            }

            currentText.alpha = 1;

            yield return new WaitForSeconds(stayDuration);

            elapsedTime = 0;
            Vector2 initialPosition = currentRect.anchoredPosition;
            Vector2 targetPosition = initialPosition + new Vector2(0, 100);

            while (elapsedTime < moveDuration)
            {
                elapsedTime += Time.deltaTime;
                currentRect.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration);
                currentText.alpha = Mathf.Lerp(1, 0, elapsedTime / moveDuration);

                yield return null;
            }

            currentText.alpha = 0;
            currentRect.anchoredPosition = targetPosition;
            currentIndex = (currentIndex + 1) % texts.Length;
        }*/
    }
}

}

