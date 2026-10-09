using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance;

    [Header("Dishes")]
    [Tooltip("Used when you press Play directly in this scene (no area was chosen on the area-select scene). Drag Luzon here for testing.")]
    public AreaData defaultArea;
    [Tooltip("Last fallback: type the dishes by hand if you use neither AreaData")]
    public List<Dish> dishes = new List<Dish>();

    [Header("Choice buttons (created from the dish data)")]
    public IngredientButton choiceButtonPrefab;
    public RectTransform choicesContainer;

    [Header("Table")]
    public Image slot1Icon;
    public Image slot2Icon;
    public Image resultIcon;

    [Header("Order")]
    public TMP_Text orderNameText;
    [Tooltip("Optional: shows the region of origin of the current dish")]
    public TMP_Text originText;
    [Tooltip("Optional: shows 'Dish 1 / 5'")]
    public TMP_Text progressText;

    [Header("Feedback")]
    public TMP_Text feedbackText;
    public float feedbackDuration = 1.2f;
    public string correctMessage = "Correct!";
    public string stepMessage = "Nice!";
    public string wrongMessage = "Wrong Recipe";

    [Header("Timer & Scenes")]
    public GameTimer gameTimer;
    public string orderCompleteSceneName = "3";
    public float sceneTransitionDelay = 1.5f;

    [Header("Hint (once per dish)")]
    public TMP_Text hintDisplayText;
    public Button hintButton;

    private List<Dish> activeDishes;
    private Dish currentDish;
    private readonly HashSet<string> knownChoices = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    private string slot1Ingredient = "";
    private string slot2Ingredient = "";
    private IngredientButton slot1Button;
    private IngredientButton slot2Button;
    private bool hintUsed = false;
    private bool finished = false;
    private Coroutine feedbackRoutine;

    void Awake()
    {
        Instance = this;
        SetSlotIcon(slot1Icon, null);
        SetSlotIcon(slot2Icon, null);
        SetSlotIcon(resultIcon, null);

        // Dishes come from the chosen AreaData if there is one, otherwise from this scene's list
        AreaData area = GameSession.currentArea != null ? GameSession.currentArea : defaultArea;
        activeDishes = (area != null && area.dishes.Count > 0) ? area.dishes : dishes;

        if (activeDishes == null || activeDishes.Count == 0)
        {
            Debug.LogError("CraftingManager: no dishes. Fill the Dishes list or assign an AreaData on the area button.");
            return;
        }

        GameSession.areaSceneName = SceneManager.GetActiveScene().name;
        GameSession.totalDishes = activeDishes.Count;

        int index = Mathf.Clamp(GameSession.dishIndex, 0, activeDishes.Count - 1);
        currentDish = activeDishes[index];

        if (orderNameText != null) orderNameText.text = currentDish.dishName;
        if (originText != null) originText.text = currentDish.origin;
        if (progressText != null) progressText.text = "Dish " + (index + 1) + " / " + activeDishes.Count;
        if (gameTimer != null && currentDish.timeLimit > 0f) gameTimer.timeLimit = currentDish.timeLimit;
        if (hintDisplayText != null) hintDisplayText.gameObject.SetActive(false);

        BuildStartingChoices();
        ValidateDish();
    }

    // ---------- choices ----------
    void BuildStartingChoices()
    {
        knownChoices.Clear();

        if (choiceButtonPrefab != null && choicesContainer != null)
        {
            foreach (Transform child in choicesContainer)
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }

        foreach (IngredientDef c in currentDish.startingChoices)
            AddChoice(c.ingredientName, c.icon);
    }

    void AddChoice(string choiceName, Sprite icon)
    {
        string n = Norm(choiceName);
        if (n.Length == 0 || !knownChoices.Add(n)) return;
        if (choiceButtonPrefab == null || choicesContainer == null) return;

        IngredientButton ib = Instantiate(choiceButtonPrefab, choicesContainer);
        ib.gameObject.SetActive(true);
        ib.Setup(n, icon);
    }

    // Called by IngredientButton when tapped
    public void TryPlaceIngredient(IngredientButton ib)
    {
        if (finished || currentDish == null) return;

        if (string.IsNullOrEmpty(slot1Ingredient))
        {
            SetSlotIcon(slot2Icon, null);
            SetSlotIcon(resultIcon, null);
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
    }

    // ---------- buttons ----------
    // Hook to the COMBINE button
    public void OnCombinePressed()
    {
        if (finished || currentDish == null) return;
        if (string.IsNullOrEmpty(slot1Ingredient) || string.IsNullOrEmpty(slot2Ingredient)) return;

        RecipeStep step = FindStep(slot1Ingredient, slot2Ingredient);

        if (step == null)
        {
            ShowFeedback(wrongMessage);
            ResetIngredientState();
            ClearTable();
            return;
        }

        SetSlotIcon(resultIcon, step.resultSprite);

        if (Same(step.resultName, currentDish.dishName))
        {
            finished = true;
            ShowFeedback(correctMessage);

            float remaining = gameTimer != null ? gameTimer.GetRemainingSeconds() : 0f;
            if (gameTimer != null) gameTimer.StopTimer();

            GameSession.remainingTime = remaining;
            GameSession.completedOrderName = currentDish.dishName;

            StartCoroutine(LoadSceneAfterDelay(orderCompleteSceneName, sceneTransitionDelay));
            return;
        }

        ShowFeedback(stepMessage);
        AddChoice(step.resultName, step.resultSprite);
        ResetIngredientState();
    }

    // Hook to a CLEAR button: removes the picked ingredients from the table
    public void OnClearPressed()
    {
        if (finished) return;
        ResetIngredientState();
        ClearTable();
    }

    // Hook to the HINT button
    public void OnHintPressed()
    {
        if (hintUsed || currentDish == null) return;

        RecipeStep next = null;
        foreach (RecipeStep s in currentDish.steps)
        {
            if (!knownChoices.Contains(Norm(s.resultName))) { next = s; break; }
        }

        if (next != null && hintDisplayText != null)
        {
            hintDisplayText.text = next.hint;
            hintDisplayText.gameObject.SetActive(true);
        }

        hintUsed = true;
        if (hintButton != null) hintButton.interactable = false;
    }

    // ---------- helpers ----------
    RecipeStep FindStep(string a, string b)
    {
        foreach (RecipeStep s in currentDish.steps)
        {
            if ((Same(s.ingredientA, a) && Same(s.ingredientB, b)) ||
                (Same(s.ingredientA, b) && Same(s.ingredientB, a)))
                return s;
        }
        return null;
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    void SetSlotIcon(Image slotImage, Sprite icon)
    {
        if (slotImage == null) return;
        slotImage.sprite = icon;
        slotImage.enabled = icon != null;
    }

    void ClearTable()
    {
        SetSlotIcon(slot1Icon, null);
        SetSlotIcon(slot2Icon, null);
        SetSlotIcon(resultIcon, null);
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
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        feedbackRoutine = StartCoroutine(FeedbackRoutine(message));
    }

    IEnumerator FeedbackRoutine(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.gameObject.SetActive(false);
    }

    static string Norm(string s) { return (s ?? "").Trim(); }

    static bool Same(string x, string y)
    {
        return string.Equals(Norm(x), Norm(y), StringComparison.OrdinalIgnoreCase);
    }

    // Warns in the Console about typos or missing pieces in the dish data
    void ValidateDish()
    {
        var available = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (IngredientDef c in currentDish.startingChoices) available.Add(Norm(c.ingredientName));
        foreach (RecipeStep s in currentDish.steps) available.Add(Norm(s.resultName));

        bool hasFinalStep = false;
        foreach (RecipeStep s in currentDish.steps)
        {
            if (!available.Contains(Norm(s.ingredientA)))
                Debug.LogWarning("[" + currentDish.dishName + "] step '" + s.resultName + "': '" + s.ingredientA + "' is not a starting choice or step result.");
            if (!available.Contains(Norm(s.ingredientB)))
                Debug.LogWarning("[" + currentDish.dishName + "] step '" + s.resultName + "': '" + s.ingredientB + "' is not a starting choice or step result.");
            if (Same(s.resultName, currentDish.dishName)) hasFinalStep = true;
        }

        if (!hasFinalStep)
            Debug.LogWarning("[" + currentDish.dishName + "] no step has Result Name '" + currentDish.dishName + "', so this dish can never be completed.");
    }
}