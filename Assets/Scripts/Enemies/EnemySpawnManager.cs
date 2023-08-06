using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{

    #region Tentacles
    [Header("Tentacle")]
    [SerializeField]
    private GameObject tentaclePrefab;
    #endregion
    #region Executioner
    [Header("Executioner")]
    [SerializeField]
    private GameObject executionerPrefab;
    private GameObject executioner;
    #endregion

    [Header("Hell hound")]
    #region Hell hound
    [SerializeField]
    private GameObject hellHoundPrefab;
    [SerializeField]
    private GameObject[] corpsePrefabs;
    #endregion
    
    #region Executiner Mini Boss Fight
    [Header("Executioner Mini Boss Fight")]
    [SerializeField]
    private ExecutionerMiniBossFightAreaTrigger executionerMiniBossFightTriggerArea;
    public event Action OnMiniBossFightStarted;
    public event Action OnBossFightFinished;
    #endregion
    #region Ghost
    [Header("Ghost, must be only 1 in a map section")]

    [SerializeField]
    private GameObject ghostPrefab;
    #endregion
    #region Teleporter
    [Header("Teleporter, must be only 1 in a map section")]

    [SerializeField]
    private GameObject teleportedPrefab;
    private GameObject teleporter;
    #endregion
    #region Player Fake
    [Header("Player fake")]
    [SerializeField]
    private GameObject fakePrefab;
    #endregion

    [SerializeField]
    private GameObject initiateBossFightTrigger;

    List<GameObject> spawnedExecutioners = new List<GameObject>();

    #region Checkpoints
    [SerializeField]
    private Checkpoints[] checkpoints;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
       // LoadData.Instance.OnGameLoaded += LoadData_OnGameLoaded;

        executioner = Instantiate(executionerPrefab, Vector3.zero, Quaternion.identity);
        executioner.SetActive(false);

        teleporter = Instantiate(teleportedPrefab, Vector3.zero, Quaternion.identity);
        teleporter.SetActive(false);

        executionerMiniBossFightTriggerArea.OnPlayerStartedBossFight += ExecutionerMiniBossFightTriggerArea_OnPlayerStartedBossFight;

        for(int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].OnSpawnNextAreaEnemies += EnemySpawnManager_OnSpawnNextAreaEnemies;
        }
    }

    private void OnDestroy()
    {
        executionerMiniBossFightTriggerArea.OnPlayerStartedBossFight -= ExecutionerMiniBossFightTriggerArea_OnPlayerStartedBossFight;

        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].OnSpawnNextAreaEnemies -= EnemySpawnManager_OnSpawnNextAreaEnemies;
        }
    }

    private void EnemySpawnManager_OnSpawnNextAreaEnemies(int nextCheckpointID)
    {
        if (nextCheckpointID > checkpoints.Length)
            return;

        SpawnEnemies(nextCheckpointID);
    }

    //instead of respawning enemies, scene is reloaded
    //spawn the enemies before the last checkpoint because otherwise player would be able to respawn at checkpoint without any threat
    //private void LoadData_OnGameLoaded(int checkpointID)
    //{
    //    if (checkpointID == 0)
    //        SpawnEnemies(0);
    //}

    private void SpawnEnemies(int i)
    {
        SpawnHellHoundPack(checkpoints[i].GetComponentsInChildren<HellHoundParentObject>());
        SpawnAttackTentacles(checkpoints[i].GetComponentsInChildren<TentacleAttackParentObject>());
        SpawnIdleTentacles(checkpoints[i].GetComponentsInChildren<TentaclesIdleParentObject>());
        ListenToOnPlayerEnterExecutionerTriggers(checkpoints[i].GetComponentsInChildren<ExecutionerTriggerArea>(true));
        SpawnGhost(checkpoints[i].GetComponentInChildren<GhostParentObject>());
        SpawnTeleporter(checkpoints[i].GetComponentInChildren<TeleporterParentObject>());
        SpawnPlayerFakes(checkpoints[i].GetComponentsInChildren<PlayerFakeParentObject>());
    }

    #region regular enemies spawn logic
    private void ListenToOnPlayerEnterExecutionerTriggers(ExecutionerTriggerArea[] exec)
    {
        bool respawn = false;
        for(int i = 0; i < exec.Length; i++)
        {
            if (!exec[i].gameObject.activeSelf)
            {
                respawn = true;
                exec[i].gameObject.SetActive(true);
            }

        }

        if (respawn)
            return;

        if (exec != null)
        {
            for(int i = 0; i < exec.Length;i++)
                exec[i].OnPlayerEnteredTriggerArea += Exec_OnPlayerEnteredTriggerArea;
        }
    }
    private void Exec_OnPlayerEnteredTriggerArea(Transform parent)
    {
        executioner.SetActive(false);
        executioner.SetActive(true);
        executioner.transform.position = parent.position;
    }

    int lastRandomIndex = -1;

    private void SpawnHellHoundPack(HellHoundParentObject[] parent)
    {
        if (parent == null)
            return;

        for(int i = 0; i < parent.Length;i++)
        {
            int rand;
            do
            {
                rand = UnityEngine.Random.Range(0, corpsePrefabs.Length);
            } while (rand == lastRandomIndex);

            lastRandomIndex = rand;

            GameObject corpse = Instantiate(corpsePrefabs[rand]
                , new Vector3(0,0,0), Quaternion.identity);
            corpse.transform.position = parent[i].transform.position;
            corpse.transform.parent = parent[i].transform;

            int numOfDogs = UnityEngine.Random.Range(2, 4);

            int side = 1;

            float[] occipiedPositions = new float[numOfDogs];
            //too big spawn offset can cause infinite loop
            float spawnOffset = 0.3f;

            for (int j = 0; j < numOfDogs; j++)
            {
                float offset;
                do
                {
                    offset = UnityEngine.Random.Range(-1.5f, -0.5f) * side;
                } while (IsPositionOccupied(offset, occipiedPositions, spawnOffset));

                occipiedPositions[j] = offset;

                GameObject temp = Instantiate(hellHoundPrefab, parent[i].transform);
                temp.transform.position += new Vector3(offset, 0, 0);

                Vector3 scale = temp.transform.localScale;
                scale.x *= side;
                temp.transform.localScale = scale;

                side *= -1;
            }
        }
    }

    private bool IsPositionOccupied(float position, float[] positions, float offset)
    {
        for(int i = 0; i < positions.Length;i++)
        {
            if(position < positions[i] + offset && position > positions[i] - offset)
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnIdleTentacles(TentaclesIdleParentObject[] parent)
    {
        if(parent == null) 
            return;


        tentaclePrefab.GetComponent<TentacleStateManager>().initialState = TentacleStateManager.InitialState.idleState;

        for (int i = 0; i < parent.Length; i++)
        {
            Instantiate(tentaclePrefab, parent[i].transform);
        }

    }

    private void SpawnAttackTentacles(TentacleAttackParentObject[] parent)
    {
        if (parent == null)
            return;

        tentaclePrefab.GetComponent<TentacleStateManager>().initialState = TentacleStateManager.InitialState.attackState;

        for (int i = 0; i < parent.Length; i++)
        {
            Instantiate(tentaclePrefab, parent[i].transform);
        }
    }

    private void SpawnGhost(GhostParentObject parent)
    {
        if (parent == null)
            return;

        GameObject ghost;

        ghost = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        ghost.transform.parent = parent.transform;
        ghost.transform.position = parent.transform.position;
    }

    private void SpawnTeleporter(TeleporterParentObject parent)
    {
        if (parent == null)
            return;

        teleporter.SetActive(true);
        teleporter.transform.parent = parent.transform;
        teleporter.transform.position = parent.transform.position;
    }

    private void SpawnPlayerFakes(PlayerFakeParentObject[] parent)
    {
        if (parent == null)
            return;

        for(int i = 0;i < parent.Length;i++)
        {
            GameObject temp = Instantiate(fakePrefab, parent[i].transform);
            temp.SetActive(true);
        }
    }
    #endregion

    #region mini boss fight
    private bool miniBossFightStarted = false;

    private void ExecutionerMiniBossFightTriggerArea_OnPlayerStartedBossFight(Transform[] spawnPositions)
    {
        if (miniBossFightStarted)
            return;

        miniBossFightStarted = true;
        OnMiniBossFightStarted?.Invoke();
        StartCoroutine(SpawnExecutioners(spawnPositions));
    }

    private IEnumerator SpawnExecutioners(Transform[] spawnPositions)
    {
        float delayBetweenSpawns = 3.5f;
        for (int i = 1; i < spawnPositions.Length; i++)
        {
            GameObject exec = Instantiate(executionerPrefab, spawnPositions[i].position, Quaternion.identity);
            exec.GetComponent<ExecutionerHealth>().SetHealthTo1();
            exec.GetComponent<ExecutionerApproachPlayer>().SetSpeed(UnityEngine.Random.Range(0.15f, 0.25f));
            exec.GetComponent<DissolveOnDeath>().SetDissolveTicktime(0.015f);

            spawnedExecutioners.Add(exec);

            delayBetweenSpawns -= 0.05f;
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        StartCoroutine(CheckPlayerFinishedBossFight());
    }

    private IEnumerator CheckPlayerFinishedBossFight()
    {
        while(!CheckEveryExecutionerDead())
        {
            yield return null;
        }      

        OnBossFightFinished?.Invoke();
    }

    private bool CheckEveryExecutionerDead()
    {
        for (int i = 0; i < spawnedExecutioners.Count; i++)
        {
            if (spawnedExecutioners[i].activeSelf)
                return false;
        }
        return true;
    }
    #endregion
}
