using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BlockPhase : MonoBehaviour
{
    
    public GameObject dot1;
    public GameObject dot2;
    public GameObject dot3;
    public bool start = false;
    public bool end = false;
    public bool blockSuccess = false;

    public List<Button> enemyBlockBtnList = new List<Button>();
    public List<Button> miniBossBlockBtnList = new List<Button>();
    public int currentBtn = 0;
    public bool blockPhase;
    public EnemySpawner spawner;
    public Canvas canvas;
    private Vector3 canvasBounds;
    private Vector2 temporaryPos;
    private RectTransform rectTran;
    private bool animateButton, checkPos;
    private List<int> intMiniBoss = new List<int>{ 1, 2, 3, 4, 5}; // 0 is used to substitute already output integers
    private List<int> sequenceNumMiniBoss = new List<int>();
    private List<Vector2> spawnedButtonsPositions = new List<Vector2>();

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
        if (spawner.enemyTime)
            SpawnEnemyBlockButtons();
        else if (spawner.miniBossTime)
            SpawnMiniBossBlockButtons();
    }

    public void OnBlockButtonPressed()
    {
        if (blockPhase)
        {
            StopAllCoroutines();
            currentBtn++;
            if (currentBtn == enemyBlockBtnList.Count)
            {
                currentBtn = 0;
                blockPhase = false;
                SetBlockButtons(false, enemyBlockBtnList);
                spawner.pullBackEnemy = true;
                spawner.enemiesObjects[spawner.enemyCount].transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
                spawner.enemiesObjects[spawner.enemyCount].transform.GetChild(0).transform.rotation = Quaternion.identity;

                Debug.Log("3 Dots Clicked");
                return;
            }
            SpawnEnemyBlockButtons();
        }
    }

    public void OnMiniBossButtonPressed()
    {
        StopAllCoroutines();
        miniBossBlockBtnList[sequenceNumMiniBoss.FindIndex(a => a == currentBtn+1)].gameObject.SetActive(false);
        currentBtn++;
        if (currentBtn == miniBossBlockBtnList.Count)
        {
            currentBtn = 0;
            blockPhase = false;
            SetBlockButtons(false, miniBossBlockBtnList);
            sequenceNumMiniBoss = new List<int>();
            spawner.pullBackEnemy = true;
            spawner.enemiesObjects[spawner.enemyCount].transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
            spawner.enemiesObjects[spawner.enemyCount].transform.GetChild(0).transform.rotation = Quaternion.identity;

            Debug.Log("5 MiniBoss Buttons Clicked");
            return;
        }
        miniBossBlockBtnList[sequenceNumMiniBoss.FindIndex(a => a == currentBtn+1)].enabled = true;
        miniBossBlockBtnList[sequenceNumMiniBoss.FindIndex(a => a == currentBtn + 1)].transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(AnimateButtonCoroutine(miniBossBlockBtnList[sequenceNumMiniBoss.FindIndex(a => a == currentBtn+1)]));
    }

    public void SetBlockButtons(bool state, List<Button> buttons)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].gameObject.SetActive(state);
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

    private IEnumerator AnimateButtonCoroutine(Button button)
    {
        button.transform.DOComplete();
        button.transform.DOScale(1.2f, 0.4f).OnComplete(() =>
        {
            button.transform.DOScale(1f, 0.4f);
        });
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(AnimateButtonCoroutine(button));
    }

    private void SpawnEnemyBlockButtons()
    {
        for (int i = 0; i < enemyBlockBtnList.Count; i++)
        {
            if (i == currentBtn)
            {
                enemyBlockBtnList[currentBtn].GetComponent<RectTransform>().localPosition = new Vector2(Random.Range(-canvasBounds.x / 2 + enemyBlockBtnList[currentBtn].GetComponent<RectTransform>().rect.width,
                    canvasBounds.x / 2 - enemyBlockBtnList[currentBtn].GetComponent<RectTransform>().rect.width), Random.Range(-canvasBounds.y / 2 + enemyBlockBtnList[currentBtn].GetComponent<RectTransform>().rect.height,
                    canvasBounds.y / 2 - enemyBlockBtnList[currentBtn].GetComponent<RectTransform>().rect.height));
                enemyBlockBtnList[currentBtn].gameObject.SetActive(true);
                StartCoroutine(AnimateButtonCoroutine(enemyBlockBtnList[currentBtn]));
                //AnimateButton(dotBtnList[currentBtn]);
                //Debug.Log("Button " + currentBtn + " spawned");
            }
            else if (i != currentBtn)
            {
                enemyBlockBtnList[i].gameObject.SetActive(false);
                //Debug.Log("Button " + i + " deactivated");
            }
        }
    }

    public void SpawnMiniBossBlockButtons()
    {
        for (int i = 0; i < miniBossBlockBtnList.Count; i++)
        {
            miniBossBlockBtnList[i].gameObject.SetActive(true);
            miniBossBlockBtnList[i].enabled = false;
            //RectTransform rectTran = miniBossBlockBtnList[i].GetComponent<RectTransform>();
            //Vector2 temporaryPos = new Vector2(Random.Range(-canvasBounds.x / 2 + miniBossBlockBtnList[i].GetComponent<RectTransform>().rect.width,
            //    canvasBounds.x / 2 - miniBossBlockBtnList[i].GetComponent<RectTransform>().rect.width), Random.Range(-canvasBounds.y / 2 + miniBossBlockBtnList[i].GetComponent<RectTransform>().rect.height,
            //    canvasBounds.y / 2 - miniBossBlockBtnList[i].GetComponent<RectTransform>().rect.height));
            //rectTran.localPosition = temporaryPos;
            //GeneratePosition(i);
            int number = DistributeNumbers();
            miniBossBlockBtnList[i].transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
            miniBossBlockBtnList[i].transform.GetChild(0).gameObject.SetActive(false);
            if (number == 1)
            {
                miniBossBlockBtnList[i].enabled = true;
                miniBossBlockBtnList[i].transform.GetChild(0).gameObject.SetActive(true);
                StartCoroutine(AnimateButtonCoroutine(miniBossBlockBtnList[i]));
            }
        }
        intMiniBoss = new List<int>{ 1, 2, 3, 4, 5 };
    }

    private void GeneratePosition(int orderNum)
    {
        bool spawn = false;
        rectTran = miniBossBlockBtnList[orderNum].GetComponent<RectTransform>();
        temporaryPos = new Vector2(Random.Range(-canvasBounds.x / 2 + miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.width,
                canvasBounds.x / 2 - miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.width), Random.Range(-canvasBounds.y / 2 + miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.height,
                canvasBounds.y / 2 - miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.height));
        //Debug.Log("BEFORE FOR LOOP");
        if(spawnedButtonsPositions.Count == 0)
        {
            rectTran.localPosition = temporaryPos;
            return;
        }
        for (int j = 0; j < spawnedButtonsPositions.Count; j++)
        {
            while (Vector2.Distance(temporaryPos, spawnedButtonsPositions[j]) < 1.2*Mathf.Sqrt(Mathf.Pow(rectTran.rect.width, 2) + Mathf.Pow(rectTran.rect.height, 2)))
            {
                temporaryPos = new Vector2(Random.Range(-canvasBounds.x / 2 + miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.width,
                    canvasBounds.x / 2 - miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.width), Random.Range(-canvasBounds.y / 2 + miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.height,
                    canvasBounds.y / 2 - miniBossBlockBtnList[orderNum].GetComponent<RectTransform>().rect.height));
            }
            if(Vector2.Distance(temporaryPos, spawnedButtonsPositions[j]) > Mathf.Sqrt(Mathf.Pow(rectTran.rect.width, 2) + Mathf.Pow(rectTran.rect.height, 2)))
            {
                spawn = true;
            }
        }
        if (spawn)
        {
            spawnedButtonsPositions.Add(temporaryPos);
            rectTran.localPosition = temporaryPos;
        }
    }

    private int DistributeNumbers()
    {
        int n = Random.Range(1, 6);
        if (intMiniBoss[n - 1] == n)
        {
            intMiniBoss[n - 1] = 0;
            sequenceNumMiniBoss.Add(n);
            Debug.Log("Button number success: " + n);
            return n;
        }
        else if (intMiniBoss[n - 1] == 0)
        {
            //int elseN = intMiniBoss.Find(x => x > 0);
            //Debug.Log("Button number through Find: " + elseN);
            //intMiniBoss[n - 1] = 0;
            //return elseN;
            int elseN;
            for (int x = 0; x < intMiniBoss.Count; x++)
            {
                if (intMiniBoss[x] > 0)
                {
                    elseN = intMiniBoss[x];
                    sequenceNumMiniBoss.Add(elseN);
                    intMiniBoss[x] = 0;
                    Debug.Log("Button number through Find: " + elseN);
                    return elseN;
                }
                else
                    continue;
            }
        }

        Debug.Log("Distribution Failed");
        return 0;
    }
}
