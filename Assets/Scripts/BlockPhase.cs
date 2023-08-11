using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlockPhase : MonoBehaviour
{
    
    public GameObject dot1;
    public GameObject dot2;
    public GameObject dot3;
    public bool start = false;
    public bool end = false;
    public bool blockSuccess = false;

    public List<Button> dotBtnList = new List<Button>();
    public int currentBtn = 0;
    public bool blockPhase;
    public EnemySpawner spawner;
    public Canvas canvas;
    private Vector3 canvasBounds;

    // Start is called before the first frame update
    void Start()
    {
        canvasBounds = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
        Debug.Log("Canvas bounds: " + canvasBounds);
    }

    // Update is called once per frame
    void Update()
    {
        //BlockingPhase(start, end, blockSuccess);
    }
    private void BlockingPhase(bool start, bool end, bool blockSuccess){
        if (start)
        {
            Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            dot1.transform.position = randomPositionOnScreen;
            dot1.SetActive(true);
            randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            dot2.transform.position = randomPositionOnScreen;
            dot2.SetActive(true);
            randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            dot3.transform.position = randomPositionOnScreen;
            dot3.SetActive(true);
        }
        else if(end)
        {
            dot1.SetActive(false);
            dot2.SetActive(false);
            dot3.SetActive(false);
        }
        if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            //Debug.Log(raycast);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                Debug.Log(raycastHit);
                if (raycastHit.collider.CompareTag("BlockDot"))
                {
                    Debug.Log("Dot Clikced");
                }
            }
        }
    }

    public void SpawnBlockButtons()
    {
        if (blockPhase)
        {
            for (int i = 0; i < dotBtnList.Count; i++)
            {
                if (i == currentBtn)
                {
                    dotBtnList[currentBtn].GetComponent<RectTransform>().localPosition = new Vector2(Random.Range(-canvasBounds.x/2 + dotBtnList[currentBtn].GetComponent<RectTransform>().rect.width,
                        canvasBounds.x/2 - dotBtnList[currentBtn].GetComponent<RectTransform>().rect.width), Random.Range(-canvasBounds.y/2 + dotBtnList[currentBtn].GetComponent<RectTransform>().rect.height,
                        canvasBounds.y/2 - dotBtnList[currentBtn].GetComponent<RectTransform>().rect.height));
                    dotBtnList[currentBtn].gameObject.SetActive(true);
                    AnimateButton(dotBtnList[currentBtn]);
                    //Debug.Log("Button " + currentBtn + " spawned");
                }
                else if (i != currentBtn)
                {
                    dotBtnList[i].gameObject.SetActive(false);
                    //Debug.Log("Button " + i + " deactivated");
                }
            }
        }
    }

    public void OnBlockButtonPressed()
    {
        if (blockPhase)
        {
            currentBtn++;
            if (currentBtn == dotBtnList.Count)
            {
                currentBtn = 0;
                blockPhase = false;
                SetBlockButtons(false);
                spawner.pullBackEnemy = true;
                spawner.enemiesObjects[spawner.enemyCount].transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
                spawner.enemiesObjects[spawner.enemyCount].transform.GetChild(0).transform.rotation = Quaternion.identity;

                Debug.Log("3 Dots Clicked");
                return;
            }
            SpawnBlockButtons();
        }
    }

    public void SetBlockButtons(bool state)
    {
        for (int i = 0; i < dotBtnList.Count; i++)
        {
            dotBtnList[i].gameObject.SetActive(state);
            Debug.Log("Button " + i + " " + state);
        }
    }

    private void AnimateButton(Button button)
    {
        button.transform.DOComplete();
        button.transform.DOScale(1.2f, 0.4f).OnComplete(() =>
        {
            button.transform.DOScale(1f, 0.4f);
        });
    }
}
