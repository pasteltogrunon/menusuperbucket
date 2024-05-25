using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenacuajoAI : MonoBehaviour, IPushable
{
    [SerializeField] float speed = 2f;
    [SerializeField] float timeStunned = 0.4f;
    [SerializeField] float timePushed = 0.1f;
    [SerializeField] float raycastOffset = 1.3f;
    [SerializeField] float raycastDown = 1.3f;

    [SerializeField] float receivedKnockback = 5;

    [SerializeField] LayerMask groundLayer;

    //Time being pushed and time being unable to do things
    float stunTime = 0;
    float pushTime = 0;

    int direction = -1;

    void Awake()
    {
        direction = -Mathf.FloorToInt(transform.localScale.x);
    }

    void Update()
    {
        if (!grounded) return;

        if (pushTime > 0)
        {
            //If pushed we do not countdown stun
            pushTime -= Time.deltaTime;
            return;
        }

        if (stunTime > 0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * GetComponent<Rigidbody2D>().velocity.y;
            stunTime -= Time.deltaTime;
            return;
        }

        

        if(canMoveForward)
        {
            GetComponent<Rigidbody2D>().velocity = direction * speed * transform.right + GetComponent<Rigidbody2D>().velocity.y * Vector3.up;
        }
        else
        {
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }
    }

    bool canMoveForward
    {
        get => !Physics2D.Raycast(transform.position, direction * transform.right, raycastOffset, groundLayer) &&
            Physics2D.Raycast(transform.position + direction * raycastOffset * transform.right, Vector2.down, raycastDown, groundLayer);
    }

    bool grounded
    {
        get => Physics2D.Raycast(transform.position, Vector2.down, raycastDown, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + direction * raycastOffset * transform.right, transform.position + direction * raycastOffset * transform.right + Vector3.down * raycastDown);
        Gizmos.DrawLine(transform.position , transform.position + direction * raycastOffset * transform.right);
    }

    public void push(Vector2 pushDirection)
    {
        GetComponent<Rigidbody2D>().velocity = pushDirection * 5;
        stunTime = timeStunned;
        pushTime = timePushed;
    }
}
