using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    private Level1Logic gameState;

    public void ResetToGameMenu()
    {
        // Load Game Menu
        SceneManager.LoadScene(0);
    }

    public void NewGame(string sceneIndex)
    {
        // Load Main Game Scene
        SceneManager.LoadScene(1);
    }

    public void Options(string sceneIndex)
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
