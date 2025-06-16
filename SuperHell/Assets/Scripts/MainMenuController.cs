using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void RestarLevel(){
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToMainMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

        public void GoToControlesMenu(){
        SceneManager.LoadScene("ControlesMenu");
    }

    public void EndGame()
    { 
        Application.Quit();
    }
}
