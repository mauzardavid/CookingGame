using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OrderCompleteDisplay : MonoBehaviour
{
    public TMP_Text messageText;
    [Tooltip("The NEXT ORDER button object. Hidden after the last dish.")]
    public GameObject nextOrderButton;
    public string shiftCompleteLine = "Shift complete!";
    [Tooltip("Only used if no gameplay scene is remembered (testing this scene alone)")]
    public string nextOrderSceneName = "2";
    [Tooltip("Area-select scene")]
    public string menuSceneName = "1";

    void Start()
    {
        bool lastDish = GameSession.IsLastDish;

        if (messageText != null)
        {
            int seconds = Mathf.FloorToInt(GameSession.remainingTime);
            string msg = "You plated " + GameSession.completedOrderName + " with " + seconds + "s to spare.";
            if (lastDish && !string.IsNullOrEmpty(shiftCompleteLine))
                msg += "\n" + shiftCompleteLine;
            messageText.text = msg;
        }

        if (nextOrderButton != null) nextOrderButton.SetActive(!lastDish);
    }

    public void OnNextOrderPressed()
    {
        if (GameSession.IsLastDish) return;
        GameSession.dishIndex++;
        string scene = string.IsNullOrEmpty(GameSession.areaSceneName) ? nextOrderSceneName : GameSession.areaSceneName;
        SceneManager.LoadScene(scene);
    }

    public void OnBackToMenuPressed()
    {
        GameSession.StartNewShift();
        SceneManager.LoadScene(menuSceneName);
    }
}