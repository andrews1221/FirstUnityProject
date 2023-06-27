using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(0,0,0);
    private float speed = 10f;
    public int attack = 1;
    public int health = 10;
    public int ARROW_DIRECTION;
    private Player player;
    private EnemySpawner spawner;
    // Start is called before the first frame update
    void Start()
    {
        player =  GameObject.Find("Player").GetComponent<Player>();
        spawner = GameObject.Find("SpawnController").GetComponent<EnemySpawner>();
        ARROW_DIRECTION = Random.Range(1,5);
        //Debug.Log(playerControls.GetComponent<ControlsScript>().SWIPE_DEIRECTION);
    }

    // Update is called once per frame
    void Update()
    {
        spawner.trigger = false;
        if (health<1)
        {
            spawner.trigger = true;
            Destroy(transform.parent.gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime*speed);
        if (player.SWIPE_DEIRECTION == ARROW_DIRECTION)
        {
            //Debug.Log("Damage Dealt");

            player.SWIPE_DEIRECTION = 0;
            ARROW_DIRECTION = Random.Range(1,5);
            health -= player.playerAttack;
            return;
        }
        else if (player.SWIPE_DEIRECTION!=0)
        {
            //Debug.Log("Damage Recieved");

            player.SWIPE_DEIRECTION = 0;
            ARROW_DIRECTION = Random.Range(1,5);
            player.playerHealth -= attack;
        }
    }
}
