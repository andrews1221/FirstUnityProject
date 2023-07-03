using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    //public SpriteRenderer spriteRenderer;
    //[SerializeField] private Sprite up;
    //[SerializeField] private Sprite down;
    //[SerializeField] private Sprite left;
    //[SerializeField] private Sprite right;
    [SerializeField] private Transform arrowTransform;
    private int spriteNumber;
    // Start is called before the first frame update
    void Start()
    {
        spriteNumber = GameObject.Find("EnemySprite").GetComponent<EnemyScript>().arrowDirection;
        ChooseArrowDirection(spriteNumber);
    }

    // Update is called once per frame
    void Update()
    {
        //spriteNumber = GetComponentInParent<EnemyScript>().arrowDirection;
        //switch(spriteNumber){
        //    case 1:
        //        spriteRenderer.sprite = up;
        //        break;
        //    case 2:
        //        spriteRenderer.sprite = down;
        //        break;
        //    case 3:
        //        spriteRenderer.sprite = left;
        //        break;
        //    case 4:
        //        spriteRenderer.sprite = right;
        //        break;
        //    default:
        //        break;
        //}
    }

    public void ChooseArrowDirection(int number)
    {
        switch (number)
        {
            case 1:
                //spriteRenderer.sprite = up;
                arrowTransform.rotation = new Quaternion(0, 0, 90, 0);
                break;
            case 2:
                //spriteRenderer.sprite = down;
                arrowTransform.rotation = new Quaternion(0, 0, 270, 0);
                break;
            case 3:
                //spriteRenderer.sprite = left;
                arrowTransform.rotation = new Quaternion(0, 0, 180, 0);
                break;
            case 4:
                //spriteRenderer.sprite = right;
                arrowTransform.rotation = new Quaternion(0, 0, 0, 0);
                break;
            default:
                break;
        }
    }
}
