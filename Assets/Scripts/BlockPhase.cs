using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPhase : MonoBehaviour
{
    
    public GameObject dot1;
    public GameObject dot2;
    public GameObject dot3;
    public bool start = false;
    public bool end = false;
    public bool blockSuccess = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BlockingPhase(start, end, blockSuccess);
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
}
