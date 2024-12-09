using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // Asegúrate de que el nombre de la escena coincida con la escena de tu juego principal
        SceneManager.LoadScene("SampleScene");
    }

    public void EndGame()
    { 
        Application.Quit();
    }
}
