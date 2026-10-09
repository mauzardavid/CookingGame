#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Put this file in Assets/Editor.
// Menu: MixKitchen > Build Luzon Area
// Creates Assets/Data/Luzon.asset with all 5 Luzon dishes (choices, steps, hints)
// and fills in the sprites by matching sprite NAMES. Sprites that are not found stay
// empty - assign those by hand afterwards. Running it again rebuilds the asset
// (it overwrites manual edits made to Luzon.asset).
public static class LuzonAreaBuilder
{
    // Where to look for your sliced sprites. Use "Assets/Sprites" to make it faster.
    const string SpriteFolder = "Assets";
    const string OutputFolder = "Assets/Data";
    const string OutputPath = "Assets/Data/Luzon.asset";
    const float TimeLimit = 300f;

    static Dictionary<string, Sprite> sprites;
    static readonly List<string> missing = new List<string>();

    [MenuItem("MixKitchen/Build Luzon Area")]
    public static void Build()
    {
        LoadSprites();
        missing.Clear();

        var dishes = new List<Dish>();

        // 1. LONGGANISANG LUCBAN
        dishes.Add(new Dish
        {
            dishName = "Longganisang Lucban",
            origin = "Quezon - CALABARZON (Region IV-A)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Ground Pork"), Ing("Garlic"), Ing("Vinegar"), Ing("Sugar"), Ing("Salt"), Ing("Onion")
            },
            steps = new List<RecipeStep>
            {
                Step("Ground Pork", "Garlic", "Garlic Pork", "Start with the meat and the garlic."),
                Step("Garlic Pork", "Vinegar", "Marinated Pork", "Marinate the garlicky pork in something sour."),
                Step("Marinated Pork", "Sugar", "Longganisang Lucban", "Lucban longganisa is sweet. Add the final seasoning.")
            }
        });

        // 2. BULALO
        dishes.Add(new Dish
        {
            dishName = "Bulalo",
            origin = "Batangas - CALABARZON (Region IV-A)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Beef Shank"), Ing("Marrow Bones"), Ing("Water"), Ing("Onion"), Ing("Corn"), Ing("Cabbage")
            },
            steps = new List<RecipeStep>
            {
                Step("Beef Shank", "Marrow Bones", "Beef Bone Mix", "Begin with the two beef parts."),
                Step("Beef Bone Mix", "Water", "Beef Broth", "A soup needs liquid."),
                Step("Beef Broth", "Onion", "Seasoned Broth", "Season the broth with a vegetable."),
                Step("Seasoned Broth", "Corn", "Corn Beef Soup", "Add the sweet yellow vegetable."),
                Step("Corn Beef Soup", "Cabbage", "Bulalo", "Finish with the leafy green.")
            }
        });

        // 3. BUKO PIE
        dishes.Add(new Dish
        {
            dishName = "Buko Pie",
            origin = "Laguna - CALABARZON (Region IV-A)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Buko"), Ing("Sugar"), Ing("Milk"), Ing("Flour", "Flour / Dough", "Dough"), Ing("Butter"), Ing("Salt")
            },
            steps = new List<RecipeStep>
            {
                Step("Buko", "Sugar", "Sweet Buko", "Sweeten the coconut."),
                Step("Sweet Buko", "Milk", "Buko Filling", "Make the filling creamy."),
                Step("Flour", "Butter", "Pie Dough", "The crust is made separately from flour and fat."),
                Step("Buko Filling", "Pie Dough", "Buko Pie", "Put the filling and the crust together.")
            }
        });

        // 4. PASTILLAS
        dishes.Add(new Dish
        {
            dishName = "Pastillas",
            origin = "Bulacan - Central Luzon (Region III)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Milk"), Ing("Powdered Milk"), Ing("Sugar"), Ing("Heat"), Ing("Butter"), Ing("Flour", "Flour / Dough", "Dough")
            },
            steps = new List<RecipeStep>
            {
                Step("Milk", "Powdered Milk", "Milk Mixture", "Two kinds of milk go first."),
                Step("Milk Mixture", "Sugar", "Sweet Milk", "Make it sweet."),
                Step("Sweet Milk", "Heat", "Pastillas", "Cook it down until it can be rolled.")
            }
        });

        // 5. SINAING NA TULINGAN
        dishes.Add(new Dish
        {
            dishName = "Sinaing na Tulingan",
            origin = "Batangas - CALABARZON (Region IV-A)",
            timeLimit = TimeLimit,
            startingChoices = new List<IngredientDef>
            {
                Ing("Tulingan"), Ing("Salt"), Ing("Kamias"), Ing("Water"), Ing("Vinegar"), Ing("Garlic")
            },
            steps = new List<RecipeStep>
            {
                Step("Tulingan", "Salt", "Salted Tulingan", "Season the fish first."),
                Step("Salted Tulingan", "Kamias", "Sour Tulingan", "The sour fruit softens the fish."),
                Step("Sour Tulingan", "Water", "Sinaing na Tulingan", "Slow-cook it with water.")
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

        area.areaName = "Luzon";
        area.dishes = dishes;
        EditorUtility.SetDirty(area);
        AssetDatabase.SaveAssets();
        Selection.activeObject = area;
        EditorGUIUtility.PingObject(area);

        if (missing.Count > 0)
        {
            Debug.LogWarning("Luzon built, but these sprites were not found (assign them by hand or rename the sprites): "
                + string.Join(", ", new HashSet<string>(missing)));
        }
        else
        {
            Debug.Log("Luzon built with all sprites found.");
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