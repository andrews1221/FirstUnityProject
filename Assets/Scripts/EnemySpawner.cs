using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab, miniBoss1Prefab, miniBoss2Prefab;
    [SerializeField] private Player player;
    //public bool trigger;
    public bool triggerMoveEnemy, pullBackEnemy;
    public int numberOfEnemies, enemyCount = 0, miniBossCount;
    public float distanceBetweenEnemyAndPlayer;
    public ProgressController progressController;
    public BlockPhase blockphaseController;
    public GameObject currentMiniBoss;
    [HideInInspector] public GameObject[] enemiesObjects;
    [HideInInspector] public Vector2 oldEnemyPosition, startingPosition;
    [HideInInspector] public bool enemyTime, miniBossTime, bossTime;
    //public int numberOfSpawnPoints;
    private GameObject[] spawnersList;
    private GameObject miniBoss1, miniBoss2;
    private Vector2 enemiesPoolPosition = new Vector2(40, 40);
    private Vector2 cameraBounds, finalPosition;
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
            enemiesObjects[i].transform.GetChild(0).GetComponent<EnemyScript>().number = i;
            enemiesObjects[i].SetActive(false);
        }

        //Instantiaye miniBosses
        miniBoss1 = (GameObject)Instantiate(miniBoss1Prefab, enemiesPoolPosition, Quaternion.identity);
        miniBoss2 = (GameObject)Instantiate(miniBoss2Prefab, enemiesPoolPosition, Quaternion.identity);
        //miniBoss1.SetActive(false);
        //miniBoss2.SetActive(false);

        progressController.SetProgressBarMaxValue(numberOfEnemies);
    }

    private void Start()
    {
        DetermineEnemyToSpawn();
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
        enemyScript.initialSpeed = 2;
        DetermineEnemySpeed(enemyScript);
        player.DeterminePlayerAttackBasedOnMaterial(player.material, enemyScript.material);
        arrowScript = enemiesObjects[enemyCount].transform.GetChild(0).GetChild(0).GetComponent<ArrowDirection>();
        //Assign new direction
        enemyScript.arrowDirection = Random.Range(1, 5);
        arrowScript.ChooseArrowDirection(enemyScript.arrowDirection);

        enemyScript.targetPosition = startingPosition;
        arrowScript.maxDistanceBetweenEnemyAndPlayer = Vector2.Distance(enemiesObjects[enemyCount].transform.position, finalPosition);
        //enemiesObjects[enemyCount].transform.GetChild(0).GetComponent<EnemyScript>().enabled = true;
        triggerMoveEnemy = true;
        enemyTime = true;
        miniBossTime = bossTime = false;
        //Debug.Log("Enemy Count: " + enemyCount);
    }

    public void MoveEnemy()
    {
        if (enemyTime)
            MoveBasicEnemy();
        else if (miniBossTime)
            MoveMiniBoss();
    }

    private void MoveBasicEnemy()
    {
        enemiesObjects[enemyCount].transform.position = Vector3.MoveTowards(enemiesObjects[enemyCount].transform.position, enemyScript.targetPosition, Time.deltaTime * enemyScript.speed);
        if ((Vector2)enemiesObjects[enemyCount].transform.position == startingPosition)
        {
            enemyScript.targetPosition = finalPosition;
        }
        else if ((Vector2)enemiesObjects[enemyCount].transform.position == finalPosition)
        {
            blockphaseController.blockPhase = true;
            blockphaseController.SpawnBlockButtons();
            enemyScript.AttackPlayer(enemyScript.attack);
            triggerMoveEnemy = false;
        }
        distanceBetweenEnemyAndPlayer = Vector2.Distance(enemiesObjects[enemyCount].transform.position, finalPosition);
        arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
        enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
    }

    private void MoveMiniBoss()
    {
        currentMiniBoss.transform.position = Vector3.MoveTowards(currentMiniBoss.transform.position, enemyScript.targetPosition, Time.deltaTime * enemyScript.speed);
        if ((Vector2)currentMiniBoss.transform.position == startingPosition)
        {
            enemyScript.targetPosition = finalPosition;
        }
        else if ((Vector2)currentMiniBoss.transform.position == finalPosition)
        {
            blockphaseController.blockPhase = true;
            blockphaseController.SpawnBlockButtons();
            enemyScript.AttackPlayer(enemyScript.attack);
            triggerMoveEnemy = false;
        }
        distanceBetweenEnemyAndPlayer = Vector2.Distance(currentMiniBoss.transform.position, finalPosition);
        arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
        enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
    }

    public void CountEnemies()
    {
        enemyCount++;
        if (enemyCount >= numberOfEnemies)
        {
            enemyCount = 0;
        }
    }

    private void PullBackEnemy()
    {
        if (enemyTime)
        {
            enemiesObjects[enemyCount].transform.position = Vector3.MoveTowards(enemiesObjects[enemyCount].transform.position, new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + 2), Time.deltaTime * enemyScript.speed * 2);
            if ((Vector2)enemiesObjects[enemyCount].transform.position == new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + 2))
            {
                triggerMoveEnemy = true;
                pullBackEnemy = false;
            }
            distanceBetweenEnemyAndPlayer = Vector2.Distance(enemiesObjects[enemyCount].transform.position, finalPosition);
            arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
            enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
        }
        else if (miniBossTime)
        {
            currentMiniBoss.transform.position = Vector3.MoveTowards(currentMiniBoss.transform.position, new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + 2), Time.deltaTime * enemyScript.speed * 2);
            if ((Vector2)currentMiniBoss.transform.position == new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + 2))
            {
                triggerMoveEnemy = true;
                pullBackEnemy = false;
            }
            distanceBetweenEnemyAndPlayer = Vector2.Distance(currentMiniBoss.transform.position, finalPosition);
            arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
            enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
        }
    }

    private void DetermineEnemySpeed(EnemyScript enemy)
    {
        //Debug.Log("Enemy Ratio: " + (float)enemyCount / (float)numberOfEnemies);
        for (float i = 0; i <= 1; i += 0.1f)
        {
            //if (i == 0 || (float)enemyCount / (float)numberOfEnemies <= 0.1f)
            //{
            //    Debug.Log("Speed Increased");
            //    //enemy.speed = enemy.initialSpeed;
            //    break;
            //}
            if ((float)enemyCount / (float)numberOfEnemies > i && (float)enemyCount / (float)numberOfEnemies <= (i + 0.1f))
            {
                enemy.speed = enemy.initialSpeed*(1 + i);
                Debug.Log("Speed Increased " + enemy.speed);
                break;
            }
        }
        //    Debug.Log("Enemy Ratio: " + (float)enemyCount / (float)numberOfEnemies);
        //    if ((float)enemyCount / (float)numberOfEnemies <= 0.1f)
        //    {
        //        Debug.Log("Speed Increased");
        //        enemy.speed = enemy.initialSpeed;
        //    }
        //    else if ((float) enemyCount / (float)numberOfEnemies > 0.1f && (float)enemyCount / (float)numberOfEnemies <= 0.2f)
        //    {
        //        Debug.Log("Speed Increased");
        //        enemy.speed = enemy.initialSpeed * 1.1f;
        //    }
        //    else if ((float)enemyCount / (float)numberOfEnemies > 0.2f && (float)enemyCount / (float)numberOfEnemies <= 0.3f)
        //    {
        //        Debug.Log("Speed Increased");
        //        enemy.speed = enemy.initialSpeed * 1.2f;
        //    }
        //    else if ((float)enemyCount / (float)numberOfEnemies > 0.3f && (float) enemyCount / (float)numberOfEnemies <= 0.4f)
        //    {
        //        Debug.Log("Speed Increased");
        //        enemy.speed = enemy.initialSpeed * 1.3f;
        //    }
        //    else
        //        enemy.speed = enemy.initialSpeed * 1.5f;
        //}
    }

    public void SpawnMiniBoss()
    {
        if (miniBossCount == 0)
        {
            //miniBoss1.SetActive(true);
            currentMiniBoss = miniBoss1;
            currentMiniBoss.SetActive(true);
            Debug.Log("Mini Boss: " + currentMiniBoss);
        }
        else if(miniBossCount == 1)
        {
            //miniBoss2.SetActive(true);
            currentMiniBoss = miniBoss2;
            currentMiniBoss.SetActive(true);
        }
        currentMiniBoss.transform.position = spawnersList[Random.Range(0, 2)].transform.position;
        enemyScript = currentMiniBoss.transform.GetChild(0).GetComponent<EnemyScript>();
        enemyScript.health = enemyScript.maxHealth;
        player.DeterminePlayerAttackBasedOnMaterial(player.material, enemyScript.material);
        arrowScript = currentMiniBoss.transform.GetChild(0).GetChild(0).GetComponent<ArrowDirection>();
        //Assign new direction
        //enemyScript.arrowDirection = Random.Range(1, 5);
        arrowScript.ChooseArrowDirection(enemyScript.arrowDirection);

        enemyScript.targetPosition = startingPosition;
        arrowScript.maxDistanceBetweenEnemyAndPlayer = Vector2.Distance(currentMiniBoss.transform.position, finalPosition);
        //enemiesObjects[enemyCount].transform.GetChild(0).GetComponent<EnemyScript>().enabled = true;
        triggerMoveEnemy = true;
        miniBossTime = true;
        enemyTime = bossTime = false;
        miniBossCount++;
        if (miniBossCount >= 2)
            miniBossCount = 0;
        Debug.Log("Mini Boss Appeared: ");
    }

    public void DetermineEnemyToSpawn()
    {
        if (enemyCount == 1 || enemyCount == 6)
        {
            SpawnMiniBoss();
        }
        else
        {
            SpawnEnemy();
        }
    }

    public void SetCurrentEnemy(bool state)
    {
        if (enemyTime)
            enemiesObjects[enemyCount].SetActive(state);
        else if (miniBossTime)
            currentMiniBoss.SetActive(state);
    }

    public void CheckForPullBack()
    {
        if (enemyTime)
        {
            if (enemiesObjects[enemyCount].transform.position.y <= cameraBounds.y*0.25f)
            {
                triggerMoveEnemy = false;
                pullBackEnemy = true;
                oldEnemyPosition = enemiesObjects[enemyCount].transform.position;
            }
        }

        else if(miniBossTime)
        {
            if (currentMiniBoss.transform.position.y <= cameraBounds.y * 0.25f)
            {
                triggerMoveEnemy = false;
                pullBackEnemy = true;
                oldEnemyPosition = currentMiniBoss.transform.position;
            }
        }
    }
}
