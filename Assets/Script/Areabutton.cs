using UnityEngine;
using UnityEngine.SceneManagement;

// Put on each Luzon / Visayas / Mindanao button of the area-select scene.
// Hook Play() to the button's OnClick().
//  - Three-scene setup: type the scene name (Luzon / Visayas / Mindanao), leave Area Data empty.
//  - One-scene setup:   type the gameplay scene name, drag the area's AreaData asset in.
public class AreaButton : MonoBehaviour
{
    public string gameplaySceneName = "Luzon";
    public AreaData areaData;

    public void Play()
    {
        GameSession.StartNewShift();
        GameSession.currentArea = areaData;   // null = use the dishes typed in the scene
        SceneManager.LoadScene(gameplaySceneName);
    }
}