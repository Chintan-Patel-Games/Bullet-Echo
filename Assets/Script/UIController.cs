using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject defaultSelectedButton; // The button to select when the menu opens
    [SerializeField] private Selectable[] pauseMenuButtons; // Array of buttons for navigation in pause menu

    private bool isPaused = false;
    private int currentButtonIndex = 0;

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

        // Select the default button
        if (defaultSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            UpdateButtonIndex(defaultSelectedButton);
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

        // Handle keyboard navigation in pause menu
        if (isPaused)
        {
            HandlePauseMenuNavigation();
        }

        // Continuously update the UI if needed
        UpdateHealthUI(playerController.CurrentHealth);
        UpdateBulletUI(playerController.BulletCount);
    }

    private bool IsLobbyScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        bool isLobby = currentSceneName == "Lobby"; // Replace "Lobby" with your lobby scene name

        if (isLobby)
        {
            // Make the cursor visible and unlock it
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        return isLobby;
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

        // Select the first button in the pause menu
        if (pauseMenuButtons.Length > 0)
        {
            SelectButton(pauseMenuButtons[0]);
        }
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

    private void HandlePauseMenuNavigation()
    {
        if (pauseMenuButtons == null || pauseMenuButtons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectPreviousButton();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectNextButton();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                var button = selected.GetComponent<Button>();
                button?.onClick.Invoke();
            }
        }
    }

    private void SelectPreviousButton()
    {
        currentButtonIndex = (currentButtonIndex - 1 + pauseMenuButtons.Length) % pauseMenuButtons.Length;
        SelectButton(pauseMenuButtons[currentButtonIndex]);
    }

    private void SelectNextButton()
    {
        currentButtonIndex = (currentButtonIndex + 1) % pauseMenuButtons.Length;
        SelectButton(pauseMenuButtons[currentButtonIndex]);
    }

    private void SelectButton(Selectable button)
    {
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }

    private void UpdateButtonIndex(GameObject selectedButton)
    {
        for (int i = 0; i < pauseMenuButtons.Length; i++)
        {
            if (pauseMenuButtons[i].gameObject == selectedButton)
            {
                currentButtonIndex = i;
                break;
            }
        }
    }
}