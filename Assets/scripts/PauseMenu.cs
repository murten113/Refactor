using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #region References
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private bool isPaused = false;
    #endregion

    #region start and update
    //this part first disables the ui and makes listeners for all the buttons
    private void Start()
    {
        pauseMenuUI.SetActive(false);

        resumeButton.onClick.AddListener(Resume);
        mainMenuButton.onClick.AddListener(LoadMainMenu);
        quitButton.onClick.AddListener(QuitGame);
    }



    private void Update()
    {
        CheckPause();
    }
    #endregion

    #region pause check
    //this checks if the escape key gets pressed, if so then toggle between activating the pause menu or deactivating it
    private void CheckPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    #endregion

    #region pause
    //if this gets called activate the ui, pause the game time and unlock the cursor so the player can interact with the buttons
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }
    #endregion

    #region button functions
    //if this button gets pressed: hide the ui, resume the time and lock the cursor so the plaer can move normally
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    //if this button gets pressed: resume game time and load the main menu scene
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }


    //if this button gets pressed: quit the application and if youre running in th editor quit the editor player
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion
}
