using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OrderCompleteDisplay : MonoBehaviour
{
    [Tooltip("The text that says e.g. 'You plated Cooked Rice with 60s to spare.'")]
    public TMP_Text messageText;

    [Tooltip("Scene name/number to load for the next order (your gameplay scene)")]
    public string nextOrderSceneName = "2";

    [Tooltip("Scene name/number to load for the main menu")]
    public string menuSceneName = "1";

    void Start()
    {
        if (messageText != null)
        {
            int seconds = Mathf.FloorToInt(GameSession.remainingTime);
            messageText.text = $"You plated {GameSession.completedOrderName} with {seconds}s to spare.";
        }
    }

    // Hook to the NEXT ORDER button's OnClick()
    public void OnNextOrderPressed()
    {
        SceneManager.LoadScene(nextOrderSceneName);
    }

    // Hook to the BACK TO MENU button's OnClick()
    public void OnBackToMenuPressed()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
