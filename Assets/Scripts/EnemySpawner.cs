using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    public bool trigger;
    public int numberOfEnemies;
    public int enemyCount = 0;
    //public int numberOfSpawnPoints;
    private GameObject[] spawnersList;
    private GameObject[] enemiesObjects;
    private Vector2 enemiesPoolPosition = new Vector2(40, 40);
    private Vector2 cameraBounds;
    private EnemyScript enemyScript;

    // Start is called before the first frame update
    void Awake()
    {
        cameraBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        spawnersList = GameObject.FindGameObjectsWithTag("SpawnPoint");
        //For now we specify spawn points' locations, needs optimization further
        spawnersList[0].transform.position = new Vector2(-cameraBounds.x - 2, cameraBounds.y);
        spawnersList[1].transform.position = new Vector2(cameraBounds.x + 2, cameraBounds.y);
        //GameObject Enemy = Instantiate(EnemyPrefab);
        //Enemy.transform.position = _spawnersList[Random.Range(0, _spawnersList.Length)].transform.position;
        /*foreach (var spawn in _spawnersList)
        {
            Debug.Log(spawn.name);
        }*/
        enemiesObjects = new GameObject[numberOfEnemies];
        for (int i = 0; i < numberOfEnemies; i++)
        {
            enemiesObjects[i] = (GameObject)Instantiate(enemyPrefab, spawnersList[0].transform.position, Quaternion.identity);
        }
        //for (int i = numberOfEnemies / 2; i < numberOfEnemies; i++)
        //{
        //    enemiesObjects[i] = (GameObject)Instantiate(enemyPrefab, spawnersList[1].transform.position, Quaternion.identity);
        //}
    }

    private void Start()
    {
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        //if (trigger)
        //{
        //    trigger = false;
        //    GameObject Enemy = Instantiate(EnemyPrefab);
        //    Enemy.transform.position = _spawnersList[Random.Range(0, _spawnersList.Length)].transform.position;
        //}
        //trigger = false;

        if (trigger)
        {
            MoveEnemy();
        }
    }

    public void SpawnEnemy()
    {
        enemiesObjects[enemyCount].SetActive(true);
        enemyScript = enemiesObjects[enemyCount].transform.GetChild(0).GetComponent<EnemyScript>();
        //enemiesObjects[enemyCount].transform.GetChild(0).GetComponent<EnemyScript>().enabled = true;
        if (enemyCount >= enemiesObjects.Length)
            enemyCount = 0;
        trigger = true;
        Debug.Log("Enemy Count: " + enemyCount);
    }

    public void MoveEnemy()
    {
        enemiesObjects[enemyCount].transform.position = Vector3.MoveTowards(enemiesObjects[enemyCount].transform.position, enemyScript.targetPosition, Time.deltaTime * enemyScript.speed);
    }


}
