using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    private Teleporter teleporter;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        teleporter = GetComponent<Teleporter>();
        teleporter.OnAppear += Teleporter_OnAppear;
    }

    private void OnDestroy()
    {
        if(teleporter != null)
            teleporter.OnAppear -= Teleporter_OnAppear;
    }

    private void Teleporter_OnAppear()
    {
       audioSource.Play();
    }

    
}
