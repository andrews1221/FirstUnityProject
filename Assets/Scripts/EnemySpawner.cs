using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{ 
    //public bool trigger;
    public bool triggerMoveEnemy, pullBackEnemy;
    public int numberOfEnemies, enemyCount = 0;
    public float distanceBetweenEnemyAndPlayer;
    public ProgressController progressController;
    public BlockPhase blockphaseController;
    public GameObject currentMiniBoss;
    [HideInInspector] public GameObject[] enemiesObjects;
    [HideInInspector] public GameObject boss;
    [HideInInspector] public Vector2 oldEnemyPosition, startingPosition;
    [HideInInspector] public bool enemyTime, miniBossTime, bossTime;
    [HideInInspector] public int miniBossCount;
    //public int numberOfSpawnPoints;
    [SerializeField] private GameObject enemyPrefab, miniBoss1Prefab, miniBoss2Prefab, bossPrefab;
    [SerializeField] private Player player;
    [SerializeField] private GameObject[] spawnersList;
    private GameObject miniBoss1, miniBoss2;
    private Vector2 enemiesPoolPosition = new Vector2(40, 40);
    private Vector2 cameraBounds, finalPosition;
    private EnemyScript enemyScript;
    private ArrowDirection arrowScript;
    private int countEncountersWithPlayer;
    private float pullBackOffset;

    // Start is called before the first frame update
    void Awake()
    {
        cameraBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        startingPosition = new Vector2(0, cameraBounds.y - 2);//offset may be mended
        finalPosition = new Vector2(0, -cameraBounds.y + 2);
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
        boss = (GameObject)Instantiate(bossPrefab, enemiesPoolPosition, Quaternion.identity);
        miniBoss1.SetActive(false);
        miniBoss2.SetActive(false);
        boss.SetActive(false);

        progressController.SetProgressBarMaxValue(numberOfEnemies);
    }

    private void Start()
    {
        SetPullbackOffset(1);
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

    public void MoveEnemy()
    {
        if (enemyTime)
            MoveBasicEnemy();
        else if (miniBossTime)
            MoveMiniBoss();
        else if (bossTime)
            MoveBoss();
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
            enemyScript.AttackPlayer(enemyScript.attack, 3f);
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
            enemyScript.AttackPlayer(enemyScript.attack, 4f);
            triggerMoveEnemy = false;
            countEncountersWithPlayer++;
        }
        distanceBetweenEnemyAndPlayer = Vector2.Distance(currentMiniBoss.transform.position, finalPosition);
        arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
        enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
    }

    private void MoveBoss()
    {
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, enemyScript.targetPosition, Time.deltaTime * enemyScript.speed);
        //blockphaseController.blockPhase = false;
        if ((Vector2)boss.transform.position == startingPosition)
        {
            enemyScript.targetPosition = finalPosition;
        }
        else if ((Vector2)boss.transform.position == finalPosition)
        {
            blockphaseController.blockPhase = true;
            blockphaseController.SpawnBlockButtons();
            enemyScript.AttackPlayer(enemyScript.attack, 6f);
            triggerMoveEnemy = false;
            countEncountersWithPlayer++;
        }
        distanceBetweenEnemyAndPlayer = Vector2.Distance(boss.transform.position, finalPosition);
        arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
        enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
    }

    public void CountEnemies()
    {
        enemyCount++;
        if (enemyCount > numberOfEnemies)
        {
            enemyCount = 0;
        }
    }

    private void PullBackEnemy()
    {
        if (enemyTime)
        {
            enemiesObjects[enemyCount].transform.position = Vector3.MoveTowards(enemiesObjects[enemyCount].transform.position, new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + pullBackOffset), Time.deltaTime * enemyScript.speed);
            if ((Vector2)enemiesObjects[enemyCount].transform.position == new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + pullBackOffset))
            {
                enemyScript.speed = 0;
                Invoke("InvokePullBackResult", 0.3f);
            }
            distanceBetweenEnemyAndPlayer = Vector2.Distance(enemiesObjects[enemyCount].transform.position, finalPosition);
            arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
            enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
        }
        else if (miniBossTime)
        {
            currentMiniBoss.transform.position = Vector3.MoveTowards(currentMiniBoss.transform.position, new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + pullBackOffset), Time.deltaTime * enemyScript.speed);
            if ((Vector2)currentMiniBoss.transform.position == new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + pullBackOffset))
            {
                enemyScript.speed = 0;
                Invoke("InvokePullBackResult", 0.3f);
            }
            distanceBetweenEnemyAndPlayer = Vector2.Distance(currentMiniBoss.transform.position, finalPosition);
            arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
            enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
        }
        else if (bossTime)
        {
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + pullBackOffset), Time.deltaTime * enemyScript.speed);
            if ((Vector2)boss.transform.position == new Vector2(oldEnemyPosition.x, oldEnemyPosition.y + pullBackOffset))
            {
                enemyScript.speed = 0;
                Invoke("InvokePullBackResult", 0.3f);
            }
            distanceBetweenEnemyAndPlayer = Vector2.Distance(boss.transform.position, finalPosition);
            arrowScript.SetArrowColor(distanceBetweenEnemyAndPlayer);
            enemyScript.SetEnemyScale(distanceBetweenEnemyAndPlayer);
        }
    }

    private void InvokePullBackResult()
    {
        triggerMoveEnemy = true;
        pullBackEnemy = false;
        blockphaseController.blockPhase = false; //since swipe is detected instantly when blockphase is false after Boss combo completed
        enemyScript.speed = enemyScript.oldSpeed;
        SetPullbackOffset(2);
        //Debug.Log("Speed After Pullback: " + enemyScript.speed);
    }

    public void DetermineEnemySpeed(EnemyScript enemy)
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
                enemy.oldSpeed = enemy.initialSpeed * (1 + i);
                Debug.Log("Speed Increased " + enemy.oldSpeed);
                break;
            }
        }
    }

    public void SpawnEnemy()
    {
        enemiesObjects[enemyCount].SetActive(true);
        enemiesObjects[enemyCount].transform.position = spawnersList[Random.Range(0, 2)].transform.position;
        enemyScript = enemiesObjects[enemyCount].transform.GetChild(0).GetComponent<EnemyScript>();
        enemyScript.initialSpeed = 2;
        enemyScript.oldSpeed = enemyScript.initialSpeed;
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
        miniBossTime = false;
        bossTime = false;
        //Debug.Log("Enemy Count: " + enemyCount);
    }

    public void SpawnMiniBoss()
    {
        if (miniBossCount == 0)
        {
            //miniBoss1.SetActive(true);
            currentMiniBoss = miniBoss1;
            currentMiniBoss.SetActive(true);
            //Debug.Log("Mini Boss: " + currentMiniBoss);
        }
        else if(miniBossCount == 1)
        {
            //miniBoss2.SetActive(true);
            currentMiniBoss = miniBoss2;
            currentMiniBoss.SetActive(true);
        }
        currentMiniBoss.transform.position = spawnersList[Random.Range(0, 2)].transform.position;
        enemyScript = currentMiniBoss.transform.GetChild(0).GetComponent<EnemyScript>();

        //Invoke function is used since it helps to set health at a max value:
        Invoke("SetEnemyHealth", 0.3f);
        DetermineEnemySpeed(enemyScript);
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
        enemyTime = false;
        bossTime = false;
        countEncountersWithPlayer = 0;
        miniBossCount++;
        if (miniBossCount >= 2)
            miniBossCount = 0;

        //Prepare block buttons since the code takes time to execute
        //BlockPhase.instance.PrepareMiniBossBlockButtons();
    }

    public void SpawnBoss()
    {
        boss.SetActive(true);
        boss.transform.position = spawnersList[Random.Range(0, 2)].transform.position;
        enemyScript = boss.transform.GetChild(0).GetComponent<EnemyScript>();
        enemyScript.initialSpeed = 2;

        //Invoke function is used since it helps to set health at a max value:
        Invoke("SetEnemyHealth", 0.5f);
        Debug.Log("Health " + enemyScript.health);
        DetermineEnemySpeed(enemyScript);
        player.DeterminePlayerAttackBasedOnMaterial(player.material, enemyScript.material);
        arrowScript = boss.transform.GetChild(0).GetChild(0).GetComponent<ArrowDirection>();
        arrowScript.ChooseArrowDirection(enemyScript.arrowDirection);

        enemyScript.targetPosition = startingPosition;
        arrowScript.maxDistanceBetweenEnemyAndPlayer = Vector2.Distance(boss.transform.position, finalPosition);

        triggerMoveEnemy = true;
        bossTime = true;
        enemyTime = false;
        miniBossTime = false;
    }

    public void DetermineEnemyToSpawn()
    {
        Debug.Log("Enemy Count: " + enemyCount);
        if (enemyCount == 3 || enemyCount == 6)
        {
            SpawnMiniBoss();
            Debug.Log("Mini Boss Appeared: ");
        }
        else if (enemyCount == 1)
        {
            SpawnBoss();
            Debug.Log("Boss Appeared: ");
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
        else if (bossTime)
            boss.SetActive(state);
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
                enemyScript.speed = enemyScript.initialSpeed;
                Debug.Log("Speed While Pullback " + enemyScript.speed);
            }
        }

        else if(miniBossTime)
        {
            if (currentMiniBoss.transform.position.y <= cameraBounds.y * 0.25f && countEncountersWithPlayer > 1)
            {
                triggerMoveEnemy = false;
                pullBackEnemy = true;
                oldEnemyPosition = currentMiniBoss.transform.position;
                enemyScript.speed = enemyScript.initialSpeed;
            }
        }

        else if (bossTime)
        {
            if (boss.transform.position.y <= cameraBounds.y * 0.25f && countEncountersWithPlayer > 1)
            {
                triggerMoveEnemy = false;
                pullBackEnemy = true;
                oldEnemyPosition = boss.transform.position;
                enemyScript.speed = enemyScript.initialSpeed;
            }
        }
    }

    private void SetEnemyHealth()
    {
        enemyScript.health = enemyScript.maxHealth;
        enemyScript.UpdateHealthBar();
    }

    //Each type of enemy has different distance to travel back
    public void SetPullbackOffset(float value)
    {
        pullBackOffset = value;
    }
}
