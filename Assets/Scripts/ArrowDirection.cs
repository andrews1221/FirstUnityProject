using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDirection : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;
    private int spriteNumber;
    // Start is called before the first frame update
    void Start()
    {
        spriteNumber = GameObject.Find("EnemySprite").GetComponent<EnemyScript>().ARROW_DIRECTION;
        switch(spriteNumber){
            case 1:
                spriteRenderer.sprite = up;
                break;
            case 2:
                spriteRenderer.sprite = down;
                break;
            case 3:
                spriteRenderer.sprite = left;
                break;
            case 4:
                spriteRenderer.sprite = right;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        spriteNumber = GetComponentInParent<EnemyScript>().ARROW_DIRECTION;
        switch(spriteNumber){
            case 1:
                spriteRenderer.sprite = up;
                break;
            case 2:
                spriteRenderer.sprite = down;
                break;
            case 3:
                spriteRenderer.sprite = left;
                break;
            case 4:
                spriteRenderer.sprite = right;
                break;
            default:
                break;
        }
    }
}
