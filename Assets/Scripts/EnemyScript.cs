using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(0,0,0);
    public float speed = 10f;
    public float attack = 1;
    public float health = 10;
    public int arrowDirection;
    private Player player;
    private EnemySpawner spawner;
    private ArrowDirection arrowScript;
    // Start is called before the first frame update
    void Start()
    {
        player =  GameObject.Find("Player").GetComponent<Player>();
        spawner = GameObject.Find("SpawnController").GetComponent<EnemySpawner>();
        arrowScript = this.transform.GetChild(0).GetComponent<ArrowDirection>();
        arrowDirection = Random.Range(1,5);
        //Debug.Log(playerControls.GetComponent<ControlsScript>().SWIPE_DEIRECTION);
    }

    // Update is called once per frame
    void Update()
    {
        //spawner.trigger = false;
        if (health<1)
        {
            //spawner.trigger = true;
            //Destroy(transform.parent.gameObject);
            spawner.enemiesObjects[spawner.enemyCount].SetActive(false);
            spawner.triggerMoveEnemy = false;
            spawner.CountEnemies();
            spawner.SpawnEnemy();
            //this.transform.GetChild(0).GetComponent<EnemyScript>().enabled = false;

            Debug.Log("Enemy Killed");
        }

        //transform.GetComponentInParent<Transform>().position = Vector3.MoveTowards(transform.GetComponentInParent<Transform>().position, targetPosition, Time.deltaTime*speed);
        if (player.swipeDirection == arrowDirection)
        {
            player.swipeDirection = 0;
            arrowDirection = Random.Range(1,5);
            this.transform.GetChild(0).GetComponent<ArrowDirection>().ChooseArrowDirection(arrowDirection);
            health -= player.playerAttack*arrowScript.Convert(spawner.distanceBetweenEnemyAndPlayer);
            spawner.triggerMoveEnemy = false;
            spawner.pullBackEnemy = true;

            Debug.Log("Damage Dealt");
        }
        else if (player.swipeDirection != 0)
        {
            player.swipeDirection = 0;
            arrowDirection = Random.Range(1,5);
            this.transform.GetChild(0).GetComponent<ArrowDirection>().ChooseArrowDirection(arrowDirection);
            player.playerHealth -= attack;

            Debug.Log("Damage Recieved");
        }
    }
}
