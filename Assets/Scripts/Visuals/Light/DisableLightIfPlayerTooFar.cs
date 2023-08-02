using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisableLightIfPlayerTooFar : MonoBehaviour
{
    private FlickeringLight flickeringLight;
    private PlayerMovement playerMovement;

    private Light2D light2D;

    private float maxDistance = 20f;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        flickeringLight = GetComponent<FlickeringLight>();
        playerMovement = GameManager.Instance.GetPlayerReference();   
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, playerMovement.transform.position);
        flickeringLight.enabled =
                light2D.enabled = distance < maxDistance;
    }
}
