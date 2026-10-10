#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Put this file in Assets/Editor.
// Menu: MixKitchen > Build Visayas Area
// Creates Assets/Data/Visayas.asset with all 5 Visayas dishes (choices, steps, hints)
// and fills in the sprites by matching sprite NAMES. Sprites that are not found stay
// empty - assign those by hand afterwards. Running it again rebuilds the asset
// (it overwrites manual edits made to Visayas.asset).
public static class VisayasAreaBuilder
{
    // Where to look for your sliced sprites. Use "Assets/Sprites" to make it faster.
    const string SpriteFolder = "Assets";
    const string OutputFolder = "Assets/Data";
    const string OutputPath = "Assets/Data/Visayas.asset";
    const float TimeLimit = 240f;   // Visayas = Medium

    static Dictionary<string, Sprite> sprites;
    static readonly List<string> missing = new List<string>();

    [MenuItem("MixKitchen/Build Visayas Area")]
    public static void Build()
    {
        LoadSprites();
        missing.Clear();

        var dishes = new List<Dish>();

        // 1. CHICKEN INASAL
        dishes.Add(new Dish
        {
            dishName = "Chicken Inasal",
            origin = "Bacolod, Negros Occidental - Western Visayas (Region VI)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Chicken"), Ing("Calamansi"), Ing("Garlic"), Ing("Vinegar"), Ing("Annatto"), Ing("Ginger")
            },
            steps = new List<RecipeStep>
            {
                Step("Chicken", "Calamansi", "Calamansi Chicken", "Start with the chicken and the sour citrus."),
                Step("Calamansi Chicken", "Garlic", "Garlic Chicken", "Add the aromatic bulb."),
                Step("Garlic Chicken", "Vinegar", "Inasal Marinade", "Marinate it in something sour."),
                Step("Inasal Marinade", "Annatto", "Chicken Inasal", "Annatto gives inasal its orange-yellow color.")
            }
        });

        // 2. BINIGNIT
        dishes.Add(new Dish
        {
            dishName = "Binignit",
            origin = "Cebu - Central Visayas (Region VII)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Banana"), Ing("Sweet Potato"), Ing("Coconut Milk"), Ing("Sticky Rice", "Glutinous Rice"), Ing("Sugar"), Ing("Muscovado Sugar")
            },
            steps = new List<RecipeStep>
            {
                Step("Banana", "Sweet Potato", "Mixed Root Fruits", "Combine the fruit and the root crop."),
                Step("Coconut Milk", "Sticky Rice", "Creamy Rice", "The rice is cooked in coconut milk."),
                Step("Mixed Root Fruits", "Creamy Rice", "Binignit Base", "Bring the two mixtures together."),
                Step("Binignit Base", "Sugar", "Binignit", "Sweeten it to finish.")
            }
        });

        // 3. PIAYA
        dishes.Add(new Dish
        {
            dishName = "Piaya",
            origin = "Negros Occidental - Western Visayas (Region VI)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Flour"), Ing("Muscovado Sugar"), Ing("Water"), Ing("Margarine"), Ing("Sugar"), Ing("Salt")
            },
            steps = new List<RecipeStep>
            {
                Step("Flour", "Water", "Dough", "Begin the dough with flour and water."),
                Step("Dough", "Margarine", "Soft Dough", "Fat makes the dough soft."),
                Step("Soft Dough", "Muscovado Sugar", "Sweet Dough", "Piaya is filled with dark brown sugar."),
                Step("Sweet Dough", "Flour", "Piaya", "You need flour once more to shape it.")
            }
        });

        // 4. PUTO MAYA
        dishes.Add(new Dish
        {
            dishName = "Puto Maya",
            origin = "Cebu - Central Visayas (Region VII)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Sticky Rice", "Glutinous Rice"), Ing("Coconut Milk"), Ing("Ginger"), Ing("Sugar"), Ing("Banana"), Ing("Salt")
            },
            steps = new List<RecipeStep>
            {
                Step("Sticky Rice", "Coconut Milk", "Coconut Sticky Rice", "Cook the rice in coconut milk."),
                Step("Ginger", "Sugar", "Sweet Ginger", "Make the sweet ginger on the side."),
                Step("Coconut Sticky Rice", "Sweet Ginger", "Puto Maya", "Serve the rice with the sweet ginger.")
            }
        });

        // 5. PAKSIW NA ISDA
        dishes.Add(new Dish
        {
            dishName = "Paksiw na Isda",
            origin = "Cebu - Central Visayas (Region VII)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Fish"), Ing("Vinegar"), Ing("Garlic"), Ing("Ginger"), Ing("Salt"), Ing("Onion")
            },
            steps = new List<RecipeStep>
            {
                Step("Fish", "Vinegar", "Vinegar Fish", "Paksiw means cooked in vinegar."),
                Step("Garlic", "Ginger", "Garlic-Ginger Mix", "Make the aromatics separately."),
                Step("Vinegar Fish", "Garlic-Ginger Mix", "Paksiw Base", "Join the fish and the aromatics."),
                Step("Paksiw Base", "Salt", "Paksiw na Isda", "Season it to finish.")
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

        area.areaName = "Visayas";
        area.dishes = dishes;
        DishTriviaFiller.Apply(area);
        EditorUtility.SetDirty(area);
        AssetDatabase.SaveAssets();
        Selection.activeObject = area;
        EditorGUIUtility.PingObject(area);

        if (missing.Count > 0)
        {
            Debug.LogWarning("Visayas built, but these sprites were not found (assign them by hand or rename the sprites): "
                + string.Join(", ", new HashSet<string>(missing)));
        }
        else
        {
            Debug.Log("Visayas built with all sprites found.");
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