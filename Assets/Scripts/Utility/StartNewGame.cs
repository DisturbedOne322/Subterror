using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNewGame : MonoBehaviour
{
    [SerializeField]
    private GameObject agreeWindow;

    [SerializeField]
    private GameObject errorWindow;


    public void StartFreshGame()
    {
        if (SaveExists())
        {
            ShowAgreeMessage();
        }
        else
        {
            ShowErrorMessage();
        }
    }

    public void DeleteSave()
    {
        System.IO.File.Delete(Application.persistentDataPath + "/SaveData.json");
    }

    private void ShowAgreeMessage()
    {
        agreeWindow.SetActive(true);
    }

    private void ShowErrorMessage()
    {
        errorWindow.SetActive(true);
    }

    private bool SaveExists()
    {
        try
        {
            string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
            return true;
        }
        catch
        {
            Debug.LogError("Save does not exist");
            return false;
        }
    }
}
