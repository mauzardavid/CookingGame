#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

// Put this file in Assets/Editor.
// Open the Order Complete scene (3), then click MixKitchen > Build Order Complete Screen.
// Builds the new layout on the Canvas using Order_Complete_bg.png:
//   left  = picture of the finished dish
//   right = dish name, place of origin, "Did you know?" trivia
//   bottom = banner title, message, NEXT ORDER and BACK TO MENU buttons
// The old objects under the Canvas are switched OFF (not deleted). Running the tool again
// replaces the panel it made before.
public static class OrderCompleteScreenBuilder
{
    // size of Order_Complete_bg.png in pixels; every position below is measured on it
    const float W = 2000f;
    const float H = 1125f;

    static readonly Color Dark = new Color(0.25f, 0.15f, 0.08f, 1f);
    static readonly Color Heading = new Color(0.48f, 0.22f, 0.06f, 1f);
    static readonly Color Cream = new Color(0.99f, 0.95f, 0.86f, 1f);
    static readonly Color Red = new Color(0.55f, 0.10f, 0.05f, 1f);

    [MenuItem("MixKitchen/Build Order Complete Screen")]
    public static void Build()
    {
        Sprite bg = LoadBackground();
        if (bg == null) return;

        Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
        if (canvas == null)
        {
            Debug.LogError("No Canvas in this scene. Open the Order Complete scene (3) first.");
            return;
        }

        float canvasH = ((RectTransform)canvas.transform).rect.height;
        if (canvasH < 100f) canvasH = 1080f;

        // remove the panel made by an earlier run
        Transform oldPanel = canvas.transform.Find("OrderCompletePanel");
        if (oldPanel != null) Object.DestroyImmediate(oldPanel.gameObject);

        // switch off the old layout
        foreach (Transform child in canvas.transform) child.gameObject.SetActive(false);

        // ---------- background panel ----------
        GameObject panelGO = new GameObject("OrderCompletePanel", typeof(RectTransform), typeof(Image));
        panelGO.transform.SetParent(canvas.transform, false);
        RectTransform panel = (RectTransform)panelGO.transform;
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;
        Image bgImage = panelGO.GetComponent<Image>();
        bgImage.sprite = bg;
        bgImage.raycastTarget = false;

        // ---------- left: dish picture (inside the oval) ----------
        RectTransform dishRT = MakeRect("DishImage", panel, 400, 295, 720, 615);
        Image dishImage = dishRT.gameObject.AddComponent<Image>();
        dishImage.preserveAspect = true;
        dishImage.raycastTarget = false;
        dishImage.enabled = false;   // the script turns it on when there is a picture

        // ---------- right: name, origin, trivia (kept left of the book in the corner) ----------
        TMP_Text dishName = AddText(MakeRect("DishNameText", panel, 1070, 213, 1640, 268),
            "Longganisang Lucban", canvasH * 0.045f, Heading, FontStyles.Bold, TextAlignmentOptions.Left);

        AddText(MakeRect("OriginHeading", panel, 1070, 285, 1640, 325),
            "PLACE OF ORIGIN", canvasH * 0.028f, Heading, FontStyles.Bold, TextAlignmentOptions.Left);

        TMP_Text origin = AddText(MakeRect("OriginText", panel, 1070, 330, 1640, 420),
            "Quezon - CALABARZON (Region IV-A)", canvasH * 0.032f, Dark, FontStyles.Normal, TextAlignmentOptions.TopLeft);

        AddText(MakeRect("TriviaHeading", panel, 1070, 435, 1640, 475),
            "DID YOU KNOW?", canvasH * 0.028f, Heading, FontStyles.Bold, TextAlignmentOptions.Left);

        TMP_Text trivia = AddText(MakeRect("TriviaText", panel, 1070, 480, 1640, 685),
            "Sample trivia text. The real fun fact of the finished dish appears here.", canvasH * 0.030f, Dark,
            FontStyles.Normal, TextAlignmentOptions.TopLeft);

        // ---------- bottom: banner title, message, buttons ----------
        TMP_Text title = AddText(MakeRect("TitleText", panel, 770, 727, 1235, 790),
            "Order Complete!", canvasH * 0.040f, Heading, FontStyles.Bold, TextAlignmentOptions.Center);

        TMP_Text message = AddText(MakeRect("MessageText", panel, 200, 815, 1800, 880),
            "You plated Longganisang Lucban with 60s to spare.", canvasH * 0.033f, Dark, FontStyles.Normal,
            TextAlignmentOptions.Center);

        Button nextButton = MakeButton("NextOrderButton", panel, 530, 888, 910, 970, "NEXT ORDER", Red, Cream, canvasH);
        Button backButton = MakeButton("BackToMenuButton", panel, 1090, 888, 1470, 970, "BACK TO MENU", Cream, Dark, canvasH);

        // ---------- the script ----------
        OrderCompleteDisplay display = Object.FindFirstObjectByType<OrderCompleteDisplay>(FindObjectsInactive.Include);
        if (display == null)
        {
            GameObject go = new GameObject("OrderCompleteDisplay");
            display = go.AddComponent<OrderCompleteDisplay>();
        }

        Undo.RecordObject(display, "Build Order Complete Screen");
        display.dishImage = dishImage;
        display.dishNameText = dishName;
        display.originText = origin;
        display.triviaText = trivia;
        display.titleText = title;
        display.messageText = message;
        display.nextOrderButton = nextButton.gameObject;
        if (string.IsNullOrEmpty(display.nextOrderSceneName) || display.nextOrderSceneName == "2")
            display.nextOrderSceneName = "Luzon";
        EditorUtility.SetDirty(display);

        UnityEventTools.AddPersistentListener(nextButton.onClick, new UnityAction(display.OnNextOrderPressed));
        UnityEventTools.AddPersistentListener(backButton.onClick, new UnityAction(display.OnBackToMenuPressed));

        EditorSceneManager.MarkSceneDirty(display.gameObject.scene);
        Selection.activeGameObject = panelGO;

        Debug.Log("Order Complete screen built. Now check 'Menu Scene Name' on the OrderCompleteDisplay object " +
                  "(it must be the exact name of your difficulty / area-select scene), then press Ctrl+S.");
    }

    // ---------- helpers ----------
    static Sprite LoadBackground()
    {
        string[] guids = AssetDatabase.FindAssets("Order_Complete_bg t:Texture2D");
        if (guids.Length == 0)
        {
            Debug.LogError("Order_Complete_bg.png was not found. Put it in Assets/Sprites first.");
            return null;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // Box measured in pixels on the 2000x1125 picture, from the top-left corner.
    static RectTransform MakeRect(string name, Transform parent, float x0, float y0, float x1, float y1)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = new Vector2(x0 / W, 1f - y1 / H);
        rt.anchorMax = new Vector2(x1 / W, 1f - y0 / H);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return rt;
    }

    static TMP_Text AddText(RectTransform rt, string text, float maxSize, Color color, FontStyles style,
                            TextAlignmentOptions align)
    {
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.color = color;
        t.fontStyle = style;
        t.alignment = align;
        t.raycastTarget = false;
        t.fontSize = maxSize;
        t.enableAutoSizing = true;      // shrinks to fit the box
        t.fontSizeMax = maxSize;
        t.fontSizeMin = maxSize * 0.4f;
        return t;
    }

    static Button MakeButton(string name, Transform parent, float x0, float y0, float x1, float y1,
                             string label, Color fill, Color textColor, float canvasH)
    {
        RectTransform rt = MakeRect(name, parent, x0, y0, x1, y1);

        Image image = rt.gameObject.AddComponent<Image>();
        image.color = fill;

        Outline outline = rt.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0.35f, 0.20f, 0.08f, 1f);
        outline.effectDistance = new Vector2(3f, -3f);

        Button button = rt.gameObject.AddComponent<Button>();
        button.targetGraphic = image;

        GameObject labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(rt, false);
        RectTransform lr = (RectTransform)labelGO.transform;
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.offsetMin = new Vector2(8f, 4f);
        lr.offsetMax = new Vector2(-8f, -4f);
        AddText(lr, label, canvasH * 0.032f, textColor, FontStyles.Bold, TextAlignmentOptions.Center);

        return button;
    }
}
#endif