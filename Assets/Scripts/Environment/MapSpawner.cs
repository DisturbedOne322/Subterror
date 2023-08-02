using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mapParts;

    [SerializeField]
    private MapCheckpoint[] mapCheckpoints;

    private Dictionary<int, List<int>> savePointToMapPartsDict;

    private void Awake()
    {
        for (int i = 0; i < mapParts.Length; i++)
        {
            mapParts[i].SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadData.Instance.OnGameLoaded += Instance_OnGameLoaded;
        for(int i = 0; i < mapCheckpoints.Length; i++)
        {
            mapCheckpoints[i].OnSpawnNextMapPart += MapSpawner_OnSpawnNextMapPart;
        }

        savePointToMapPartsDict = new Dictionary<int, List<int>>
        {
            { 0, new List<int> { 0, 1 } },
            { 1, new List<int> { 1, 2 } },
            { 2, new List<int> { 2 } },
            { 3, new List<int> { 2, 3 } },
            { 4, new List<int> { 3, 4 } },
            { 5, new List<int> { 4, 5 } },
            { 6, new List<int> { 5 } }
        };
    }

    private void MapSpawner_OnSpawnNextMapPart(int nextMapPartId)
    {
        for(int i = 0; i < nextMapPartId - 1; i++)
        {
            mapParts[i].SetActive(false);
        }
        mapParts[nextMapPartId].SetActive(true);
    }

    private void Instance_OnGameLoaded(int lastCheckpointId)
    {
        List<int> parts = savePointToMapPartsDict[lastCheckpointId];
        for(int i = 0; i < parts.Count;i++)
        {
            mapParts[parts[i]].SetActive(true);
        }
    }
}
