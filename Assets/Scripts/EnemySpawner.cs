using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public bool trigger = true;
    GameObject[] _spawnersList;
    // Start is called before the first frame update
    void Awake()
    {
        _spawnersList = GameObject.FindGameObjectsWithTag("SpawnPoint");
        GameObject Enemy = Instantiate(EnemyPrefab);
        Enemy.transform.position = _spawnersList[Random.Range(0, _spawnersList.Length)].transform.position;
        /*foreach (var spawn in _spawnersList)
        {
            Debug.Log(spawn.name);
        }*/
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger)
        {
            trigger = false;
            GameObject Enemy = Instantiate(EnemyPrefab);
            Enemy.transform.position = _spawnersList[Random.Range(0, _spawnersList.Length)].transform.position;
        }
        trigger = false;
    }
}
