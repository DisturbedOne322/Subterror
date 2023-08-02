using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loadScreen;

    [SerializeField]
    private GameObject optionsWindow;

    [SerializeField]
    private Animator playerMainMenuAnimator;

    private const string PLAYER_PLAY_ANIM = "Base Layer.PlayerMainMenuExit";
    private const string PLAYER_EXIT_ANIM = "Base Layer.PlayerMainMenuPlay";

    private const string LOAD_SCREEN_TRIGGER = "OnLoad";

    private readonly float playAnimDuration = 4;
    private readonly float exitAnimDuration = 4;

    public static bool playerActed { get; private set; }

    private void Start()
    {
        Time.timeScale = 1;
        playerActed = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnExitPressed()
    {
        playerMainMenuAnimator.Play(PLAYER_EXIT_ANIM);
        playerActed = true;
        StartCoroutine(ExitGame(exitAnimDuration));
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnPlayPressed()
    {
        playerMainMenuAnimator.Play(PLAYER_PLAY_ANIM);
        playerActed = true;
        StartCoroutine(PlayGame(playAnimDuration));
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator ExitGame(float delay)
    {
        loadScreen.GetComponent<Animator>().SetTrigger(LOAD_SCREEN_TRIGGER);
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }

    private IEnumerator PlayGame(float delay)
    {
        loadScreen.GetComponent<Animator>().SetTrigger(LOAD_SCREEN_TRIGGER);
        yield return new WaitForSeconds(delay);
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void OpenOptionsWindow()
    {
        optionsWindow.SetActive(true);
    }
}
