using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossCombo : MonoBehaviour
{
    public float circleScaleMultiplier;
    public bool bossCombo, bossZigzag, bossCross;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Circle")
        {
            Debug.Log("Boss Trigger System");
            if (bossCombo)
            {
                BlockPhase.instance.countComboCircles++;
                float initialScale = collision.transform.localScale.x;
                collision.transform.DOComplete();
                collision.transform.DOScale(initialScale * circleScaleMultiplier, 0.2f).OnComplete(() =>
                {
                    collision.transform.DOScale(initialScale, 0.2f).OnComplete(() =>
                    {
                        collision.gameObject.SetActive(false);
                    });
                });
            }

            else if (bossZigzag)
            {
                Debug.Log("Boss Zigzag");
                BlockPhase.instance.countComboCircles++;
                float initialScale = collision.transform.localScale.x;
                collision.GetComponent<CircleCollider2D>().enabled = false;
                collision.transform.DOComplete();
                collision.transform.DOScale(initialScale * circleScaleMultiplier, 0.2f).OnComplete(() =>
                {
                    collision.transform.DOScale(initialScale, 0.2f).OnComplete(() =>
                    {
                        collision.gameObject.SetActive(false);
                        BlockPhase.instance.CheckRespawnZigzagCoroutine();
                    });
                });
            }

            else if (bossCross)
            {
                Debug.Log("Boss Cross");
                BlockPhase.instance.countComboCircles++;
                float initialScale = collision.transform.localScale.x;
                collision.GetComponent<CircleCollider2D>().enabled = false;
                collision.transform.DOComplete();
                collision.transform.DOScale(initialScale * circleScaleMultiplier, 0.2f).OnComplete(() =>
                {
                    collision.transform.DOScale(initialScale, 0.2f).OnComplete(() =>
                    {
                        collision.gameObject.SetActive(false);
                        BlockPhase.instance.CheckRespawnCrossCoroutine();
                    });
                });
            }
        }
        //if (collision.tag == "Blade")
        //{
        //    float initialScale = this.transform.localScale.x;
        //    this.transform.DOComplete();
        //    this.transform.DOScale(initialScale * circleScaleMultiplier, 0.2f).OnComplete(() => {
        //        this.transform.DOScale(initialScale, 0.2f).OnComplete(() => {
        //            BlockPhase.instance.countComboCircles++;
        //            this.gameObject.SetActive(false);
        //        });
        //    });
        //}
    }
}
