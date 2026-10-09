#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

// Put this file in Assets/Editor.
// Menu: MixKitchen > Rename Sprite Sheets
//
// Slices AND names your sprite sheets with exact rectangles, so every sheet always ends up
// with the right number of slices (Main 50, Results 48, Finished 15, Extras 4).
// Sheets are found by file name inside Assets/Sprites:
//   "...main...ingred..."  -> 50 slices
//   "...result..."         -> 48 slices
//   "...finished..."       -> 15 finished dishes (files with "label" in the name are skipped)
//   "...extra..."          -> 2x2 = Heat, Muscovado Sugar, Margarine, Soy Sauce
// The names follow the *_labeled.png pictures. Luzon and Visayas names are certain;
// a few Mindanao result names are best guesses.
// If your PNG is a different size than expected, the tool falls back to renaming the
// existing slices in reading order.
public static class SpriteSheetRenamer
{
    const string SpriteFolder = "Assets/Sprites";

    // ---------- names ----------
    static readonly string[] MainNames =
    {
        "Ground Pork", "Garlic", "Vinegar", "Sugar", "Beef Shank",
        "Marrow Bones", "Water", "Onion", "Corn", "Cabbage",
        "Buko", "Milk", "Flour", "Butter", "Powdered Milk",
        "Tulingan", "Salt", "Kamias", "Chicken", "Calamansi",
        "Annatto", "Banana", "Sweet Potato", "Coconut Milk", "Sticky Rice",
        "Ginger", "Fish", "Rice", "Rice Flour", "Tuna",
        "Chili", "Beef", "Pepper", "Cooking Oil", "Fresh Fish",
        "Fresh Tuna", "Fish 2", "Garlic Onion Ginger", "Spare Bowl 1", "Spare Bowl 2",
        "Spare Bowl 3", "Spare Bowl 4", "Spare Bowl 5", "Spare Bowl 6", "Spare Bowl 7",
        "Spare Bowl 8", "Spare Bowl 9", "Spare Bowl 10", "Spare Bowl 11", "Spare Bowl 12"
    };

    static readonly string[] ResultNames =
    {
        "Garlic Pork", "Marinated Pork", "Beef Bone Mix", "Beef Broth", "Seasoned Broth",
        "Corn Beef Soup", "Sweet Buko", "Buko Filling", "Pie Dough", "Milk Mixture",
        "Sweet Milk", "Salted Tulingan", "Sour Tulingan", "Calamansi Chicken", "Garlic Chicken",
        "Inasal Marinade", "Mixed Root Fruits", "Creamy Rice", "Binignit Base", "Dough",
        "Soft Dough", "Sweet Dough", "Coconut Sticky Rice", "Sweet Ginger", "Vinegar Fish",
        "Garlic-Ginger Mix", "Paksiw Base", "Chicken Filling", "Coconut Rice Mix", "Salted Tuna",
        "Seasoned Tuna", "Onion-Ginger Mix", "Kinilaw Mix", "Spicy Kinilaw", "Vinegar Tuna",
        "Tuna Kinilaw Mix", "Spicy Tuna", "Marinated Meat", "Garlic Meat", "Seasoned Meat",
        "Peppered Meat", "Garlic-Onion Mix", "Sauteed Mix", "Seasoned Fish", "Coconut Fish",
        "Spare 1", "Spare 2", "Spare 3"
    };

    static readonly string[] FinishedNames =
    {
        "Longganisang Lucban", "Bulalo", "Buko Pie", "Pastillas", "Sinaing na Tulingan",
        "Chicken Inasal", "Binignit", "Piaya", "Puto Maya", "Paksiw na Isda",
        "Grilled Tuna", "Kinilaw na Isda", "Tuna Kinilaw", "Satti", "Ginataang Isda"
    };

    // top-left, top-right, bottom-left, bottom-right
    static readonly string[] ExtraNames = { "Heat", "Muscovado Sugar", "Margarine", "Soy Sauce" };

    // ---------- exact boxes: x, y, width, height measured from the TOP-LEFT of the PNG ----------
    static readonly int[] MainBoxes =
    {
        70,105,242,225, 307,120,214,209, 517,90,145,242, 668,147,212,185,
        875,115,232,221, 1106,137,213,201, 1319,103,189,229, 1508,119,216,205,
        1724,117,206,212, 74,337,234,210, 303,339,199,214, 501,337,169,214,
        671,351,230,199, 892,354,215,180, 1109,373,204,172, 1308,342,225,216,
        1525,375,202,169, 1728,361,201,189, 73,563,232,202, 305,578,191,192,
        497,577,201,182, 697,564,198,199, 899,584,211,179, 1112,560,206,205,
        1319,585,209,179, 1524,567,201,190, 1724,570,212,202, 74,783,220,183,
        297,789,198,175, 497,787,218,176, 682,773,224,191, 906,780,224,186,
        1138,791,183,163, 1345,772,139,203, 1502,786,238,180, 1732,782,211,181,
        75,981,221,161, 302,967,197,177, 496,972,207,174, 698,973,209,174,
        905,975,207,171, 1113,978,206,173, 1315,977,206,181, 1518,981,207,180,
        1723,978,214,183, 73,1136,235,176, 325,1127,232,187, 562,1141,234,171,
        798,1140,241,172, 1038,1145,232,168,
    };

    static readonly int[] ResultBoxes =
    {
        36,121,246,205, 279,116,246,209, 520,107,246,218, 763,116,236,208,
        997,117,234,207, 1225,110,244,212, 1464,97,239,231, 1697,129,237,204,
        38,333,231,201, 282,354,225,185, 526,321,203,225, 741,334,264,209,
        1004,340,240,200, 1246,334,235,206, 1479,339,231,202, 1711,343,234,193,
        32,546,264,211, 298,553,233,197, 537,551,245,201, 788,557,222,187,
        1013,558,218,184, 1243,557,228,186, 1481,556,228,194, 1720,546,224,197,
        35,760,244,189, 282,757,235,195, 521,754,249,196, 769,755,229,192,
        1002,753,234,191, 1243,751,239,198, 1486,758,236,193, 1724,752,223,193,
        37,953,240,177, 286,952,233,181, 526,951,234,182, 767,950,241,183,
        1015,951,236,182, 1256,952,233,182, 1492,953,230,181, 1725,954,224,183,
        31,1130,235,187, 269,1130,232,179, 499,1132,245,185, 743,1131,259,183,
        1005,1137,232,179, 1236,1132,236,184, 1475,1131,242,187, 1717,1133,233,186,
    };

    static readonly int[] FinishedBoxes =
    {
        18,48,261,188, 298,43,244,201, 561,48,255,186, 827,46,259,189,
        1094,36,267,213, 11,295,275,200, 304,298,234,193, 553,300,275,194,
        821,291,271,197, 1105,296,246,193, 14,551,270,188, 304,552,235,182,
        571,555,234,179, 820,531,275,209, 1106,551,244,185,
    };

    [MenuItem("MixKitchen/Rename Sprite Sheets")]
    public static void Run()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { SpriteFolder });
        int done = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string file = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();

            if (file.Contains("label")) continue;

            if (file.Contains("main") && file.Contains("ingred")) { if (SliceAndName(path, MainNames, MainBoxes, 2000, 1414)) done++; }
            else if (file.Contains("result")) { if (SliceAndName(path, ResultNames, ResultBoxes, 2000, 1414)) done++; }
            else if (file.Contains("finished")) { if (SliceAndName(path, FinishedNames, FinishedBoxes, 1376, 768)) done++; }
            else if (file.Contains("extra")) { if (HandleExtras(path)) done++; }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Sprite sheets processed: " + done + ". Now run MixKitchen > Build Luzon Area.");
    }

    // ---------- slicing with exact boxes ----------
    static bool SliceAndName(string path, string[] names, int[] boxes, int expectedW, int expectedH)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return false;

        int count = boxes.Length / 4;
        if (count != names.Length)
        {
            Debug.LogError(path + ": internal mismatch, " + count + " boxes but " + names.Length + " names.");
            return false;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.SaveAndReimport();

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex == null || tex.width != expectedW || tex.height != expectedH)
        {
            Debug.LogWarning(path + ": expected a " + expectedW + "x" + expectedH + " image but found " +
                             (tex == null ? "nothing" : tex.width + "x" + tex.height) +
                             ". Falling back to renaming the existing slices in reading order.");
            return RenameExisting(path, names);
        }

        ISpriteEditorDataProvider dp = GetProvider(importer);
        var rects = new List<SpriteRect>();
        for (int i = 0; i < count; i++)
        {
            int x = boxes[i * 4];
            int yTop = boxes[i * 4 + 1];
            int w = boxes[i * 4 + 2];
            int h = boxes[i * 4 + 3];
            rects.Add(new SpriteRect
            {
                name = names[i],
                rect = new Rect(x, expectedH - (yTop + h), w, h),   // Unity measures y from the bottom
                alignment = SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
                spriteID = GUID.Generate()
            });
        }

        dp.SetSpriteRects(rects.ToArray());
        dp.Apply();
        importer.SaveAndReimport();
        Debug.Log("Sliced and named " + count + " sprites in " + path);
        return true;
    }

    // ---------- fallback: rename what is already sliced ----------
    static bool RenameExisting(string path, string[] names)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return false;

        ISpriteEditorDataProvider dp = GetProvider(importer);
        var rects = new List<SpriteRect>(dp.GetSpriteRects());

        if (rects.Count != names.Length)
        {
            Debug.LogWarning(path + ": found " + rects.Count + " slices but expected " + names.Length +
                             ". Names are applied in reading order, so check the result against the *_labeled.png picture.");
        }

        rects.Sort(CompareReadingOrder);
        int count = Mathf.Min(rects.Count, names.Length);
        for (int i = 0; i < count; i++)
            rects[i].name = names[i];

        dp.SetSpriteRects(rects.ToArray());
        dp.Apply();
        importer.SaveAndReimport();
        Debug.Log("Renamed " + count + " slices in " + path);
        return true;
    }

    static bool HandleExtras(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return false;

        if (importer.spriteImportMode == SpriteImportMode.Multiple)
        {
            var existing = GetProvider(importer).GetSpriteRects();
            if (existing.Length == 4) return RenameExisting(path, ExtraNames);
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.SaveAndReimport();

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex == null) return false;
        float w = tex.width;
        float h = tex.height;

        ISpriteEditorDataProvider dp = GetProvider(importer);
        var rects = new List<SpriteRect>();
        for (int i = 0; i < 4; i++)
        {
            int col = i % 2;
            int row = i / 2;   // 0 = top row
            rects.Add(new SpriteRect
            {
                name = ExtraNames[i],
                rect = new Rect(col * w / 2f, (1 - row) * h / 2f, w / 2f, h / 2f),
                alignment = SpriteAlignment.Center,
                pivot = new Vector2(0.5f, 0.5f),
                spriteID = GUID.Generate()
            });
        }

        dp.SetSpriteRects(rects.ToArray());
        dp.Apply();
        importer.SaveAndReimport();
        Debug.Log("Sliced " + path + " into Heat / Muscovado Sugar / Margarine / Soy Sauce");
        return true;
    }

    static ISpriteEditorDataProvider GetProvider(TextureImporter importer)
    {
        var factory = new SpriteDataProviderFactories();
        factory.Init();
        ISpriteEditorDataProvider dp = factory.GetSpriteEditorDataProviderFromObject(importer);
        dp.InitSpriteEditorDataProvider();
        return dp;
    }

    // Top row first, then left to right. (Sprite rect y starts at the bottom of the image.)
    static int CompareReadingOrder(SpriteRect a, SpriteRect b)
    {
        float tolerance = Mathf.Min(a.rect.height, b.rect.height) * 0.5f;
        float dy = b.rect.center.y - a.rect.center.y;
        if (Mathf.Abs(dy) > tolerance) return dy > 0 ? 1 : -1;
        return a.rect.center.x.CompareTo(b.rect.center.x);
    }
}
#endif