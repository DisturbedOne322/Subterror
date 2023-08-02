using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnParticleOnObject : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSystem;

    [SerializeField, Header("Must implement IParticleSpawnerCaller")]
    private GameObject go;

    // Start is called before the first frame update
    void Start()
    {
        if(go.TryGetComponent<IParticleSpawnerCaller>(out IParticleSpawnerCaller caller))
            caller.OnSpawnParticleAction += Caller_OnSpawnParticleAction;
    }

    private void OnDestroy()
    {
        if (go.TryGetComponent<IParticleSpawnerCaller>(out IParticleSpawnerCaller caller))
            caller.OnSpawnParticleAction -= Caller_OnSpawnParticleAction;
    }

    private void Caller_OnSpawnParticleAction()
    {
        particleSystem.Play();
    }
}
