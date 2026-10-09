using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class IngredientButton : MonoBehaviour
{
    public string ingredientName;
    public Sprite ingredientIcon;

    [Header("Optional")]
    [Tooltip("Text that shows the name. If empty, the first TMP Text inside the button is used.")]
    public TMP_Text label;
    [Tooltip("Picture on the button itself")]
    public Image iconImage;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);
    }

    void OnClicked()
    {
        CraftingManager.Instance.TryPlaceIngredient(this);
    }

    // Called by CraftingManager when it creates a choice button
    public void Setup(string ingredient, Sprite icon)
    {
        ingredientName = ingredient;
        ingredientIcon = icon;

        TMP_Text text = label != null ? label : GetComponentInChildren<TMP_Text>(true);
        if (text != null) text.text = ingredient;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }
    }

    public void SetUsed(bool used)
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null) button.interactable = !used;
    }
}