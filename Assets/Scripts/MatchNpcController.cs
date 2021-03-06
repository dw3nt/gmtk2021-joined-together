using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchNpcController : MonoBehaviour
{
    enum AnimationType { Bounce, Roll }
    enum State { Wander, Carried }

    [SerializeField] public string hobby;
    [SerializeField] private AnimationType animType;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minActionTime = 0.25f;
    [SerializeField] private float maxActionTime = 1.25f;
    [SerializeField] private float carryOffset = 0.85f;
    [SerializeField] private float dropOffset = 1.25f;
    [SerializeField] private Transform matchWallPrefab;

    private Rigidbody2D body;
    private CircleCollider2D matchCollider;
    private BoxCollider2D collisionCollider;

    private State state;

    private Vector2 moveDir = Vector2.zero;
    private float wanderTimer;
    private GameObject holderTarget;
    private GameObject mate;
    private bool squishBounce = true;

    public bool canMatch = true;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        matchCollider = GetComponent<CircleCollider2D>();
        collisionCollider = GetComponent<BoxCollider2D>();

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

                // if matched, bias movement towards each other a bit

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
        switch (animType) {
            case AnimationType.Bounce:
                BounceAnimation();
                break;
            case AnimationType.Roll:
                RollAnimation();
                break;
        }

        body.velocity = moveDir * moveSpeed;
    }

    void BounceAnimation()
    {
        Vector3 squishScale = new Vector3(1.3f, 0.7f, 1);
        Vector3 stretchScale = new Vector3(0.7f, 1.3f, 1);

        if (moveDir != Vector2.zero) {
            if (squishBounce) {
                transform.localScale = Vector3.Lerp(transform.localScale, squishScale, 0.1f);
                if (transform.localScale.x > 1.25f) {
                    squishBounce = false;
                }
            } else {
                transform.localScale = Vector3.Lerp(transform.localScale, stretchScale, 0.1f);
                if (transform.localScale.y > 1.25f) {
                    squishBounce = true;
                }
            }
        } else {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 0.2f);
        }
    }

    void RollAnimation()
    {
        if (moveDir != Vector2.zero) {
            int rollDir = moveDir.x > 0 ? -1 : 1;
            Quaternion rotateAmount = Quaternion.AngleAxis(90 * rollDir, Vector3.forward);
            Quaternion targetRotation = transform.rotation * rotateAmount;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 2 * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (canMatch && collider.gameObject.CompareTag("MatchNpc")) {
            MatchNpcController otherController = collider.gameObject.GetComponent<MatchNpcController>();
            if (otherController.canMatch && hobby == otherController.hobby) {
                otherController.SetMatchNpc(gameObject);
                SetMatchNpc(collider.gameObject);
                CreateMatchWall(gameObject, collider.gameObject);
                GameManager.instance.AddPoints(1);
            }
        }
    }

    void CreateMatchWall(GameObject go1, GameObject go2)
    {
        Transform matchWall = Instantiate(matchWallPrefab);
        MatchWallController controller = matchWall.GetComponent<MatchWallController>();
        controller.match1 = go1;
        controller.match2 = go2;
    }

    public void CarryState(GameObject go)
    {
        // play carry animation
        holderTarget = go;
        matchCollider.enabled = false;
        collisionCollider.enabled = false;
        canMatch = false;
        state = State.Carried;
    }

    public void DropNpc(int facing)
    {
        transform.position = holderTarget.transform.position + (Vector3.right * dropOffset * facing);
        holderTarget = null;
        WanderState(facing);
    }

    public void WanderState(int facing)
    {
        matchCollider.enabled = true;
        collisionCollider.enabled = true;
        canMatch = true;
        state = State.Wander;
    }

    public void SetMatchNpc(GameObject go) 
    {
        mate = go;
        matchCollider.enabled = false;
        canMatch = false;
    }
}
