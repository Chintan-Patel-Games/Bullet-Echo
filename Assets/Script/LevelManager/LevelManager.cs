using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject enemiesParent; // Array of enemies in the level

    public void CompleteLevel()
    {
        if (AreAllEnemiesDead())
        {
            LoadNextLevel(); // Load the next level
        }
        else
        {
            Debug.Log("There are still enemies to defeat!");
        }
    }

    private bool AreAllEnemiesDead()
    {
        // Get all children of the "Enemies" parent GameObject
        Transform[] enemyTransforms = enemiesParent.GetComponentsInChildren<Transform>();

        foreach (Transform enemyTransform in enemyTransforms)
        {
            // Ignore the parent GameObject itself
            if (enemyTransform != enemiesParent)
            {
                EnemyAI enemy = enemyTransform.GetComponent<EnemyAI>();
                if (enemy != null) // Check if the enemy is alive
                {
                    return false; // If at least one enemy is alive, return false
                }
            }
        }

        return true; // If all enemies are dead, return true
    }

    // Method to load a scene by SceneName enum
    public void LoadScene(SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneName.Lobby:
                SceneManager.LoadScene("Lobby");  // Replace with your actual scene name
                break;

            case SceneName.Level1:
                SceneManager.LoadScene("Level1");
                break;

            case SceneName.Level2:
                SceneManager.LoadScene("Level2");
                break;

            default:
                Debug.LogError("Scene not found: " + sceneName);
                break;
        }
    }

    // Optionally, add a method to load the next scene based on the current one
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
                Debug.LogError("Unknown scene: " + currentScene.name);
                break;
        }
    }

    // Optionally, you can add methods to handle loading the previous level or restart
    public void ReloadCurrentLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);  // Reloads the current scene
    }
}