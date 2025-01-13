using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int vidas = 3;
    public Text vidasTexto;
    public GameObject gameOverPanel;

    void Awake()
    {
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

    // 1. Te suscribes en OnEnable
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 2. Te desuscribes en OnDisable
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            GameObject canvasGO = GameObject.Find("Canvas");
            if (canvasGO != null)
            {
                Transform panelTransform = canvasGO.transform.Find("GameOverPanel");
                if (panelTransform != null)
                {
                    gameOverPanel = panelTransform.gameObject;
                    gameOverPanel.SetActive(false);

                    // BUSCAR el Botón dentro del Panel
                    Transform buttonTransform = gameOverPanel.transform.Find("BotonReiniciar");
                    if (buttonTransform != null)
                    {
                        // Suponiendo que tu botón se llama "BotonReiniciar"
                        Button botonReiniciar = buttonTransform.GetComponent<Button>();

                        // Elimina cualquier suscripción previa, por si acaso
                        botonReiniciar.onClick.RemoveAllListeners();

                        // Asigna el método que queremos llamar al hacer click
                        botonReiniciar.onClick.AddListener(ReiniciarJuego);
                    }
                }

                Transform vidasTextoTransform = canvasGO.transform.Find("VidasTexto");
                if (vidasTextoTransform != null)
                {
                    vidasTexto = vidasTextoTransform.GetComponent<Text>();
                }
            }

            ActualizarVidasUI();
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

    public void PerderVida()
    {
        vidas--;
        ActualizarVidasUI();

        if (vidas <= 0)
        {
            MostrarGameOver();
        }
    }

    void ActualizarVidasUI()
    {
        if (vidasTexto != null)
        {
            vidasTexto.text = "Vidas: " + vidas;
        }
    }

    void MostrarGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        vidas = 3;
        ActualizarVidasUI();
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
