using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPanel : MonoBehaviour
{
    public void Close(){
        Application.Quit();
    }

    public void StartGame(){
        SceneManager.LoadScene(1);
    }
}
