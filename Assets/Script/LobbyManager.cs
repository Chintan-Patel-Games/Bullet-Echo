using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void OnPlayButtonClick()
    {
        // Load the first level (replace "Level1" with your actual level name or index)
        SceneManager.LoadScene((int)SceneName.Level1);
    }

    public void OnQuitButtonClick()
    {
        #if UNITY_WEBGL
        // Show a native browser alert
        Application.ExternalEval("alert('Thank you for playing! Please close the browser tab to exit.');");
        #else
        Application.Quit();
        #endif
    }
}