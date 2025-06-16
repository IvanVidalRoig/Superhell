using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPausaController : MonoBehaviour
{
    
    public GameObject pauseMenu;
    public static bool isPaused;
    void Start()
    {
        pauseMenu.SetActive(false);  
        isPaused = false;      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("e")){
            if (isPaused){
                ResumeGame();
            }else{
                PauseGame();
            }
        }
    }

    public void PauseGame(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
