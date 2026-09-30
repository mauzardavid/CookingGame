using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Tooltip("Total time given for this order, in seconds. Change this per difficulty level (e.g. 300 = 5 minutes, 180 = 3 minutes for Hard mode).")]
    public float timeLimit = 300f;

    [Tooltip("The TMP Text placeholder that shows the countdown, e.g. the 'TIME' box")]
    public TMP_Text timerText;

    [Tooltip("Scene to load when time runs out")]
    public string timesUpSceneName = "4";

    private float timeRemaining;
    private bool isRunning = true;

    void Start()
    {
        timeRemaining = timeLimit;
        UpdateDisplay();
    }

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            UpdateDisplay();
            SceneManager.LoadScene(timesUpSceneName);
            return;
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }

    // Call this the moment an order is completed successfully, to freeze the countdown
    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetRemainingSeconds()
    {
        return timeRemaining;
    }
}
