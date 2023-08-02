using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject window;
    public void OnButtonPress()
    {
        window.SetActive(false);
    }

    private void Instance_OnPauseAction()
    {
        OnButtonPress();
    }
}
