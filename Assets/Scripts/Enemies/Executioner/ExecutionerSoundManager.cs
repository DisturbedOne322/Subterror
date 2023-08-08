using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ExecutionerSoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private PlayerMovement player;

    [SerializeField]
    private AudioClip sliceSound1;
    [SerializeField] 
    private AudioClip sliceSound2;
    [SerializeField]
    private AudioClip spawnSound;
    private ExecutionerHealth health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<ExecutionerHealth>();
        audioSource = GetComponent<AudioSource>();
        player = GameManager.Instance.GetPlayerReference();
        health.OnDeath += Health_OnDeath;
    }

    private void OnDestroy()
    {
        if(health != null)
            health.OnDeath -= Health_OnDeath;
    }

    private void Health_OnDeath()
    {
        StartCoroutine(DecreaseVolumeOnDeath());    
    }

    private IEnumerator DecreaseVolumeOnDeath()
    {
        int stepsAmount = 20;
        float step = audioSource.volume / stepsAmount;
        for(int i = 0; i < stepsAmount; i++)
        {
            audioSource.volume -= step;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(spawnSound);
        audioSource.volume = 1;
    }

    private void PlaySliceSound1()
    {
        audioSource.PlayOneShot(sliceSound1);
    }
    private void PlaySliceSound2()
    {
        audioSource.PlayOneShot(sliceSound2);
    }
}
