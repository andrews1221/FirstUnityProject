using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyScript : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(0,0,0), initialSize, finalSize;
    public float speed;
    public string material;
    public string role;//can be "enemy", "miniBoss", "boss".
    [HideInInspector] public float initialSpeed;
    public bool fire, water, earth, metal, wood;
    public int attack = 1;
    public int health = 10, arrowDirection, number, maxHealth;
    private Player player;
    private EnemySpawner spawner;
    private ArrowDirection arrowScript;
    // Start is called before the first frame update
    void Start()
    {
        player =  GameObject.Find("Player").GetComponent<Player>();
        spawner = GameObject.Find("SpawnController").GetComponent<EnemySpawner>();
        arrowScript = this.transform.GetChild(0).GetComponent<ArrowDirection>();
        //arrowDirection = Random.Range(1,5);
        maxHealth = health;
        initialSpeed = speed;
        Debug.Log("Initial speed " + initialSpeed);
        //Debug.Log(playerControls.GetComponent<ControlsScript>().SWIPE_DEIRECTION);
    }

    // Update is called once per frame
    void Update()
    {
        //spawner.trigger = false;

        //transform.GetComponentInParent<Transform>().position = Vector3.MoveTowards(transform.GetComponentInParent<Transform>().position, targetPosition, Time.deltaTime*speed);
        if (player.swipeDirection == arrowDirection && !spawner.blockphaseController.blockPhase)
        {
            player.swipeDirection = 0;
            player.checkSwipe = false;
            health -= DetermineDamageToEnemy();//(int)Mathf.Round(player.playerAttack*arrowScript.Convert(spawner.distanceBetweenEnemyAndPlayer)*10);
            CheckEnemyHealth();
            spawner.CheckForPullBack();
            //spawner.oldEnemyPosition = spawner.enemiesObjects[spawner.enemyCount].transform.position;

            Debug.Log("Player: Damage Dealt");
            return;
        }
        else if (player.swipeDirection != arrowDirection && player.checkSwipe && !spawner.blockphaseController.blockPhase)
        {
            Debug.Log("Swipe Direction: " + player.swipeDirection + " and Arrow Direction: " + arrowDirection);
            player.swipeDirection = 0;
            player.checkSwipe = false;
            arrowDirection = Random.Range(1, 5);
            arrowScript.ChooseArrowDirection(arrowDirection);
            //player.playerHealth -= attack;
            player.RecieveDamage(attack);

            Debug.Log("Player: Damage Recieved - Wrong Swipe; Block Phase: " + spawner.blockphaseController.blockPhase);
        }
    }

    private int DetermineDamageToEnemy()
    {
        Debug.Log("Player Attack: " + player.playerAttack);
        float valueToCompare = arrowScript.Convert(spawner.distanceBetweenEnemyAndPlayer);
        if (valueToCompare <= 1 && valueToCompare > 0.7f)
        {
            return player.playerAttack;
        }
        else if (valueToCompare <= 0.7 && valueToCompare > 0.5f)
        {
            return Mathf.RoundToInt(player.playerAttack * 0.75f);
        }
        else if (valueToCompare <= 0.5 && valueToCompare > 0.2f)
        {
            return Mathf.RoundToInt(player.playerAttack * 0.5f);
        }
        else
        {
            return Mathf.RoundToInt(player.playerAttack * 0.25f);
        }
    }

    public void AttackPlayer(int damage)
    {
        Camera.main.transform.DOComplete();
        //Camera.main.transform.DOShakePosition(1f, new Vector3(1f, 0f, 0f), 10, 0);
        //this.transform.DOScale(1.2f, 0.5f).OnComplete(() => {
        //    this.transform.DOScale(1f, 0.5f);
        //    spawner.pullBackEnemy = true;
        //});
        this.transform.DORotate(new Vector3(0, 60f, 0), 5f).OnComplete(() => {
            this.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
            if(spawner.blockphaseController.blockPhase)
            {
                player.playerHealth -= damage;
                spawner.pullBackEnemy = true;
                if(spawner.enemyTime)
                    spawner.blockphaseController.SetBlockButtons(false, spawner.blockphaseController.enemyBlockBtnList);
                else if (spawner.miniBossTime)
                    spawner.blockphaseController.SetBlockButtons(false, spawner.blockphaseController.miniBossBlockBtnList);
                spawner.blockphaseController.blockPhase = false;
                spawner.blockphaseController.currentBtn = 0;
                player.CheckPlayerHealth(spawner);
            }
        });
    }

    private void CheckEnemyHealth()
    {
        if (health < 1)
        {
            Debug.Log("Enemy Killed");
            //spawner.trigger = true;
            //Destroy(transform.parent.gameObject);
            spawner.SetCurrentEnemy(false);
            health = maxHealth;
            spawner.triggerMoveEnemy = false;
            if (spawner.enemyTime)
            {
                spawner.progressController.UpdateProgressBar();
            }
            spawner.CountEnemies();
            spawner.DetermineEnemyToSpawn();
            GameDataManager.instance.AddCoins(1);
            //this.transform.GetChild(0).GetComponent<EnemyScript>().enabled = false;
        }
        else
        {
            arrowDirection = Random.Range(1, 5);
            arrowScript.ChooseArrowDirection(arrowDirection);
            Debug.Log("New Direction From Enemy Script: " + arrowDirection);
        }
    }

    public void SetEnemyScale(float distance)
    {
        float sizeToAdd = Vector3.Distance(finalSize, initialSize)*(1f-distance/arrowScript.maxDistanceBetweenEnemyAndPlayer);
        this.transform.localScale = new Vector2(initialSize.x+sizeToAdd, initialSize.y+sizeToAdd);
    }

    public void StopRotationAnimation()
    {
        this.transform.DOKill();
    }

    
}
