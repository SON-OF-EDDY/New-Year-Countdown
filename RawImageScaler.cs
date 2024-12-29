using UnityEngine;
using UnityEngine.UI;

public class RawImageScaler : MonoBehaviour
{
    public RawImage rawImage; // Assign your RawImage in the Inspector

    private void Start()
    {
        if (rawImage != null)
        {
            AdjustRawImageSizeAndPosition();
        }
        else
        {
            Debug.LogError("RawImage reference is not assigned in the Inspector.");
        }
    }

    private void AdjustRawImageSizeAndPosition()
    {
        // Get the screen height
        float screenHeight = Screen.height;

        // Calculate the new size (48.5% of screen height)
        float newSize = screenHeight * 0.485f;

        // Adjust the RawImage's RectTransform size
        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(newSize, newSize);

        // Position it at the top-center of the screen
        rectTransform.anchorMin = new Vector2(0.5f, 1f); // Top-center anchor
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f); // Top-center pivot
        rectTransform.anchoredPosition = new Vector2(0, 0); // Align at the top
    }
}
