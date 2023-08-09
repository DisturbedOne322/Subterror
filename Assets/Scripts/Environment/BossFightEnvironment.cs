using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossFightEnvironment : MonoBehaviour
{
    [SerializeField]
    private GameObject closeRoomCollider;
    [SerializeField]
    private Light2D outsideLight;
    [SerializeField]
    private GameObject openDoor;
    [SerializeField]
    private GameObject closeDoor;
    [SerializeField]
    private GameObject teleport;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnBossFightStarted += Instance_OnBossFightStarted;
        BossFightEnvironment go = GameObject.FindObjectOfType<BossFightEnvironment>();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnBossFightStarted -= Instance_OnBossFightStarted;
    }

    private void Instance_OnBossFightStarted()
    {
        outsideLight.intensity = 0;
        closeRoomCollider.SetActive(true);
        openDoor.SetActive(false);
        closeDoor.SetActive(true);
        SoundManager.Instance.PlayBossFightCloseDoorSound();
        teleport.SetActive(true);
    }

}
