using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class IngredientButton : MonoBehaviour
{
    [Tooltip("Must match the ingredient name used in CraftingManager's recipe list, e.g. 'Egg', 'Water', 'Rice'")]
    public string ingredientName;

    [Tooltip("The image that will appear on the table slot when this ingredient is picked")]
    public Sprite ingredientIcon;

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

    // Called by CraftingManager to lock/unlock this button
    public void SetUsed(bool used)
    {
        button.interactable = !used;
    }
}