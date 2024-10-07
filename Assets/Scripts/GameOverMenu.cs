using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject menu;

    private void Start()
    {
        menu.SetActive(false);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        menu.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Diner"); //restarts diner scene
    }

    public void Quit()
    {
        Application.Quit();
    }
}
