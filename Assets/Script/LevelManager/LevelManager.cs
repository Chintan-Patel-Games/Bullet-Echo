using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; } // Singleton instance

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set the instance
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    // Method to load a scene by SceneName enum
    public void LoadScene(SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneName.Lobby:
                SceneManager.LoadScene(SceneName.Lobby.ToString());  // Replace with your actual scene name
                break;

            case SceneName.Level1:
                SceneManager.LoadScene(SceneName.Level1.ToString());
                break;

            case SceneName.Level2:
                SceneManager.LoadScene(SceneName.Level2.ToString());
                break;

            default:
                Debug.Log("Scene not found: " + sceneName);
                break;
        }
    }

    public void LoadNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        switch (currentScene.name)
        {
            case "Lobby":
                LoadScene(SceneName.Level1);
                break;

            case "Level1":
                LoadScene(SceneName.Level2);
                break;

            case "Level2":
                LoadScene(SceneName.Lobby);
                break;

            default:
                Debug.Log("Unknown scene: " + currentScene.name);
                break;
        }
    }

    public void ReloadCurrentLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);  // Reloads the current scene
    }
}