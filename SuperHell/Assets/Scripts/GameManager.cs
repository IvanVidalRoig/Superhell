using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int vidas = 3; // N�mero inicial de vidas
    public Text vidasTexto; // Referencia al elemento de UI que mostrar� las vidas
    public GameObject gameOverPanel; // Referencia al panel de Game Over

    void Awake()
    {
        // Implementar el patr�n Singleton
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

    // M�todo para restar una vida
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

        // Reiniciar el juego despu�s de unos segundos
        Invoke("ReiniciarJuego", 3f); // Espera 3 segundos antes de reiniciar
    }

    // Reiniciar el juego
    void ReiniciarJuego()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo est� activo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        vidas = 3; // Reiniciar el n�mero de vidas
        ActualizarVidasUI();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
}
