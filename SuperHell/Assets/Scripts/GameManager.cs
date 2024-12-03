using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int vidas = 3; // Número inicial de vidas
    public Text vidasTexto; // Referencia al elemento de UI que mostrará las vidas
    public GameObject gameOverPanel; // Referencia al panel de Game Over

    void Awake()
    {
        // Implementar el patrón Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ActualizarVidasUI();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Método para restar una vida
    public void PerderVida()
    {
        vidas--;
        ActualizarVidasUI();

        if (vidas <= 0)
        {
            MostrarGameOver();
        }
        
    }

    // Actualizar el texto de vidas en la UI
    void ActualizarVidasUI()
    {
        if (vidasTexto != null)
        {
            vidasTexto.text = "Vidas: " + vidas.ToString();
        }
    }

    // Mostrar la pantalla de Game Over
    void MostrarGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Opcional: Pausar el juego
        Time.timeScale = 0f;

        // Reiniciar el juego después de unos segundos
        Invoke("ReiniciarJuego", 3f); // Espera 3 segundos antes de reiniciar
    }

    // Reiniciar el juego
    void ReiniciarJuego()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo esté activo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        vidas = 3; // Reiniciar el número de vidas
        ActualizarVidasUI();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}
