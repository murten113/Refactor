using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{

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
