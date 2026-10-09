using System.Collections.Generic;
using UnityEngine;

// One selectable choice (e.g. "Ground Pork" + its picture)
[System.Serializable]
public class IngredientDef
{
    [Tooltip("Must match the names used in the steps, e.g. 'Ground Pork'")]
    public string ingredientName;
    public Sprite icon;
}

// One alchemy step: ingredientA + ingredientB -> resultName
[System.Serializable]
public class RecipeStep
{
    public string ingredientA;
    public string ingredientB;
    [Tooltip("The new choice this step creates, e.g. 'Garlic Pork'")]
    public string resultName;
    [Tooltip("Picture shown after the '=' sign and on the new choice")]
    public Sprite resultSprite;
    [TextArea] public string hint;
}

// One dish of a shift
[System.Serializable]
public class Dish
{
    [Tooltip("Shown as the order name. The LAST step's Result Name must be exactly this.")]
    public string dishName;
    [Tooltip("Region of origin, e.g. 'Quezon - CALABARZON (IV-A)'")]
    public string origin;
    [Tooltip("Time for this dish, in seconds")]
    public float timeLimit = 300f;
    [Tooltip("Choices shown at the start (decoys allowed)")]
    public List<IngredientDef> startingChoices = new List<IngredientDef>();
    [Tooltip("Every step needed to make the dish")]
    public List<RecipeStep> steps = new List<RecipeStep>();
}