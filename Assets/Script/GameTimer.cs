using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Tooltip("Set automatically from the dish by CraftingManager")]
    public float timeLimit = 300f;
    public TMP_Text timerText;
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

    // Takes seconds off the clock (used by the hint button). Time's Up is triggered by Update when it reaches 0.
    public void SubtractTime(float seconds)
    {
        timeRemaining = Mathf.Max(0f, timeRemaining - seconds);
        UpdateDisplay();
    }

    public void StopTimer() { isRunning = false; }
    public float GetRemainingSeconds() { return timeRemaining; }
}