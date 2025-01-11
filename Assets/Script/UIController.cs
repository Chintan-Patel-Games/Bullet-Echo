using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private GameObject pauseMenuCanvas;

    private bool isPaused = false;

    private void Start()
    {
        // Check if this is a lobby scene and destroy the UIController if so
        if (IsLobbyScene())
        {
            Destroy(gameObject);
            return;
        }

        // Hide and lock the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Update the UI initially
        UpdateHealthUI(playerController.CurrentHealth);
        UpdateBulletUI(playerController.BulletCount);

        // Ensure the pause menu is hidden at the start
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        // Toggle Pause Menu with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // Continuously update the UI if needed
        UpdateHealthUI(playerController.CurrentHealth);
        UpdateBulletUI(playerController.BulletCount);
    }

    private bool IsLobbyScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        return currentSceneIndex == (int)SceneName.Lobby;
    }

    public void UpdateHealthUI(int currentHealth)
    {
        healthText.text = "Health " + currentHealth;
    }

    public void UpdateBulletUI(int bulletCount)
    {
        bulletText.text = "Bullets " + bulletCount;
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuCanvas.SetActive(true); // Show the pause menu
        Time.timeScale = 0f; // Freeze game time
        Cursor.visible = true; // Show the cursor
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false); // Hide the pause menu
        Time.timeScale = 1f; // Resume game time
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Reset game time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload current scene
    }

    public void ReturnToLobby()
    {
        Time.timeScale = 1f; // Reset game time
        SceneManager.LoadScene((int)SceneName.Lobby); // Load the Lobby scene
    }
}