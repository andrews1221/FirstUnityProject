using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(0,0,0);
    private float speed = 10f;
    public int attack = 1;
    public int health = 10;
    public int arrowDirection;
    private Player player;
    private EnemySpawner spawner;
    // Start is called before the first frame update
    void Start()
    {
        player =  GameObject.Find("Player").GetComponent<Player>();
        spawner = GameObject.Find("SpawnController").GetComponent<EnemySpawner>();
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
            transform.parent.gameObject.SetActive(false);
            spawner.SpawnEnemy();
            this.transform.GetChild(0).GetComponent<EnemyScript>().enabled = false;
            return;
        }

        transform.GetComponentInParent<Transform>().position = Vector3.MoveTowards(transform.GetComponentInParent<Transform>().position, targetPosition, Time.deltaTime*speed);
        if (player.swipeDirection == arrowDirection)
        {
            //Debug.Log("Damage Dealt");

            player.swipeDirection = 0;
            arrowDirection = Random.Range(1,5);
            this.transform.GetChild(0).GetComponent<ArrowDirection>().ChooseArrowDirection(arrowDirection);
            health -= player.playerAttack;
            return;
        }
        else if (player.swipeDirection != 0)
        {
            //Debug.Log("Damage Recieved");

            player.swipeDirection = 0;
            arrowDirection = Random.Range(1,5);
            this.transform.GetChild(0).GetComponent<ArrowDirection>().ChooseArrowDirection(arrowDirection);
            player.playerHealth -= attack;
        }
    }
}
