using UnityEngine;
using TMPro;

public class FontSizeAdjuster : MonoBehaviour
{
    public TMP_Text textObject; // Assign your TextMeshPro text object in the Inspector
    private const int SmallFontSize = 50;
    private const int NormalFontSize = 100;

    private void Start()
    {
        AdjustFontSize();
    }

    private void AdjustFontSize()
    {
        if (textObject != null)
        {
            // Check the screen width and adjust font size
            if (Screen.width < 1000)
            {
                textObject.fontSize = SmallFontSize;
                Debug.Log("Using small font size...");
            }
            else
            {
                textObject.fontSize = NormalFontSize;
                Debug.Log("Using standard font size...");
            }
        }
        else
        {
            Debug.LogError("TextObject reference is not assigned in the Inspector.");
        }
    }

    
}

