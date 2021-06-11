using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchNpcController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minActionTime = 0.25f;
    [SerializeField] private float maxActionTime = 1.25f;
    [SerializeField] private float carryOffset = 0.85f;
    [SerializeField] private float dropOffset = 1.25f;

    enum State { Wander, Carried, Matched }

    private Rigidbody2D body;

    private State state;

    private Vector2 moveDir = Vector2.zero;
    private float wanderTimer;
    private GameObject holderTarget;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        state = State.Wander;
        wanderTimer = Random.Range(minActionTime, maxActionTime);
    }

    void Update()
    {
        switch (state) {
            case State.Wander:
                WanderUpdate();
                break;
            case State.Carried:
                CarriedUpdate();
                break;
        }
    }

    void FixedUpdate()
    {
        switch (state) {
            case State.Wander:
                WanderFixedUpdate();
                break;
        }
    }

    void WanderUpdate()
    {   
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f) {
            if (Random.Range(0.0f, 1.0f) > 0.5f) {
                // play walk animation

                float xDir = Random.Range(-1f, 1f);
                float yDir = Random.Range(-1f, 1f);
                moveDir = new Vector2(xDir, yDir).normalized;
            } else {
                // play idle animation
                
                moveDir = Vector2.zero;
            }

            wanderTimer = Random.Range(minActionTime, maxActionTime);
        }
    }

    void CarriedUpdate()
    {
        transform.position = holderTarget.transform.position + (Vector3.up * carryOffset);
    }

    void WanderFixedUpdate()
    {
        body.velocity = moveDir * moveSpeed;
    }

    public void CarryState(GameObject go)
    {
        // play carry animation
        holderTarget = go;
        state = State.Carried;
    }

    public void WanderState(int facing)
    {
        state = State.Wander;
        transform.position = holderTarget.transform.position + (Vector3.right * dropOffset * facing);
        holderTarget = null;
    }
}