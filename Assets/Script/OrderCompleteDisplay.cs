using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Order Complete screen: dish picture on the left, place of origin and trivia on the right,
// NEXT ORDER and BACK TO MENU at the bottom.
public class OrderCompleteDisplay : MonoBehaviour
{
    [Header("Left side")]
    [Tooltip("The picture of the finished dish")]
    public Image dishImage;

    [Header("Right side")]
    public TMP_Text dishNameText;
    public TMP_Text originText;
    public TMP_Text triviaText;

    [Header("Bottom")]
    [Tooltip("Title on the banner, e.g. 'Order Complete!'")]
    public TMP_Text titleText;
    [Tooltip("e.g. 'You plated Bulalo with 60s to spare.'")]
    public TMP_Text messageText;
    [Tooltip("The NEXT ORDER button object. Hidden after the last dish.")]
    public GameObject nextOrderButton;

    [Header("Texts")]
    public string orderCompleteTitle = "Order Complete!";
    public string shiftCompleteTitle = "Shift Complete!";

    [Header("Scenes")]
    [Tooltip("Only used if no gameplay scene is remembered (testing this scene alone)")]
    public string nextOrderSceneName = "Luzon";
    [Tooltip("Area-select scene (type its exact name)")]
    public string menuSceneName = "1";

    void Start()
    {
        bool lastDish = GameSession.IsLastDish;

        if (dishImage != null)
        {
            dishImage.sprite = GameSession.completedSprite;
            dishImage.preserveAspect = true;
            dishImage.enabled = GameSession.completedSprite != null;
        }

        if (dishNameText != null) dishNameText.text = GameSession.completedOrderName;
        if (originText != null) originText.text = GameSession.completedOrigin;
        if (triviaText != null) triviaText.text = GameSession.completedTrivia;

        if (titleText != null) titleText.text = lastDish ? shiftCompleteTitle : orderCompleteTitle;

        if (messageText != null)
        {
            int seconds = Mathf.FloorToInt(GameSession.remainingTime);
            messageText.text = "You plated " + GameSession.completedOrderName + " with " + seconds + "s to spare.";
        }

        // After the last dish only BACK TO MENU remains
        if (nextOrderButton != null) nextOrderButton.SetActive(!lastDish);
    }

    // Hook to the NEXT ORDER button's OnClick()
    public void OnNextOrderPressed()
    {
        if (GameSession.IsLastDish) return;
        GameSession.dishIndex++;
        string scene = string.IsNullOrEmpty(GameSession.areaSceneName) ? nextOrderSceneName : GameSession.areaSceneName;
        SceneManager.LoadScene(scene);
    }

    // Hook to the BACK TO MENU button's OnClick()
    public void OnBackToMenuPressed()
    {
        GameSession.StartNewShift();
        SceneManager.LoadScene(menuSceneName);
    }
}