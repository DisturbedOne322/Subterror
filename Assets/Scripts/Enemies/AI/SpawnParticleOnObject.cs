using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnParticleOnObject : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particleSys;

    [SerializeField, Header("Must implement IParticleSpawnerCaller")]
    private GameObject go;

    private IParticleSpawnerCaller particleSpawnerCaller;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out IParticleSpawnerCaller particleSpawnerCaller);
        if (particleSpawnerCaller!=null)
            particleSpawnerCaller.OnSpawnParticleAction += Caller_OnSpawnParticleAction;
    }

    private void OnDestroy()
    {
        if (particleSpawnerCaller != null)
            particleSpawnerCaller.OnSpawnParticleAction -= Caller_OnSpawnParticleAction;
    }

    private void Caller_OnSpawnParticleAction()
    {
        particleSys.Play(); 
    }
}
