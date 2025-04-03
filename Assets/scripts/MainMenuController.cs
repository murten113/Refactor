using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{

    public Button loadGameButton;
    public Button quitGameButton;


    //make listeners so when the button gets pressed it loads that function
    private void Start()
    {
        loadGameButton.onClick.AddListener(LoadGame);
        quitGameButton.onClick.AddListener(QuitGame);
    }


    //when the button assigned to this function gets pressed it loads the scene called MainScene
    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene");
    }



    //when the button assigned to this function gets pressed it closes the application or closes the editro if they are in the editor
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
