using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{

    public static event Action<bool> OnGamePaused;

    [SerializeField]
    private GameObject pauseScreen;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private GameObject hintDisplay;

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.OnPauseAction += PauseResumeGame;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnPauseAction -= PauseResumeGame;
    }

    public void OpenSettingsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void ToMainScreen()
    {
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public void PauseResumeGame()
    {
        if(hintDisplay.activeSelf)
        {
            hintDisplay.SetActive(false);
            return;
        }
        if(optionsMenu.activeSelf)
        {
            optionsMenu.SetActive(false);
            return;
        }
        pauseScreen.SetActive(!pauseScreen.activeSelf);
        OnGamePaused?.Invoke(pauseScreen.activeSelf);      
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
