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

    public Gradient gradient;
    [HideInInspector] public float maxDistanceBetweenEnemyAndPlayer;
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private SpriteRenderer arrowSpriteRenderer;
    private int spriteNumber;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent.GetComponent<EnemyScript>().spawner.enemyCount == 0 && transform.parent.GetComponent<EnemyScript>().role == "enemy")
        {
            spriteNumber = this.transform.parent.GetComponent<EnemyScript>().arrowDirection;
            ChooseArrowDirection(spriteNumber);
            Debug.Log("First Direction Chosen");
        }
        //if(transform.parent.GetComponent<EnemyScript>().number == 0 && transform.parent.GetComponent<EnemyScript>().role == "enemy")
        //{
        //    spriteNumber = this.transform.parent.GetComponent<EnemyScript>().arrowDirection;
        //    ChooseArrowDirection(spriteNumber);
        //}
        //else
        //{
        //    ChooseArrowDirection(1);
        //}
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
                arrowTransform.rotation = Quaternion.Euler(0, 0, 90);
                Debug.Log("Arrow is UP");
                break;
            case 2:
                //spriteRenderer.sprite = down;
                arrowTransform.rotation = Quaternion.Euler(0, 0, 270);
                Debug.Log("Arrow is DOWN");
                break;
            case 3:
                //spriteRenderer.sprite = left;
                arrowTransform.rotation = Quaternion.Euler(0, 0, 180);
                Debug.Log("Arrow is LEFT");
                break;
            case 4:
                //spriteRenderer.sprite = right;
                arrowTransform.rotation = Quaternion.Euler(0, 0, 0);
                Debug.Log("Arrow is RIGHT");
                break;
            default:
                break;
        }

        //Debug.Log("Arrow Direction " + arrowTransform.rotation.z);
    }

    public void SetArrowColor(float value)
    {

        //slider.value = value;
        float valueToEval = Convert(value);

        arrowSpriteRenderer.color = gradient.Evaluate(valueToEval);
    }

    public float Convert(float distance)
    {
        //Debug.Log("Arrow value: " + distance / maxDistanceBetweenEnemyAndPlayer);
        return distance / maxDistanceBetweenEnemyAndPlayer;
    }

    public void SetTransparency(float a)
    {
        arrowSpriteRenderer.color = new Color(arrowSpriteRenderer.color.r, arrowSpriteRenderer.color.g, arrowSpriteRenderer.color.b, a);
    }
}
