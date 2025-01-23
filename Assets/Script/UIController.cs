using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private TextMeshProUGUI bulletText;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject gameWinCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    private bool isPaused = false;
    private bool isGameOver = false;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UpdateBulletUI(playerController.BulletCount);

        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        if (gameWinCanvas != null) gameWinCanvas.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && !isGameOver)
                ResumeGame();
            else
                PauseGame();
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
    }

    public void ResumeGame()
    {
        SoundManager.Instance.PlaySFX(SFXList.Button_Click);
        isPaused = false;
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1f;
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartLevel()
    {
        SoundManager.Instance.PlaySFX(SFXList.Button_Click);
        isGameOver = false;
        Time.timeScale = 1f;
        levelManager.ReloadCurrentLevel();
    }

    public void NextLevel()
    {
        SoundManager.Instance.PlaySFX(SFXList.Button_Click);
        isGameOver = false;
        Time.timeScale = 1f;
        levelManager.LoadNextLevel();
    }

    public void ReturnToLobby()
    {
        SoundManager.Instance.PlaySFX(SFXList.Button_Click);
        isGameOver = false;
        Time.timeScale = 1f;
        levelManager.LoadScene(SceneName.Lobby);
    }

    public void TriggerGamewin()
    {
        SoundManager.Instance.PlaySFX(SFXList.Game_Complete);
        Time.timeScale = 0f;
        gameWinCanvas.SetActive(true);
    }

    public void TriggerGameOver()
    {
        SoundManager.Instance.PlaySFX(SFXList.Game_Over);
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverCanvas.SetActive(true);
    }
}