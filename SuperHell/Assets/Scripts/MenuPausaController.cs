using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausaController : MonoBehaviour
{
    
    public GameObject pauseMenu;
    public static bool isPaused;
    void Start()
    {
        if (pauseMenu == null)
        {
            pauseMenu = GameObject.Find("PauseMenuPanel"); // Usa el nombre exacto del objeto en la jerarquía
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);  
        }
        else
        {
            Debug.LogWarning("pauseMenu no fue encontrado en la escena.");
        }

        isPaused = false;      
    }

    public void InitializePauseMenu()
    {
        GameObject canvasGO = GameObject.Find("Canvas");
        if (canvasGO != null)
        {
            Transform pauseMenuTransform = canvasGO.transform.Find("PauseMenuPanel");
            if (pauseMenuTransform != null)
            {
                pauseMenu = pauseMenuTransform.gameObject;
                pauseMenu.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (isPaused){
                ResumeGame();
            }else{
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (pauseMenu == null)
        {
            Debug.LogWarning("pauseMenu no está asignado.");
            return;
        }

        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void EndGame()
    { 
        Application.Quit();
    }

}
