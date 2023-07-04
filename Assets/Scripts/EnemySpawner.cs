using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    //public bool trigger;
    public bool triggerMoveEnemy, pullBackEnemy;
    public int numberOfEnemies, enemyCount = 0;
    public float distanceBetweenEnemyAndPlayer;
    [HideInInspector] public GameObject[] enemiesObjects;
    //public int numberOfSpawnPoints;
    private GameObject[] spawnersList;
    private Vector2 enemiesPoolPosition = new Vector2(40, 40);
    private Vector2 cameraBounds, startingPosition, finalPosition;
    private EnemyScript enemyScript;
    private ArrowDirection arrowScript;

    // Start is called before the first frame update
    void Awake()
    {
        cameraBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        startingPosition = new Vector2(0, cameraBounds.y - 2);//offset may be mended
        finalPosition = new Vector2(0, -cameraBounds.y + 2);
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
            enemiesObjects[i] = (GameObject)Instantiate(enemyPrefab, enemiesPoolPosition, Quaternion.identity);
        }
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

        if (triggerMoveEnemy)
        {
            MoveEnemy();
        }
        else if(pullBackEnemy)
        {
            PullBackEnemy();
        }
    }

    public void SpawnEnemy()
    {
        enemiesObjects[enemyCount].SetActive(true);
        enemiesObjects[enemyCount].transform.position = spawnersList[Random.Range(0, 2)].transform.position;
        enemyScript = enemiesObjects[enemyCount].transform.GetChild(0).GetComponent<EnemyScript>();
        arrowScript = enemiesObjects[enemyCount].transform.GetChild(0).GetChild(0).GetComponent<ArrowDirection>();
        enemyScript.targetPosition = startingPosition;
        arrowScript.maxDistanceBetweenEnemyAndPlayer = Vector2.Distance(enemiesObjects[enemyCount].transform.position, finalPosition);
        //enemiesObjects[enemyCount].transform.GetChild(0).GetComponent<EnemyScript>().enabled = true;
        triggerMoveEnemy = true;
        Debug.Log("Enemy Count: " + enemyCount);
}

    public void MoveEnemy()
    {
        enemiesObjects[enemyCount].transform.position = Vector3.MoveTowards(enemiesObjects[enemyCount].transform.position, enemyScript.targetPosition, Time.deltaTime * enemyScript.speed);
        if((Vector2)enemiesObjects[enemyCount].transform.position == startingPosition)
        {
            enemyScript.targetPosition = finalPosition;
        }
        distanceBetweenEnemyAndPlayer = Vector2.Distance(enemiesObjects[enemyCount].transform.position, finalPosition);
        arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
    }

    public void CountEnemies()
    {
        enemyCount++;
        //if (enemyCount >= numberOfEnemies)
        //    enemyCount = 0;
    }

    private void PullBackEnemy()
    {
        enemiesObjects[enemyCount].transform.position = Vector3.MoveTowards(enemiesObjects[enemyCount].transform.position, startingPosition, Time.deltaTime * enemyScript.speed*2);
        if ((Vector2)enemiesObjects[enemyCount].transform.position == startingPosition)
        {
            triggerMoveEnemy = true;
            pullBackEnemy = false;
        }

    }
}
