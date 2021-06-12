using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    private Rigidbody2D body;
    private SpriteRenderer sprite;

    public bool canAcceptInput = true;

    private Vector2 moveDir = Vector2.zero;
    public List<GameObject> interactableObjects = new List<GameObject>();
    private GameObject currentCarry;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (canAcceptInput) {
            GetMoveInput();
            HandleInteractInput();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetMoveInput()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();
    }

    void Move()
    {
        if (moveDir.x != 0) {
            sprite.flipX = moveDir.x < 0;
        }

        body.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }

    void HandleInteractInput()
    {
        if (Input.GetButtonDown("Interact") && currentCarry == null) {
            CarryMatchNpc();
        } else if (Input.GetButtonDown("Interact") && currentCarry != null) {
            DropMatchNpc();
        }
    }

    void CarryMatchNpc()
    {
        GameObject closestNpc = null;
        float smallestDistance = 100f;
        foreach ( GameObject go in interactableObjects) {
            MatchNpcController npc = go.GetComponent<MatchNpcController>();
            if (npc.canMatch) {     // can only carry those not matched...?
                float distanceTo = Vector2.Distance(transform.position, go.transform.position);
                if( distanceTo < smallestDistance) {
                    smallestDistance = distanceTo;
                    closestNpc = go;
                }
            }
        }

        if (closestNpc) {
            currentCarry = closestNpc;
            MatchNpcController npc = closestNpc.GetComponent<MatchNpcController>();
            npc.CarryState(gameObject);
        }
    }

    void DropMatchNpc()
    {
        MatchNpcController npc = currentCarry.GetComponent<MatchNpcController>();
        npc.DropNpc(sprite.flipX ? -1 : 1);
        currentCarry = null;
    }

    void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.CompareTag("MatchNpc") && !interactableObjects.Contains(collider.gameObject)) {
            interactableObjects.Add(collider.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("MatchNpc")) {
            interactableObjects.Remove(collider.gameObject);
        }
    }
}
