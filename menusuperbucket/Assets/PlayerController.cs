using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Horizontal")]
    [SerializeField] float maxSpeed = 10;
    [SerializeField] float acceleration = 10;
    [SerializeField] float groundFriction = 0.2f;
    [SerializeField] float onAirReduction = 0.3f;

    [Header("Verical")]
    [SerializeField] float jumpForce = 10;
    [SerializeField] float gravityMultiplier = 3;
    [SerializeField] LayerMask groundLayer;

    Rigidbody2D rb;
    bool grounded;

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

    void Update()
    {
        //To avoid computing it multiple times
        grounded = isGrounded();

        horizontalMovement();

        verticalMovement();
    }

    void horizontalMovement()
    {
        if(grounded)
        {
            //If the player is grounded we check the movement normally
            if (hInput != 0)
            {
                speed += hInput * acceleration * Time.deltaTime;
            }
            else
            {
                //If not pressing any key, decelerate the player
                speed = speed * Mathf.Pow(groundFriction, 1 / Time.deltaTime);
            }
        }
        else
        {
            //Reduce the movemnt capacity in the air
            if (hInput != 0)
            {
                speed += hInput * acceleration * Time.deltaTime * onAirReduction;
            }
        }
    }

    void verticalMovement()
    {

        if(Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.velocity = speed * Vector2.right + jumpForce * Vector2.up;
        }


        //If space if kept pressed, the gravity is smaller, so the jump goes higher (only when going up)
        if(Input.GetKey(KeyCode.Space) && rb.velocity.y >= 0)
        {
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = gravityMultiplier;
        }
    }

    bool isGrounded()
    {
        //Casting a ray downwards
        if (Physics2D.Raycast(transform.position, Vector2.down, GetComponent<BoxCollider2D>().size.y / 2 + 0.2f, groundLayer))
            return true;
        return false;
    }
}
