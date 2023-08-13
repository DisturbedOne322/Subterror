using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof(SetTimeScaleTo0), 2);
    }

    private void SetTimeScaleTo0()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
