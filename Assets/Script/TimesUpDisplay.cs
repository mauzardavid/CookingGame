using UnityEngine;
using UnityEngine.SceneManagement;

public class TimesUpDisplay : MonoBehaviour
{
    [Tooltip("Scene name/number to load to retry (your gameplay scene)")]
    public string retrySceneName = "2";

    [Tooltip("Scene name/number to load for the main menu")]
    public string menuSceneName = "1";

    // Hook to the RETRY button's OnClick()
    public void OnRetryPressed()
    {
        SceneManager.LoadScene(retrySceneName);
    }

    // Hook to the BACK TO MENU button's OnClick()
    public void OnBackToMenuPressed()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
