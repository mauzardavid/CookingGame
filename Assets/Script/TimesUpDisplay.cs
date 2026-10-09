using UnityEngine;
using UnityEngine.SceneManagement;

public class TimesUpDisplay : MonoBehaviour
{
    [Tooltip("Only used if no gameplay scene is remembered (testing this scene alone)")]
    public string retrySceneName = "2";
    [Tooltip("Area-select scene")]
    public string menuSceneName = "1";

    // dishIndex is not changed on a fail, so the SAME dish restarts.
    public void OnRetryPressed()
    {
        string scene = string.IsNullOrEmpty(GameSession.areaSceneName) ? retrySceneName : GameSession.areaSceneName;
        SceneManager.LoadScene(scene);
    }

    public void OnBackToMenuPressed()
    {
        GameSession.StartNewShift();
        SceneManager.LoadScene(menuSceneName);
    }
}