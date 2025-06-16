using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // Aseg�rate de que el nombre de la escena coincida con la escena de tu juego principal
        SceneManager.LoadScene("SampleScene");
    }

    public void RestarLevel(){
        Time.timeScale = 1f;
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
