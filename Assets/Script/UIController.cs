using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameWinCanvas;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private GameObject defaultSelectedButton;
    [SerializeField] private Selectable[] pauseMenuButtons;
    [SerializeField] private Selectable[] gameWinMenuButtons;
    [SerializeField] private Selectable[] gameOverMenuButtons;

    private bool isPaused = false;
    private bool isGameOver = false;
    private int currentButtonIndex = 0;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UpdateBulletUI(playerController.BulletCount);

        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        if (gameWinCanvas != null) gameWinCanvas.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);

        if (defaultSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            UpdateButtonIndex(defaultSelectedButton);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (isPaused)
            HandlePauseMenuNavigation();

        // Handle navigation for Game Over menu
        if (isGameOver)
        {
            HandleGameOverMenuNavigation();
            return; // Skip other updates if Game Over is active
        }

        UpdateBulletUI(playerController.BulletCount);
    }

    public void UpdateBulletUI(int bulletCount)
    {
        bulletText.text = "Bullets : " + bulletCount;
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;

        if (pauseMenuButtons.Length > 0)
            SelectButton(pauseMenuButtons[0]);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartLevel()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        levelManager.ReloadCurrentLevel();
    }

    public void NextLevel()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        levelManager.LoadNextLevel();
    }

    public void ReturnToLobby()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        levelManager.LoadScene(SceneName.Lobby);
    }

    public void TriggerGamewin()
    {
        if (levelManager.AreAllEnemiesDead())
        {
            Time.timeScale = 0f;
            gameWinCanvas.SetActive(true);

            if (gameWinMenuButtons.Length > 0)
                SelectButton(gameWinMenuButtons[0]);
        }
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);

        if (gameOverMenuButtons.Length > 0)
            SelectButton(gameOverMenuButtons[0]);
    }

    private void HandlePauseMenuNavigation()
    {
        HandleMenuNavigation(pauseMenuButtons);
    }

    private void HandleGameOverMenuNavigation()
    {
        HandleMenuNavigation(gameOverMenuButtons);
    }

    private void HandleMenuNavigation(Selectable[] menuButtons)
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            SelectPreviousButton(menuButtons);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            SelectNextButton(menuButtons);

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

    private void SelectPreviousButton(Selectable[] menuButtons)
    {
        currentButtonIndex = (currentButtonIndex - 1 + menuButtons.Length) % menuButtons.Length;
        SelectButton(menuButtons[currentButtonIndex]);
    }

    private void SelectNextButton(Selectable[] menuButtons)
    {
        currentButtonIndex = (currentButtonIndex + 1) % menuButtons.Length;
        SelectButton(menuButtons[currentButtonIndex]);
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