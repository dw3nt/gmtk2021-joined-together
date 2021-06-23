using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private AudioClip putDownClip;

    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;
    private AudioSource audioClip;

    private bool isFacingWallCloseBy = false;

    public bool canAcceptInput = true;

    private Vector2 moveDir = Vector2.zero;
    public List<GameObject> interactableObjects = new List<GameObject>();
    private GameObject currentCarry;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioClip = GetComponent<AudioSource>();
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
        RayCastObstacleDetect();
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
            sprite.flipX = moveDir.x > 0;
        }

        if (moveDir != Vector2.zero) {
            animator.SetBool("isWalking", true);
        } else {
            animator.SetBool("isWalking", false);
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
            if (npc.canMatch) {
                float distanceTo = Vector2.Distance(transform.position, go.transform.position);
                if( distanceTo < smallestDistance) {
                    smallestDistance = distanceTo;
                    closestNpc = go;
                }
            }
        }

        if (closestNpc) {
            currentCarry = closestNpc;
            audioClip.clip = pickUpClip;
            audioClip.Play();
            MatchNpcController npc = closestNpc.GetComponent<MatchNpcController>();
            npc.CarryState(gameObject);
        }
    }

    void DropMatchNpc()
    {
        int facing = sprite.flipX ? 1 : -1;
        if(isFacingWallCloseBy) {
            facing *= -1;
        }

        MatchNpcController npc = currentCarry.GetComponent<MatchNpcController>();
        npc.DropNpc(facing);
        audioClip.clip = putDownClip;
        audioClip.Play();
        currentCarry = null;
    }

    void RayCastObstacleDetect()
    {
        int facing = sprite.flipX ? 1 : -1;
        RaycastHit2D rayHit = Physics2D.Raycast((Vector2)transform.position + Vector2.right * facing, Vector2.right * facing, 2);
        if (rayHit.collider.CompareTag("Obstacle")) {
            isFacingWallCloseBy = true;
        } else {
            isFacingWallCloseBy = false;
        }
    }

    public void DisablePlayer()
    {
        canAcceptInput = false;
        moveDir = Vector2.zero;
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
