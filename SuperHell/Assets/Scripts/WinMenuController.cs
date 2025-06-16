using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinMenuController : MonoBehaviour
{

    public GameObject endGameCanvas;

    private void OnTriggerEnter(Collider collider){
        if (collider.CompareTag("Player")){
            endGameCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
