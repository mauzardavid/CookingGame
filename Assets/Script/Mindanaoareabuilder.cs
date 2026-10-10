#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Put this file in Assets/Editor.
// Menu: MixKitchen > Build Mindanao Area
// Creates Assets/Data/Mindanao.asset with all 5 Mindanao dishes (choices, steps, hints)
// and fills in the sprites by matching sprite NAMES. Sprites that are not found stay
// empty - assign those by hand afterwards. Running it again rebuilds the asset
// (it overwrites manual edits made to Mindanao.asset).
public static class MindanaoAreaBuilder
{
    // Where to look for your sliced sprites. Use "Assets/Sprites" to make it faster.
    const string SpriteFolder = "Assets";
    const string OutputFolder = "Assets/Data";
    const string OutputPath = "Assets/Data/Mindanao.asset";
    const float TimeLimit = 180f;   // Mindanao = Hard (change here if too tight)

    static Dictionary<string, Sprite> sprites;
    static readonly List<string> missing = new List<string>();

    [MenuItem("MixKitchen/Build Mindanao Area")]
    public static void Build()
    {
        LoadSprites();
        missing.Clear();

        var dishes = new List<Dish>();

        // 1. GRILLED TUNA
        dishes.Add(new Dish
        {
            dishName = "Grilled Tuna",
            origin = "General Santos City, South Cotabato - SOCCSKSARGEN (Region XII)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Tuna"), Ing("Salt"), Ing("Pepper"), Ing("Cooking Oil"), Ing("Garlic"), Ing("Chili"), Ing("Vinegar")
            },
            steps = new List<RecipeStep>
            {
                Step("Tuna", "Salt", "Salted Tuna", "Start by salting the fish."),
                Step("Salted Tuna", "Pepper", "Seasoned Tuna", "Add the black spice."),
                Step("Seasoned Tuna", "Cooking Oil", "Grilled Tuna", "Oil keeps the fish from sticking on the grill.")
            }
        });

        // 2. KINILAW NA ISDA
        dishes.Add(new Dish
        {
            dishName = "Kinilaw na Isda",
            origin = "Davao City - Davao Region (Region XI)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Fresh Fish"), Ing("Vinegar"), Ing("Onion"), Ing("Ginger"), Ing("Chili"), Ing("Salt"), Ing("Garlic"), Ing("Pepper")
            },
            steps = new List<RecipeStep>
            {
                Step("Fresh Fish", "Vinegar", "Vinegar Fish", "Kinilaw is 'cooked' by something sour, not by heat."),
                Step("Onion", "Ginger", "Onion-Ginger Mix", "Prepare the aromatics separately."),
                Step("Vinegar Fish", "Onion-Ginger Mix", "Kinilaw Mix", "Join the fish and the aromatics."),
                Step("Kinilaw Mix", "Chili", "Spicy Kinilaw", "Give it some heat."),
                Step("Spicy Kinilaw", "Salt", "Kinilaw na Isda", "Season it to finish.")
            }
        });

        // 3. TUNA KINILAW
        dishes.Add(new Dish
        {
            dishName = "Tuna Kinilaw",
            origin = "Davao City - Davao Region (Region XI)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Fresh Tuna"), Ing("Vinegar"), Ing("Onion"), Ing("Ginger"), Ing("Chili"), Ing("Salt"), Ing("Garlic"), Ing("Pepper")
            },
            steps = new List<RecipeStep>
            {
                Step("Fresh Tuna", "Vinegar", "Vinegar Tuna", "This time the fish is tuna. Soak it in something sour."),
                Step("Onion", "Ginger", "Onion-Ginger Mix", "Prepare the aromatics separately."),
                Step("Vinegar Tuna", "Onion-Ginger Mix", "Tuna Kinilaw Mix", "Join the tuna and the aromatics."),
                Step("Tuna Kinilaw Mix", "Chili", "Spicy Tuna", "Give it some heat."),
                Step("Spicy Tuna", "Salt", "Tuna Kinilaw", "Season it to finish.")
            }
        });

        // 4. SATTI  (chicken OR beef both work for the first step)
        dishes.Add(new Dish
        {
            dishName = "Satti",
            origin = "Zamboanga City - Zamboanga Peninsula (Region IX)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Chicken"), Ing("Beef"), Ing("Soy Sauce"), Ing("Garlic"), Ing("Cooking Oil"), Ing("Salt"), Ing("Pepper"), Ing("Vinegar")
            },
            steps = new List<RecipeStep>
            {
                Step("Chicken", "Soy Sauce", "Marinated Meat", "Satti is made with chicken or beef. Marinate it in the dark sauce."),
                Step("Beef", "Soy Sauce", "Marinated Meat", "Satti is made with chicken or beef. Marinate it in the dark sauce."),
                Step("Garlic", "Marinated Meat", "Garlic Meat", "Add the aromatic bulb."),
                Step("Garlic Meat", "Salt", "Seasoned Meat", "Season it."),
                Step("Seasoned Meat", "Pepper", "Peppered Meat", "Add the black spice."),
                Step("Peppered Meat", "Cooking Oil", "Satti", "Oil it before grilling the skewers.")
            }
        });

        // 5. GINATAANG ISDA
        dishes.Add(new Dish
        {
            dishName = "Ginataang Isda",
            origin = "Davao City - Davao Region (Region XI)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Fish"), Ing("Coconut Milk"), Ing("Garlic"), Ing("Onion"), Ing("Salt"), Ing("Cooking Oil"), Ing("Chili"), Ing("Ginger")
            },
            steps = new List<RecipeStep>
            {
                Step("Garlic", "Onion", "Garlic-Onion Mix", "Start with the two aromatics."),
                Step("Garlic-Onion Mix", "Cooking Oil", "Sauteed Mix", "Saute them in oil."),
                Step("Fish", "Sauteed Mix", "Seasoned Fish", "Add the fish to the pan."),
                Step("Seasoned Fish", "Coconut Milk", "Coconut Fish", "Ginataan means cooked in coconut milk."),
                Step("Coconut Fish", "Salt", "Ginataang Isda", "Season it to finish.")
            }
        });

        // Final step of each dish uses the finished-dish picture (same name as the dish)

        if (!AssetDatabase.IsValidFolder(OutputFolder))
            AssetDatabase.CreateFolder("Assets", "Data");

        AreaData area = AssetDatabase.LoadAssetAtPath<AreaData>(OutputPath);
        if (area == null)
        {
            area = ScriptableObject.CreateInstance<AreaData>();
            AssetDatabase.CreateAsset(area, OutputPath);
        }

        area.areaName = "Mindanao";
        area.dishes = dishes;
        DishTriviaFiller.Apply(area);
        EditorUtility.SetDirty(area);
        AssetDatabase.SaveAssets();
        Selection.activeObject = area;
        EditorGUIUtility.PingObject(area);

        if (missing.Count > 0)
        {
            Debug.LogWarning("Mindanao built, but these sprites were not found (assign them by hand or rename the sprites): "
                + string.Join(", ", new HashSet<string>(missing)));
        }
        else
        {
            Debug.Log("Mindanao built with all sprites found.");
        }
    }

    // ---------- helpers ----------
    static IngredientDef Ing(string name, params string[] alternateSpriteNames)
    {
        return new IngredientDef { ingredientName = name, icon = Find(name, alternateSpriteNames) };
    }

    static RecipeStep Step(string a, string b, string result, string hint)
    {
        return new RecipeStep
        {
            ingredientA = a,
            ingredientB = b,
            resultName = result,
            resultSprite = Find(result),
            hint = hint
        };
    }

    static Sprite Find(string name, params string[] alternates)
    {
        Sprite s;
        if (sprites.TryGetValue(Key(name), out s)) return s;
        foreach (string alt in alternates)
            if (sprites.TryGetValue(Key(alt), out s)) return s;

        missing.Add(name);
        return null;
    }

    static void LoadSprites()
    {
        sprites = new Dictionary<string, Sprite>();
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { SpriteFolder });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            foreach (Object o in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                Sprite sp = o as Sprite;
                if (sp == null) continue;
                string key = Key(sp.name);
                if (!sprites.ContainsKey(key)) sprites[key] = sp;
            }
        }
    }

    // "Ground Pork", "ground_pork" and "Ground-Pork" all become "groundpork"
    static string Key(string s)
    {
        var sb = new System.Text.StringBuilder();
        foreach (char c in s)
            if (char.IsLetterOrDigit(c)) sb.Append(char.ToLowerInvariant(c));
        return sb.ToString();
    }
}
#endif