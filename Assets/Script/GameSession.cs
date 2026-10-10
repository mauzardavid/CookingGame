// Static data that survives scene loads.
public static class GameSession
{
    // Filled when a dish is completed (shown on the Order Complete scene)
    public static float remainingTime;
    public static string completedOrderName;

    // Shown on the Order Complete screen
    public static UnityEngine.Sprite completedSprite;
    public static string completedOrigin = "";
    public static string completedTrivia = "";

    // The gameplay scene being played. CraftingManager fills this in by itself.
    public static string areaSceneName = "";

    // Optional: the area chosen on the area-select scene (single gameplay scene setup).
    public static AreaData currentArea = null;

    // 0 = dish 1 ... 4 = dish 5. A failed dish does NOT change this, so Retry repeats the same dish.
    public static int dishIndex = 0;
    public static int totalDishes = 5;

    public static bool IsLastDish
    {
        get { return dishIndex >= totalDishes - 1; }
    }

    public static void StartNewShift()
    {
        dishIndex = 0;
    }
}