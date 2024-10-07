using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool paused;

    public GameObject menu;

    private void Start()
    {
        Resume();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Pause();
        }
    }
    
    public void Pause()
    {
        Time.timeScale = 0;
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        paused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        paused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
