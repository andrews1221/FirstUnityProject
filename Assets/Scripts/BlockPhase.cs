using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BlockPhase : MonoBehaviour
{
    public static BlockPhase instance;
    public List<Button> enemyBlockBtnList = new List<Button>();
    public List<Button> miniBossBlockBtnList = new List<Button>();
    public int currentBtn = 0;
    public bool blockPhase;
    public EnemySpawner spawner;
    public Canvas canvas;
    public Blade blade;

    [HideInInspector] public int countComboCircles;
    [SerializeField] private GameObject circleHolder, zigzagHolder, crossHolder;
    [SerializeField] private Player player;
    [SerializeField] Button bossButton;
    private Vector3 canvasBounds;
    private Vector2 temporaryPos;
    private RectTransform rectTran;
    private GameObject activeHolder;
    private bool animateButton, checkPos;
    private List<int> intMiniBoss = new List<int>{ 1, 2, 3, 4, 5}; // 0 is used to substitute already output integers
    private List<int> sequenceNumMiniBoss = new List<int>();
    private List<Vector2> spawnedButtonsPositions = new List<Vector2>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        canvasBounds = new Vector2(canvas.GetComponent<RectTransform>().rect.width, canvas.GetComponent<RectTransform>().rect.height);
        Debug.Log("Canvas bounds: " + canvasBounds);
    }
    

    public void SpawnBlockButtons()
    {
        if (spawner.enemyTime)
            SpawnEnemyBlockButtons();
        else if (spawner.miniBossTime)
            SpawnMiniBossBlockButtons();
        else if (spawner.bossTime)
        {
            GenerateRandomBossBlock();
        }
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
                spawner.SetPullbackOffset(3);
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
            spawner.SetPullbackOffset(3);
            spawner.currentMiniBoss.transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
            spawner.currentMiniBoss.transform.GetChild(0).transform.rotation = Quaternion.identity;

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
            //Debug.Log("Button " + i + " " + state);
        }
        currentBtn = 0;
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
        //button.transform.DOComplete();
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
            int number = DistributeNumbers();
            miniBossBlockBtnList[i].transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
            miniBossBlockBtnList[i].transform.GetChild(0).gameObject.SetActive(false);
            if (number == 1)//(miniBossBlockBtnList[i].transform.GetChild(0).GetComponent<TMP_Text>().text == "1")
            {
                miniBossBlockBtnList[i].enabled = true;
                miniBossBlockBtnList[i].transform.GetChild(0).gameObject.SetActive(true);
                StartCoroutine(AnimateButtonCoroutine(miniBossBlockBtnList[i]));
            }
        }
        intMiniBoss = new List<int> { 1, 2, 3, 4, 5 };
    }

    public void ResetMiniBossBlockButtons()
    {
        StopAllCoroutines();
        for (int i = 0; i < miniBossBlockBtnList.Count; i++)
        {
            miniBossBlockBtnList[i].gameObject.SetActive(false);
            miniBossBlockBtnList[i].enabled = false;
            miniBossBlockBtnList[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void PrepareMiniBossBlockButtons()
    {
        for (int i = 0; i < miniBossBlockBtnList.Count; i++)
        {
            int number = DistributeNumbers();
            miniBossBlockBtnList[i].transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
            miniBossBlockBtnList[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        intMiniBoss = new List<int> { 1, 2, 3, 4, 5 };
    }

    private IEnumerator SpawnBossComboCoroutine()
    {
        int countCircles = 0;
        activeHolder = circleHolder;
        blade.gameObject.SetActive(true);
        blade.enabled = true;
        blade.GetComponent<BossCombo>().bossCombo = true;
        Vector2 camPos = Camera.main.ScreenToWorldPoint(canvasBounds);
        while (blockPhase && countComboCircles < circleHolder.transform.childCount)
        {
            if (countComboCircles == circleHolder.transform.childCount-1)
            {
                //blockPhase = false;
                blade.GetComponent<BossCombo>().bossCombo = false;
                player.checkSwipe = false;
                spawner.pullBackEnemy = true;
                spawner.SetPullbackOffset(3);
                spawner.boss.transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
                spawner.boss.transform.GetChild(0).transform.rotation = Quaternion.identity;
                RestoreBossCombo();
                StopAllCoroutines();
                Debug.Log("Combo Completed");
                break;
            }
            circleHolder.transform.GetChild(countCircles).gameObject.SetActive(true);
            CircleCollider2D collider = circleHolder.transform.GetChild(countCircles).GetComponent<CircleCollider2D>();
            float circleScale = circleHolder.transform.GetChild(countCircles).transform.localScale.x;
            circleHolder.transform.GetChild(countCircles).transform.position = new Vector2(Random.Range(-camPos.x
                + 2 * collider.radius * circleScale, camPos.x - 2 * collider.radius * circleScale), Camera.main.ScreenToWorldPoint(canvasBounds).y + 2 * collider.radius * circleScale);
            circleHolder.transform.GetChild(countCircles).GetComponent<Rigidbody2D>().gravityScale = 0.5f;
            countCircles++;
            if(countCircles == circleHolder.transform.childCount-1)
                countCircles = 0;
            yield return new WaitForSeconds(0.5f);
        }
        countCircles = 0;
        countComboCircles = 0;
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

    private IEnumerator SpawnBossZigzagCoroutine()
    {
        int countCircles = 0;
        activeHolder = zigzagHolder;
        blade.gameObject.SetActive(true);
        blade.enabled = true;
        blade.GetComponent<BossCombo>().bossZigzag = true;
        zigzagHolder.SetActive(true);
        while (blockPhase && countCircles < zigzagHolder.transform.childCount)
        {
            zigzagHolder.transform.GetChild(countCircles).gameObject.SetActive(true);
            zigzagHolder.transform.GetChild(countCircles).GetComponent<CircleCollider2D>().enabled = true;
            countCircles++;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void CheckRespawnZigzagCoroutine()
    {
        if (countComboCircles == zigzagHolder.transform.childCount || countComboCircles == 2*zigzagHolder.transform.childCount)
        {
            Debug.Log("Zigzag Respawned");
            //StopCoroutine(SpawnBossZigzagCoroutine());
            blade.transform.position = new Vector2(-10, 0);
            StartCoroutine(SpawnBossZigzagCoroutine());
        }
        else if (countComboCircles == 3 * zigzagHolder.transform.childCount)
        {
            //blockPhase = false;
            countComboCircles = 0;
            player.checkSwipe = false;
            spawner.pullBackEnemy = true;
            spawner.SetPullbackOffset(3);
            spawner.boss.transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
            spawner.boss.transform.GetChild(0).transform.rotation = Quaternion.identity;
            blade.transform.position = new Vector2(-10, 0);
            blade.enabled = false;
            blade.gameObject.SetActive(false);
            Debug.Log("Zigzag Completed");
        }
    }

    private IEnumerator SpawnBossCrossCoroutine()
    {
        int countCircles = 0;
        activeHolder = crossHolder;
        blade.gameObject.SetActive(true);
        blade.enabled = true;
        blade.GetComponent<BossCombo>().bossCross = true;
        crossHolder.SetActive(true);
        while (blockPhase && countCircles < crossHolder.transform.childCount)
        {
            crossHolder.transform.GetChild(countCircles).gameObject.SetActive(true);
            crossHolder.transform.GetChild(countCircles).GetComponent<CircleCollider2D>().enabled = true;
            countCircles++;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void CheckRespawnCrossCoroutine()
    {
        if (countComboCircles == crossHolder.transform.childCount || countComboCircles == 2 * crossHolder.transform.childCount)
        {
            Debug.Log("Cross Respawned");
            //StopCoroutine(SpawnBossZigzagCoroutine());
            blade.transform.position = new Vector2(-10, 0);
            StartCoroutine(SpawnBossCrossCoroutine());
        }
        else if (countComboCircles == 3 * crossHolder.transform.childCount)
        {
            //blockPhase = false;
            countComboCircles = 0;
            player.checkSwipe = false;
            spawner.pullBackEnemy = true;
            spawner.boss.transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
            spawner.boss.transform.GetChild(0).transform.rotation = Quaternion.identity;
            blade.transform.position = new Vector2(-10, 0);
            blade.enabled = false;
            blade.gameObject.SetActive(false);
            Debug.Log("Cross Completed");
        }
    }

    public void OnBossButton()
    {
        Vector2 oldScale = bossButton.transform.localScale;
        bossButton.transform.localScale = new Vector2(oldScale.x + 5*Time.deltaTime, oldScale.y + 5*Time.deltaTime);
        if (bossButton.transform.localScale.x >= 1)
        {
            bossButton.gameObject.SetActive(false);
            StopCoroutine(SpawnBossButton());
            player.checkSwipe = false;
            spawner.pullBackEnemy = true;
            spawner.boss.transform.GetChild(0).GetComponent<EnemyScript>().StopRotationAnimation();//EnemyScript is attached to a child
            spawner.boss.transform.GetChild(0).transform.rotation = Quaternion.identity;
            Debug.Log("BossButton Completed");
        }
    }

    private IEnumerator SpawnBossButton()
    {
        activeHolder = null;
        bossButton.gameObject.SetActive(true);
        bossButton.transform.localScale = new Vector2(1, 1);
        while (blockPhase && bossButton.transform.localScale.x > 0.2)
        {
            yield return new WaitForEndOfFrame();
            Vector2 oldScale = bossButton.transform.localScale;
            bossButton.transform.localScale = new Vector2(oldScale.x - 0.5f * Time.deltaTime, oldScale.y - 0.5f * Time.deltaTime);
        }
    }

    private void GenerateRandomBossBlock()
    {
        int randInt = Random.Range(0, 4);
        switch (randInt)
        {
            case 0:
                StartCoroutine(SpawnBossComboCoroutine());
                Debug.Log("Boss Combo");
                break;
            case 1:
                StartCoroutine(SpawnBossZigzagCoroutine());
                Debug.Log("Boss Zig Zag");
                break;
            case 2:
                StartCoroutine(SpawnBossCrossCoroutine());
                Debug.Log("Boss Cross");
                break;
            case 3:
                StartCoroutine(SpawnBossButton());
                Debug.Log("Boss Button");
                break;
        }
    }

    public void RestoreBossCombo()
    {
        if (activeHolder == circleHolder || activeHolder == zigzagHolder || activeHolder == crossHolder)
        {
            StopAllCoroutines();
            for (int i = 0; i < activeHolder.transform.childCount; i++)
            {
                if (activeHolder == circleHolder)
                {
                    activeHolder.transform.GetChild(i).GetComponent<Rigidbody2D>().gravityScale = 0f;
                    activeHolder.transform.GetChild(i).transform.localPosition = Vector2.zero;
                }
                else if (activeHolder == zigzagHolder || activeHolder == crossHolder)
                {
                    activeHolder.transform.GetChild(i).gameObject.SetActive(false);
                    countComboCircles = 0;
                }

            }
            blade.gameObject.SetActive(false);
            blade.enabled = false;
            Debug.Log("Boss Combos Restored");
        }

        else
        {
            bossButton.gameObject.SetActive(false);
        }
    }
}
