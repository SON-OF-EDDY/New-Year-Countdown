using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dropdown : MonoBehaviour
{

    public TextMeshProUGUI output;
    public System.DateTime targetTime;
    public System.TimeSpan chosenOffset;
    public System.TimeSpan moscowOffset;
    public System.TimeSpan sydneyOffset;
    public System.TimeSpan londonOffset;


    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            Debug.Log("london chosen...");
            chosenOffset = londonOffset;
        }
        else if (val == 1)
        {
            Debug.Log("moscow chosen...");
            chosenOffset = moscowOffset;
        }
        else if (val == 2)
        {
            Debug.Log("sydney chosen...");
            chosenOffset = sydneyOffset;
        }
    }


    void Start()
    {
        int nextYear = System.DateTime.Now.Year + 1;
        moscowOffset = System.TimeSpan.FromHours(3); // Moscow Standard Time (UTC+3)
        sydneyOffset = System.TimeSpan.FromHours(11); // Moscow Standard Time (UTC+3)
        londonOffset = System.TimeSpan.FromHours(0);
        targetTime = new System.DateTime(nextYear, 1, 1, 0, 0, 0) - londonOffset;
        Debug.Log("app initialized...");

    }

    void Update()
    {
        int nextYear = System.DateTime.Now.Year + 1;
        targetTime = new System.DateTime(nextYear, 1, 1, 0, 0, 0) - chosenOffset;
        UpdateCountdown();
    }

    void UpdateCountdown()
    {
        // Calculate the time difference between now and the target time for chosen time
        System.DateTime UTCNow = System.DateTime.UtcNow;
        System.TimeSpan timeDifference = targetTime - UTCNow;

        // Display the time difference in a formatted string
        string countdownString = $"{timeDifference.Days:D2}:{timeDifference.Hours:D2}:{timeDifference.Minutes:D2}:{timeDifference.Seconds:D2}";

        // Update the TextMeshPro text
        output.text = countdownString;
    }

}

