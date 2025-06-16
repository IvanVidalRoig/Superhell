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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            vidas = 3;
            Time.timeScale = 1f;
            GameObject canvasGO = GameObject.Find("Canvas");
            if (canvasGO != null)
            {
                Transform panelTransform = canvasGO.transform.Find("GameOverPanel");
                if (panelTransform != null)
                {
                    gameOverPanel = panelTransform.gameObject;
                    gameOverPanel.SetActive(false);

                    Transform buttonTransform = gameOverPanel.transform.Find("BotonReiniciar");
                    if (buttonTransform != null)
                    {
                        Button botonReiniciar = buttonTransform.GetComponent<Button>();

                        botonReiniciar.onClick.RemoveAllListeners();

                        botonReiniciar.onClick.AddListener(ReiniciarJuego);
                    }
                }

                Transform vidasTextoTransform = canvasGO.transform.Find("VidasTexto");
                if (vidasTextoTransform != null)
                {
                    vidasTexto = vidasTextoTransform.GetComponent<Text>();
                }
                MenuPausaController pauseController = FindObjectOfType<MenuPausaController>();
                if (pauseController != null)
                {
                    pauseController.InitializePauseMenu();
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
