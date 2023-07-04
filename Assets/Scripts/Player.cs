using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    private float holdStart;

    public float playerAttack = 1;
    public float playerHealth = 10;
    public bool blocking = false;
    public bool detectSwipeOnlyAfterRelease = true;
    public int swipeDirection = 0;
    public float swipeThreshold = 20f;
    public float holdThreshold = 0.3f;

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
                fingerUp = touch.position;
                fingerDown = touch.position;
                holdStart = Time.time;
            }

            if ((touch.phase == TouchPhase.Stationary) && (holdThreshold < Time.time - holdStart))
            {
                blocking = true;
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
                blocking = false;
            }
        }
    }

    private void CheckSwipe()
    {
        if(blocking == false)
        {
            //Check if Vertical swipe
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
        swipeDirection = 1;
    }

    void OnSwipeDown()
    {
        Debug.Log("Swipe Down");
        swipeDirection = 2;
    }

    void OnSwipeLeft()
    {
        Debug.Log("Swipe Left");
        swipeDirection = 3;
    }

    void OnSwipeRight()
    {
        Debug.Log("Swipe Right");
        swipeDirection = 4;
    }
}
