using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public Vector3 direction;

    private Camera mainCamera;

    private Collider2D sliceCollider;
    private TrailRenderer sliceTrail;
    private bool slicing;
    [SerializeField] private Player player;

    public float sliceForce = 5f;
    public float minSliceVelocity = 0.01f;
    public BlockPhase blockPhaseController;
    public EnemySpawner spawner;

    private void Awake()
    {
        mainCamera = Camera.main;
        //sliceCollider = GetComponent<Collider2D>();
        //sliceTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable()
    {
        StopSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void Update()
    {
        if (blockPhaseController.blockPhase && spawner.bossTime && !player.checkSwipe)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    StartSlice(touch);
                }

                else if (touch.phase == TouchPhase.Moved)
                {
                    ContinueSlice(touch);
                }

                //else if (touch.phase == TouchPhase.Ended)
                //{
                //    StopSlice();
                //}
            }
        }
    }

    private void StartSlice(Touch touch)
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(touch.position);
        position.z = 0f;
        transform.position = position;

        slicing = true;
        //sliceCollider.enabled = true;
        //sliceTrail.enabled = true;
        //sliceTrail.Clear();
    }

    private void StopSlice()
    {
        slicing = false;
        //sliceCollider.enabled = false;
        //sliceTrail.enabled = false;
    }

    private void ContinueSlice(Touch touch)
    {
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(touch.position);
        newPosition.z = 0f;

        transform.position = newPosition;
        direction = newPosition - transform.position;

        //float velocity = direction.magnitude / Time.deltaTime;
        //sliceCollider.enabled = velocity > minSliceVelocity;
    }
}