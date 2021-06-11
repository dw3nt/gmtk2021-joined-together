using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    private Rigidbody2D body;

    private Vector2 moveDir = Vector2.zero;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetInputs();
    }

    void FixedUpdate()
    {
        Move();
    }

    void GetInputs()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();
    }

    void Move()
    {
        body.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }
}
