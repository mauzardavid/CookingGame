using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [System.Serializable]
    public class Recipe
    {
        public string ingredientA;
        public string ingredientB;
        public string resultName; // must match the order name text exactly, e.g. "Cooked Rice"
        public Sprite resultSprite; // the image shown after the '=' sign when this recipe is completed
        [TextArea] public string hint; // shown once when the player presses the HINT button for this order
    }

    [Header("Recipes")]
    public List<Recipe> recipes = new List<Recipe>();

    [Header("Slots (the two Image objects on the table, before '+' and before '=')")]
    public Image slot1Icon;
    public Image slot2Icon;

    [Header("Result (the Image object after the '=' sign)")]
    public Image resultIcon;

    [Header("Order")]
    [Tooltip("The TMP Text showing the current order name at the top, e.g. 'Cooked Rice'")]
    public TMP_Text orderNameText;

    [Header("Feedback")]
    public TMP_Text feedbackText;
    public float feedbackDuration = 1.2f;
    public string correctMessage = "Correct!";
    public string wrongMessage = "Wrong Recipe";

    [Header("Timer & Scene Transitions")]
    public GameTimer gameTimer;
    public string orderCompleteSceneName = "3";
    [Tooltip("How long the 'Correct!' message stays on screen before switching scenes")]
    public float sceneTransitionDelay = 1.5f;

    [Header("Hint (usable once per order)")]
    public TMP_Text hintDisplayText;
    public Button hintButton;
    private bool hintUsed = false;

    private string slot1Ingredient = "";
    private string slot2Ingredient = "";
    private IngredientButton slot1Button;
    private IngredientButton slot2Button;

    void Awake()
    {
        Instance = this;
        SetSlotIcon(slot1Icon, null);
        SetSlotIcon(slot2Icon, null);
        SetSlotIcon(resultIcon, null);
        ResetIngredientState();
    }

    // Called by IngredientButton when tapped
    public void TryPlaceIngredient(IngredientButton ib)
    {
        if (string.IsNullOrEmpty(slot1Ingredient))
        {
            SetSlotIcon(resultIcon, null); // clear the previous dish's result image for the new round
            slot1Ingredient = ib.ingredientName;
            SetSlotIcon(slot1Icon, ib.ingredientIcon);
            slot1Button = ib;
            ib.SetUsed(true);
        }
        else if (string.IsNullOrEmpty(slot2Ingredient))
        {
            slot2Ingredient = ib.ingredientName;
            SetSlotIcon(slot2Icon, ib.ingredientIcon);
            slot2Button = ib;
            ib.SetUsed(true);
        }
        // both slots already full -> tap is ignored
    }

    void SetSlotIcon(Image slotImage, Sprite icon)
    {
        if (slotImage == null) return;
        slotImage.sprite = icon;
        slotImage.enabled = icon != null;
    }

    // Hook this to the COMBINE button's OnClick()
    public void OnCombinePressed()
    {
        if (string.IsNullOrEmpty(slot1Ingredient) || string.IsNullOrEmpty(slot2Ingredient))
            return; // need two ingredients first

        Recipe match = FindRecipe(slot1Ingredient, slot2Ingredient);
        string currentOrder = orderNameText.text.Trim();

        if (match != null && match.resultName == currentOrder)
        {
            SetSlotIcon(resultIcon, match.resultSprite);
            ShowFeedback(correctMessage);
            OnOrderCompleted();

            float remaining = gameTimer != null ? gameTimer.GetRemainingSeconds() : 0f;
            if (gameTimer != null) gameTimer.StopTimer();

            GameSession.remainingTime = remaining;
            GameSession.completedOrderName = match.resultName;

            StartCoroutine(LoadSceneAfterDelay(orderCompleteSceneName, sceneTransitionDelay));
            return;
        }
        else
        {
            ShowFeedback(wrongMessage);
        }

        ResetIngredientState();
    }

    // Hook to the HINT button's OnClick()
    public void OnHintPressed()
    {
        if (hintUsed) return;

        string currentOrder = orderNameText.text.Trim();
        Recipe match = recipes.Find(r => r.resultName == currentOrder);

        if (match != null && hintDisplayText != null)
        {
            hintDisplayText.text = match.hint;
            hintDisplayText.gameObject.SetActive(true);
        }

        hintUsed = true;
        if (hintButton != null) hintButton.interactable = false;
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    Recipe FindRecipe(string a, string b)
    {
        foreach (Recipe r in recipes)
        {
            if ((r.ingredientA == a && r.ingredientB == b) ||
                (r.ingredientA == b && r.ingredientB == a))
            {
                return r;
            }
        }
        return null;
    }

    void ResetIngredientState()
    {
        slot1Ingredient = "";
        slot2Ingredient = "";

        if (slot1Button != null) slot1Button.SetUsed(false);
        if (slot2Button != null) slot2Button.SetUsed(false);
        slot1Button = null;
        slot2Button = null;
    }

    void ShowFeedback(string message)
    {
        if (feedbackText == null) return;
        StopAllCoroutines();
        StartCoroutine(FeedbackRoutine(message));
    }

    IEnumerator FeedbackRoutine(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.gameObject.SetActive(false);
    }

    void OnOrderCompleted()
    {
        // Called automatically when the correct ingredients are combined.
        // Plug in your own logic here: next order, add score, play a sound, etc.
    }
}