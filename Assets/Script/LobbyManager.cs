using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelectedButton; // The button to select when the menu opens
    [SerializeField] private Selectable[] buttons; // Array of buttons for navigation

    private int currentButtonIndex = 0;

    private void Start()
    {
        // Lock and hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Select the default button
        if (defaultSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
            UpdateButtonIndex(defaultSelectedButton);
        }
    }

    private void Update()
    {
        // Navigate up and down manually
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectPreviousButton();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectNextButton();
        }

        // Activate the currently selected button
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
        if (buttons == null || buttons.Length == 0) return;

        currentButtonIndex = (currentButtonIndex - 1 + buttons.Length) % buttons.Length;
        SelectButton(buttons[currentButtonIndex]);
    }

    private void SelectNextButton()
    {
        if (buttons == null || buttons.Length == 0) return;

        currentButtonIndex = (currentButtonIndex + 1) % buttons.Length;
        SelectButton(buttons[currentButtonIndex]);
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
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject == selectedButton)
            {
                currentButtonIndex = i;
                break;
            }
        }
    }

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