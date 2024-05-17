using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float maxSpeed = 10;
    [SerializeField] float acceleration = 10;
    [SerializeField] float groundFriction = 0.2f;

    Rigidbody2D rb;

    float speed
    {
        get => rb.velocity.x;
        set
        {
            rb.velocity = Mathf.Clamp(value, -maxSpeed, maxSpeed) * Vector2.right + rb.velocity.y * Vector2.up;
        }
    }

    float hInput
    {
        get => (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
    }

    //Referencias a uno mismo
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //Referencias a otros objetos
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(hInput != 0)
        {
            speed += hInput * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            speed = speed * Mathf.Pow(groundFriction, 1/Time.fixedDeltaTime);
        }
    }
}
