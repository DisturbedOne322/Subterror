using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(IsPlayerInRange))]
public class SaveGame : MonoBehaviour
{
    [SerializeField]
    public int uniqueSavePointID;
    private IsPlayerInRange isPlayerInRange;

    private CheckCanSaveGame checkCanSaveGame;

    private bool playerInRange;

    public event Action OnDisplayIcon;
    public static event Action OnGameSaved;
    public event Action OnReduceGlobalLight;
    private bool recentlySaved = false;

    private bool canSave;

    private void Start()
    {
        isPlayerInRange = GetComponent<IsPlayerInRange>();
        InputManager.Instance.OnInteract += Instance_OnInteract;
        isPlayerInRange.OnPlayerInRange += IsPlayerInRange_OnPlayerInRange;

        checkCanSaveGame = GetComponent<CheckCanSaveGame>();
        checkCanSaveGame.CanSaveGame += CheckCanSaveGame_CanSaveGame;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnInteract -= Instance_OnInteract;
        isPlayerInRange.OnPlayerInRange -= IsPlayerInRange_OnPlayerInRange;
        checkCanSaveGame.CanSaveGame -= CheckCanSaveGame_CanSaveGame;
    }

    private void CheckCanSaveGame_CanSaveGame(bool canSave)
    {
        this.canSave = canSave;
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        playerInRange = obj;
        if (!playerInRange)
            recentlySaved = false;
    }

    private void Instance_OnInteract()
    {
        if (!canSave)
            return;

        if (recentlySaved)
            return;

        if (playerInRange)
        {
            
            PlayerMovement player = GameManager.Instance.GetPlayerReference();

            SaveData saveData = new SaveData();
            saveData.savePoint = uniqueSavePointID;
            player.GetComponent<PlayerHealth>().RestoreFullHealth();
            saveData.bulletAmount = player.GetComponentInChildren<Shoot>().currentBulletNum;
            saveData.globalLightIntensity = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>().intensity;

            string SaveDataJSON = JsonUtility.ToJson(saveData);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveDataJSON);

            OnGameSaved?.Invoke();
            OnDisplayIcon?.Invoke();
            OnReduceGlobalLight?.Invoke();
            recentlySaved = true;
        }
    }
}
