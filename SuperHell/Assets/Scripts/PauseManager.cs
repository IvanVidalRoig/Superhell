using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public GameObject pauseMenuPanel;
    bool isPaused = false;

    private void Awake()
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
            // 1) Buscamos el Canvas
            GameObject canvasGO = GameObject.Find("Canvas");
            if (canvasGO != null)
            {
                // 2) Buscamos el PauseMenuPanel
                Transform panelTransform = canvasGO.transform.Find("PauseMenuPanel");
                if (panelTransform != null)
                {
                    pauseMenuPanel = panelTransform.gameObject;
                    pauseMenuPanel.SetActive(false);

                    // 3) Buscamos los botones y reasignamos sus onClick
                    Transform resumeButtonTransform = pauseMenuPanel.transform.Find("ButtonResume");
                    if (resumeButtonTransform != null)
                    {
                        Button resumeButton = resumeButtonTransform.GetComponent<Button>();
                        resumeButton.onClick.RemoveAllListeners();
                        resumeButton.onClick.AddListener(ReanudarJuego);
                    }

                    Transform exitButtonTransform = pauseMenuPanel.transform.Find("ButtonExit");
                    if (exitButtonTransform != null)
                    {
                        Button exitButton = exitButtonTransform.GetComponent<Button>();
                        exitButton.onClick.RemoveAllListeners();
                        exitButton.onClick.AddListener(SalirDelJuego);
                    }
                }
            }

            // Asegurarnos de reiniciar variables
            isPaused = false;
            Time.timeScale = 1f;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PausarJuego();
            else ReanudarJuego();
        }
    }

    public void PausarJuego()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ReanudarJuego()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void SalirDelJuego()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("HubScene"); 
    }
}
