using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Elevator))]
public class ElevatorSoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip elevatorArrivedBeep;
    [SerializeField]
    private AudioClip elevatorArrivedScreech;
    [SerializeField]
    private AudioClip elevatorDepartedScreech;
    [SerializeField]
    private AudioClip elevatorDoorsOpen;

    [SerializeField]
    private AudioClip breakSound;

    private PlayerMovement player;

    [SerializeField]
    private EnemySpawnManager enemySpawnManager;

    private Elevator elevator;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        elevator = GetComponent<Elevator>();
        elevator.OnArrived += ElevatorSoundManager_OnArrived;
        elevator.OnDeparted += ElevatorSoundManager_OnDeparted;

        player = GameManager.Instance.GetPlayerReference();
        if (enemySpawnManager != null)
        {
            enemySpawnManager.OnMiniBossFightStarted += EnemySpawnManager_OnBossFightStarted;
            enemySpawnManager.OnBossFightFinished += EnemySpawnManager_OnBossFightFinished;
        }
    }

    private void OnDestroy()
    {
        elevator.OnArrived -= ElevatorSoundManager_OnArrived;
        elevator.OnDeparted -= ElevatorSoundManager_OnDeparted;

        if (enemySpawnManager != null)
        {
            enemySpawnManager.OnMiniBossFightStarted -= EnemySpawnManager_OnBossFightStarted;
            enemySpawnManager.OnBossFightFinished -= EnemySpawnManager_OnBossFightFinished;
        }
    }

    private void EnemySpawnManager_OnBossFightFinished()
    {
        audioSource.Play();
    }

    private void EnemySpawnManager_OnBossFightStarted()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(breakSound);
    }

    private void ElevatorSoundManager_OnDeparted()
    {
        audioSource.PlayOneShot(elevatorDepartedScreech);
        audioSource.PlayOneShot(elevatorDoorsOpen);
        audioSource.Play();
    }

    private void ElevatorSoundManager_OnArrived()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(elevatorArrivedScreech);
        audioSource.PlayOneShot(elevatorArrivedBeep);
        audioSource.PlayOneShot(elevatorDoorsOpen);
    }
}
