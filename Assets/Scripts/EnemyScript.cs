using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(0,0,0), initialSize, finalSize;
    public float initialSpeed;
    public string material;
    public string role;//can be "enemy", "miniBoss", "boss".
    [HideInInspector] public float speed, oldSpeed;
    [HideInInspector] public EnemySpawner spawner;
    public bool fire, water, earth, metal, wood;
    public int attack = 1;
    public int health, arrowDirection, number, maxHealth;
    public Slider slider;
    [SerializeField] SpriteRenderer enemySpriteRenderer;
    private Player player;
    private ArrowDirection arrowScript;
    // Start is called before the first frame update
    void Start()
    {
        player =  GameObject.Find("Player").GetComponent<Player>();
        spawner = GameObject.Find("SpawnController").GetComponent<EnemySpawner>();
        arrowScript = this.transform.GetChild(0).GetComponent<ArrowDirection>();
        //arrowDirection = Random.Range(1,5);
        health = maxHealth;
        slider.maxValue = maxHealth;
        //oldSpeed = initialSpeed;
        //Debug.Log("Health " + health);
        //Debug.Log(playerControls.GetComponent<ControlsScript>().SWIPE_DEIRECTION);
    }

    private void OnEnable()
    {
        health = maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        //spawner.trigger = false;

        //transform.GetComponentInParent<Transform>().position = Vector3.MoveTowards(transform.GetComponentInParent<Transform>().position, targetPosition, Time.deltaTime*speed);
        if (player.swipeDirection == arrowDirection && !spawner.blockphaseController.blockPhase)
        {
            GetDamageAnimation();
            player.swipeDirection = 0;
            player.checkSwipe = false;
            health -= DetermineDamageToEnemy();//(int)Mathf.Round(player.playerAttack*arrowScript.Convert(spawner.distanceBetweenEnemyAndPlayer)*10);
            CheckEnemyHealth();
            spawner.CheckForPullBack();
            //spawner.oldEnemyPosition = spawner.enemiesObjects[spawner.enemyCount].transform.position;

            Debug.Log("Player: Damage Dealt; Block Phase: " + spawner.blockphaseController.blockPhase);
            return;
        }
        else if (player.swipeDirection != arrowDirection && player.checkSwipe && !BlockPhase.instance.blockPhase)
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

    public void AttackPlayer(int damage, float attackTime)
    {
        {
            //this.transform.DOComplete();
            //Camera.main.transform.DOShakePosition(1f, new Vector3(1f, 0f, 0f), 10, 0);
            //this.transform.DOScale(1.2f, 0.5f).OnComplete(() => {
            //    this.transform.DOScale(1f, 0.5f);
            //    spawner.pullBackEnemy = true;
            //});
            //this.transform.DORotate(new Vector3(0, 60f, 0), 5f).OnComplete(() => {
            //    this.transform.DORotate(new Vector3(0, 0, 0), 0.5f);
            this.transform.DORotate(new Vector3(0, 60, 0), attackTime).OnComplete(() => {

                if (spawner.blockphaseController.blockPhase)
                {
                    if (spawner.enemyTime)
                    {
                        BlockPhase.instance.SetBlockButtons(false, BlockPhase.instance.enemyBlockBtnList);
                    }
                    else if (spawner.miniBossTime)
                    {
                        BlockPhase.instance.ResetMiniBossBlockButtons();
                    }
                    else if (spawner.bossTime)
                    {
                        BlockPhase.instance.RestoreBossCombo();
                    }
                }
                this.transform.DORotate(new Vector3(0, 0, 0), 2f).OnComplete(() =>
                {
                    player.RecieveDamage(damage);
                    spawner.pullBackEnemy = true;
                    BlockPhase.instance.blockPhase = false;
                    player.CheckPlayerHealth(spawner);
                });
                
            });
        }
    }
    private void CheckEnemyHealth()
    {
        if (health < 1)
        {
            Debug.Log("Enemy Killed");
            //spawner.trigger = true;
            //Destroy(transform.parent.gameObject);
            health = maxHealth;
            spawner.triggerMoveEnemy = false;
            spawner.SetCurrentEnemy(false);
            spawner.CountEnemies();
            spawner.progressController.UpdateProgressBar();
            spawner.DetermineEnemyToSpawn();
            //Invoke("InvokeDetermineEnemyToSpawn", 2*Time.deltaTime);
            GameDataManager.instance.AddCoins(1);
            //this.transform.GetChild(0).GetComponent<EnemyScript>().enabled = false;
        }
        else
        {
            UpdateHealthBar();
            arrowDirection = Random.Range(1, 5);
            arrowScript.ChooseArrowDirection(arrowDirection);
            Debug.Log("New Direction From Enemy Script: " + arrowDirection);
        }
    }

    private void InvokeDetermineEnemyToSpawn()
    {
        spawner.DetermineEnemyToSpawn();
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

    private void GetDamageAnimation()
    {
        this.transform.DOKill();
        this.transform.DOShakePosition(0.5f, 0.1f);
    }

    public void UpdateHealthBar()
    {
        slider.value = health;
    }
    
}
