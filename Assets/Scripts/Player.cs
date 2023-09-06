using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour
{ 
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private float holdStart;
    private int initialPlayerAttack;
    private Camera cam;

    public int playerAttack;
    public int playerHealth;

    public bool detectSwipeOnlyAfterRelease = true, checkSwipe = false;
    public bool fire, water, earth, metal, wood;
    public string material;
    public int swipeDirection = 0;
    public float swipeThreshold = 20f;
    public float holdThreshold = 0.3f;
    public EnemySpawner spawner;
    public float swordRotationOriginalZ;
    public Image swordImg;
    public GameObject bossSwiper;

    //public GameObject bossSwiperPrefab;

    private void Awake()
    {
        cam = Camera.main;
        initialPlayerAttack = playerAttack;
        Debug.Log("Initial Attack: " + initialPlayerAttack);
    }

    // Update is called once per frame
    void Update()
    {
        TouchSwipeDetection();
    }

    private void TouchSwipeDetection()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                //if (spawner.blockphaseController.blockPhase)
                //{
                //    bossSwiper.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 1));
                //}
                fingerUp = touch.position;
                fingerDown = touch.position;
                holdStart = Time.time;
            }

            /*if ((touch.phase == TouchPhase.Stationary) && (holdThreshold < Time.time - holdStart))
            {
                blocking = true;
            }*/

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    //if (spawner.blockphaseController.blockPhase)
                    //{
                    //    bossSwiper.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 1));
                    //}
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                //if (spawner.blockphaseController.blockPhase)
                //{
                //    bossSwiper.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 1));
                //}
                fingerDown = touch.position;
                CheckSwipe();
            }
        }
    }

    private void CheckSwipe()
    {
        if (!BlockPhase.instance.blockPhase)
        {
            if (VerticalMove() > swipeThreshold && VerticalMove() > HorizontalValMove())
            {
                //Debug.Log("Vertical");
                if (fingerDown.y - fingerUp.y > 0)//up swipe
                {
                    OnSwipeUp();
                }
                else if (fingerDown.y - fingerUp.y < 0)//Down swipe
                {
                    OnSwipeDown();
                }
                fingerUp = fingerDown;
                checkSwipe = true;
            }

            //Check if Horizontal swipe
            else if (HorizontalValMove() > swipeThreshold && HorizontalValMove() > VerticalMove())
            {
                //Debug.Log("Horizontal");
                if (fingerDown.x - fingerUp.x > 0)//Right swipe
                {
                    OnSwipeRight();
                }
                else if (fingerDown.x - fingerUp.x < 0)//Left swipe
                {
                    OnSwipeLeft();
                }
                fingerUp = fingerDown;
                checkSwipe = true;
            }

            //No Movement at-all
            else
            {
                //Debug.Log("No Swipe!");
            }
        }
    }

    private float VerticalMove()
    {
        Debug.Log("Vertical move " + Mathf.Abs(fingerDown.x - fingerUp.x));
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    private float HorizontalValMove()
    {
        Debug.Log("Horizontal move " + Mathf.Abs(fingerDown.x - fingerUp.x));
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        Debug.Log("Swipe UP");
        swipeDirection = 1;
        MoveSword(-15f);
    }

    void OnSwipeDown()
    {
        Debug.Log("Swipe Down");
        swipeDirection = 2;
        MoveSword(15f);
    }

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
        swipeDirection = 3;
        MoveSword(15f);
    }

    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
        swipeDirection = 4;
        MoveSword(-15f);
    }

    public void RecieveDamage(int damage)
    {
        playerHealth -= damage;
        cam.transform.DOComplete();
        cam.transform.DOShakePosition(1f);
    }

    public void CheckPlayerHealth(EnemySpawner enemySpawner)
    {
        if (playerHealth <= 0)
        {
            enemySpawner.triggerMoveEnemy = false;
            enemySpawner.pullBackEnemy = false;
            spawner.SetCurrentEnemy(false);
        }
    }

    public void DeterminePlayerAttackBasedOnMaterial(string playerMaterial, string enemyMaterial)
    {
        switch (playerMaterial)
        {
            case "fire":
                switch (enemyMaterial)
                {
                    case "fire":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1);
                        break;
                    case "metal":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.5f);
                        break;
                    case "wood":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.25f);
                        break;
                    case "earth":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.75f);
                        break;
                    case "water":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.5f);
                        break;
                }
                break;
            case "metal":
                switch (enemyMaterial)
                {
                    case "fire":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.5f);
                        break;
                    case "metal":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1);
                        break;
                    case "wood":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.5f);
                        break;
                    case "earth":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.25f);
                        break;
                    case "water":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.75f);
                        break;
                }
                break;
            case "wood":
                switch (enemyMaterial)
                {
                    case "fire":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.75f);
                        break;
                    case "metal":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.5f);
                        break;
                    case "wood":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1f);
                        break;
                    case "earth":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.5f);
                        break;
                    case "water":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.25f);
                        break;
                }
                break;
            case "earth":
                switch (enemyMaterial)
                {
                    case "fire":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.25f);
                        break;
                    case "metal":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.75f);
                        break;
                    case "wood":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.5f);
                        break;
                    case "earth":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1f);
                        break;
                    case "water":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.5f);
                        break;
                }
                break;
            case "water":
                switch (enemyMaterial)
                {
                    case "fire":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.5f);
                        break;
                    case "metal":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1.25f);
                        break;
                    case "wood":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.75f);
                        break;
                    case "earth":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 0.5f);
                        break;
                    case "water":
                        playerAttack = Mathf.RoundToInt(initialPlayerAttack * 1f);
                        break;
                }
                break;
        }

        Debug.Log(playerMaterial + " => " + enemyMaterial + ": " + playerAttack);
    }

    private void MoveSword(float rotate)
    {
        swordImg.transform.DOComplete();
        swordImg.transform.DORotate(new Vector3(swordImg.transform.rotation.x, swordImg.transform.rotation.y, swordRotationOriginalZ + rotate), 0.5f).OnComplete(() =>
        {
            swordImg.transform.DORotate(new Vector3(swordImg.transform.rotation.x, swordImg.transform.rotation.y, swordRotationOriginalZ), 0.5f);
        });
    }

    private void BossSwipe()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                bossSwiper.transform.position = touch.position;
            }

            /*if ((touch.phase == TouchPhase.Stationary) && (holdThreshold < Time.time - holdStart))
            {
                blocking = true;
            }*/

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    bossSwiper.transform.position = touch.position;
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                bossSwiper.transform.position = touch.position;
            }
        }
    }
}
