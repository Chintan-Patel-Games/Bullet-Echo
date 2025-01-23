using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private void Start()
    {
        // Lock and hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SoundManager.Instance.PlayBGM(BGMList.Menu_BGM);
    }

    public void OnPlayButtonClick()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene((int)SceneName.Level1);
        SoundManager.Instance.PlaySFX(SFXList.Button_Click);
        SoundManager.Instance.PlayBGM(BGMList.Game_BGM);
    }

    public void OnQuitButtonClick()
    {
        SoundManager.Instance.PlaySFX(SFXList.Button_Click);
        #if UNITY_WEBGL
        // Show a native browser alert
        Application.ExternalEval("alert('Thank you for playing! Please close the browser tab to exit.');");
        #else
        Application.Quit();
        #endif
    }
}