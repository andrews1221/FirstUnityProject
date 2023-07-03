using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private float holdStart;

    public int playerAttack = 1;
    public int playerHealth = 10;
    public bool BLOCKING = false;
    public bool detectSwipeOnlyAfterRelease = true;
    public int SWIPE_DEIRECTION = 0;
    public float SWIPE_THRESHOLD = 20f;
    public float HOLD_THRESHOLD = 0.3f;

    // Update is called once per frame
    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUp = touch.position;
                fingerDown = touch.position;
                holdStart = Time.time;
            }

            if ((touch.phase == TouchPhase.Stationary)&&(HOLD_THRESHOLD < Time.time - holdStart))
            {
                BLOCKING = true;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved)
            {
                if (!detectSwipeOnlyAfterRelease)
                {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                CheckSwipe();
                BLOCKING = false;
            }
        }
        //Debug.Log(BLOCKING);
    }

    private void CheckSwipe()
    {
        if(BLOCKING ==false)
        {
            //Check if Vertical swipe
            if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
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
            }

            //Check if Horizontal swipe
            else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
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
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    private float HorizontalValMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    //////////////////////////////////CALLBACK FUNCTIONS/////////////////////////////
    void OnSwipeUp()
    {
        Debug.Log("Swipe UP");
        SWIPE_DEIRECTION = 1;
    }

    void OnSwipeDown()
    {
        Debug.Log("Swipe Down");
        SWIPE_DEIRECTION = 2;
    }

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
        SWIPE_DEIRECTION = 3;
    }

    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
        SWIPE_DEIRECTION = 4;
    }
}
