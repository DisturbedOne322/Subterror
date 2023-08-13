using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class LoadData : MonoBehaviour
{
    public static LoadData Instance;

    private SaveData saveData;
    public event Action<int> OnGameLoaded;

    private bool saveDataExists = false;

    [SerializeField]
    private Transform initialSpawnPoint;

    [SerializeField]
    private GameObject loadScreen;
    private Animator loadScreenAnimator;
    private const string LOAD_SCREEN_TRIGGER = "OnLoad";
    private const string UNLOAD_SCREEN_TRIGGER = "OnUnload";

    private PlayerMovement player;
    private PlayerHealth playerHealth;

    private int LastCheckpointID;

    private int mainMenuSceneID = 0;
    private int gameSceneID = 1;
    private float sceneReloadDelay = 3.5f;

    private void Awake()
    {
        Instance = this;
        loadScreenAnimator = loadScreen.GetComponent<Animator>();
        LoadSaveData();
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.05f);
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.OnDeath += Player_OnPlayerDied;
        player.OnPlayerTeleported += Player_OnPlayerTeleported;
        player.OnPlayerTeleportedArrived += Player_OnPlayerTeleportedArrived;

        LoadGame(player);
    }

    private void OnDestroy()
    {
        playerHealth.OnDeath -= Player_OnPlayerDied;

        player.OnPlayerTeleported -= Player_OnPlayerTeleported;
        player.OnPlayerTeleportedArrived -= Player_OnPlayerTeleportedArrived;
    }

    private void Player_OnPlayerTeleportedArrived()
    {
        ResetAnimTriggers();
        loadScreenAnimator.SetTrigger(UNLOAD_SCREEN_TRIGGER);
    }

    private void Player_OnPlayerTeleported()
    {
        ResetAnimTriggers();
        loadScreenAnimator.SetTrigger(LOAD_SCREEN_TRIGGER);
    }

    private void Player_OnPlayerDied()
    {
        ResetAnimTriggers();
        loadScreenAnimator.SetTrigger(LOAD_SCREEN_TRIGGER);
        StartCoroutine(ReloadScene());
    }

    private void ResetAnimTriggers()
    {
        loadScreenAnimator.ResetTrigger(UNLOAD_SCREEN_TRIGGER);
        loadScreenAnimator.ResetTrigger(LOAD_SCREEN_TRIGGER);
    }

    private IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(sceneReloadDelay);
        SceneManager.LoadScene(gameSceneID);
    }

    public void LoadMainMenuScene()
    {
        ResetAnimTriggers();
        loadScreenAnimator.SetTrigger(LOAD_SCREEN_TRIGGER);
        StartCoroutine(LoadSceneWithDelay(0, 2));
    }

    private IEnumerator LoadSceneWithDelay(int id, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(id);
    }

    public void ExitGame()
    {
        ResetAnimTriggers();
        loadScreenAnimator.SetTrigger(LOAD_SCREEN_TRIGGER);
        StartCoroutine(ExitGameDelay());
    }

    private IEnumerator ExitGameDelay()
    {
        yield return new WaitForSecondsRealtime(2);
        Application.Quit();
    }


    private void LoadGame(PlayerMovement player)
    {
        OnGameLoaded?.Invoke(LastCheckpointID);
        if (saveDataExists)
        {
            GameObject[] savePoints = GameObject.FindGameObjectsWithTag("SavePoint");
            for (int i = 0; i < savePoints.Length; i++)
            {
                if (savePoints[i].GetComponent<SaveGame>().uniqueSavePointID == saveData.savePoint)
                {
                    player.GetComponentInChildren<Shoot>().currentBulletNum = saveData.bulletAmount;
                    player.transform.position = savePoints[i].transform.position;
                    GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>().intensity = saveData.globalLightIntensity;
                }
            }
        }
        else
        {
            player.transform.position = initialSpawnPoint.position;
        }

    }

    public void LoadEndGameScene()
    {
        SceneManager.LoadScene(mainMenuSceneID);
    }

    private void LoadSaveData()
    {
        try
        {
            string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
            saveData = JsonUtility.FromJson<SaveData>(data);
            LastCheckpointID = saveData.savePoint;
            saveDataExists = true;

        }
        catch
        {
            LastCheckpointID = 0;
        }
    }
}
