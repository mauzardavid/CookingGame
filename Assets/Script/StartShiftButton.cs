using UnityEngine;
using UnityEngine.SceneManagement;

// Main menu: hook LoadScene to the START SHIFT button and type the area-select scene name.
public class StartShiftButton : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}