using UnityEngine.UI;

using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Collections;

public class TimeFetcher : MonoBehaviour
{
    public InputField cityInput; // Legacy InputField for user to input city name
    public TMP_Text countdownDisplay; // Display the countdown to midnight

    private string geocodeApiKey = "9ee7976575464d1db4c61d6f33efe836"; // Replace with your OpenCage API key
    private string timeApiKey = "rIXmIwpStNLltaB9WQXYeQ==0rfZrVZNtYbq7VcN"; // Your WorldTimeAPI key
    private bool countdownActive = false; // Flag to check if countdown is active
    private DateTime targetTime; // Target time (midnight on December 31st)
    private Coroutine countdownCoroutine; // Reference to the countdown coroutine
    private bool isAppLoaded = false; // Flag to track if the app is first loaded

    public GameObject screen;      // Assign the "screen" GameObject in the Inspector
    public GameObject videoPlayer; // Assign the "video player" GameObject in the Inspector

    public void DisplayFireworks()
    {
        
        // Set the active state of the objects
        if (screen != null)
        {
            screen.SetActive(true);
        }

        if (videoPlayer != null)
        {
            videoPlayer.SetActive(true);
        }
    }


    // Called when the application regains focus after being minimized
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && isAppLoaded) // Only call FetchTime after the app is resumed
        {
            // Call FetchTime() when the app is reopened and has focus
            FetchTime();
        }

        // Set the flag to true after the first focus (app loaded)
        if (!isAppLoaded)
        {
            isAppLoaded = true;
        }
    }

    // Called when the application is paused or resumed (when minimized)
    private void OnApplicationPause(bool isPaused)
    {
        if (!isPaused && isAppLoaded) // Only call FetchTime when the app is resumed after being paused
        {
            // Call FetchTime() when the app is resumed
            FetchTime();
        }
    }



    public void FetchTime()
    {
        if (cityInput != null)
        {
            string city = cityInput.text.Trim();

            if (!string.IsNullOrEmpty(city))
            {
                // Clear the previous countdown and stop any active countdown coroutine
                if (countdownCoroutine != null)
                {
                    StopCoroutine(countdownCoroutine);
                }

                countdownDisplay.text = "FETCHING TIME..."; // Display a loading message or reset text

                // Step 1: Fetch latitude and longitude for the city
                string geocodeUrl = "https://api.opencagedata.com/geocode/v1/json?q=" + UnityWebRequest.EscapeURL(city) + "&key=" + geocodeApiKey;
                StartCoroutine(GetCoordinates(geocodeUrl));
            }
            else
            {
                countdownDisplay.text = "PLEASE ENTER A VALID CITY NAME";
            }
        }
    }

    // Coroutine to fetch coordinates from OpenCage API
    private IEnumerator GetCoordinates(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            countdownDisplay.text = "ERROR FETCHING COORDINATES";
        }
        else
        {
            // Parse the geocoding response
            string jsonResponse = request.downloadHandler.text;
            GeocodeResponse geocodeData = JsonUtility.FromJson<GeocodeResponse>(jsonResponse);

            if (geocodeData.results.Length > 0)
            {
                double lat = geocodeData.results[0].geometry.lat;
                double lon = geocodeData.results[0].geometry.lng;

                Debug.Log("Coordinates: latitude " + lat.ToString() + ", longitude " + lon.ToString());

                // Step 2: Use coordinates to fetch the time
                string timeUrl = "https://api.api-ninjas.com/v1/worldtime?lat=" + lat + "&lon=" + lon;
                StartCoroutine(GetTimeFromAPI(timeUrl));
            }
            else
            {
                countdownDisplay.text = "CITY NOT FOUND";
            }
        }
    }

    // Coroutine to fetch the current time using the coordinates from the WorldTime API
    private IEnumerator GetTimeFromAPI(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Api-Key", timeApiKey);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            countdownDisplay.text = "ERROR FETCHING TIME";
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            TimeData timeData = JsonUtility.FromJson<TimeData>(jsonResponse);

            if (timeData != null)
            {
                // Debug log the current time
                Debug.Log("Current Time in " + cityInput.text + ": " + timeData.datetime);

                // Step 3: Calculate the countdown to midnight, December 31st
                DateTime currentTime = DateTime.Parse(timeData.datetime); // Parse current time
                targetTime = new DateTime(currentTime.Year, 12, 31, 23, 59, 59); // Set December 31st midnight
                TimeSpan timeDifference = targetTime - currentTime;

                // Start the countdown
                countdownCoroutine = StartCoroutine(UpdateCountdown(timeDifference));
               
            }
            else
            {
                countdownDisplay.text = "FAILED TO PARSE TIME DATA";
            }
        }
    }

    // Coroutine to update the countdown every second
    private IEnumerator UpdateCountdown(TimeSpan initialTime)
    {
        TimeSpan timeRemaining = initialTime;

        while (timeRemaining.TotalSeconds > 0)
        {
            // Update the countdown display text
            countdownDisplay.text = FormatTimeSpan(timeRemaining);

            // Wait for 1 second before updating again
            yield return new WaitForSeconds(1f);

            // Decrease the time remaining by one second
            timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
        }

        // Once the countdown reaches zero, display "Happy New Year"
        countdownDisplay.text = "HAPPY NEW YEAR!!!";
        DisplayFireworks();
    }

    // Format the TimeSpan to include days along with hours, minutes, and seconds
    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return string.Format("TILL NEW YEAR:\n{0}:{1:D2}:{2:D2}:{3:D2}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    [System.Serializable]
    public class GeocodeResponse
    {
        public GeocodeResult[] results;
    }

    [System.Serializable]
    public class GeocodeResult
    {
        public GeocodeGeometry geometry;
    }

    [System.Serializable]
    public class GeocodeGeometry
    {
        public double lat;
        public double lng;
    }

    [System.Serializable]
    public class TimeData
    {
        public string datetime;
    }
}
