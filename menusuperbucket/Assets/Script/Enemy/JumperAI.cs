using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperAI : MonoBehaviour, IPushable
{

    [SerializeField] float speed = 2f;

    [SerializeField] float timeStunned = 0.4f;
    [SerializeField] float timePushed = 0.1f;
    [SerializeField] float receivedKnockback = 5;

    [Header("Raycasts")]
    [SerializeField] float raycastOffset = 1.3f;
    [SerializeField] float raycastDown = 1.3f;
    [SerializeField] LayerMask groundLayer;

    [Header("Detection and Jump")]
    [SerializeField] float detectionRadius = 7f;
    [SerializeField] float speedWhenDected = 3;
    [SerializeField] float jumpRadius = 3f;
    [SerializeField] float jumpSpeed = 3;
    [SerializeField] float detectionInterval = 0.3f;
    [SerializeField] LayerMask playerLayer;


    State state = State.Wandering;

    Transform spottedEnemy;

    //Time being pushed and time being unable to do things
    float stunTime = 0;
    float pushTime = 0;

    int direction = -1;

    void Awake()
    {
        direction = -Mathf.FloorToInt(transform.localScale.x);
        state = State.Wandering;
    }

    void Start()
    {
        StartCoroutine(checkForPlayer());
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

        
        switch(state)
        {
            case State.Wandering:
                wander();
                break;
            case State.EnemySpotted:
                followEnemy();
                break;
            default:
                break;
        }
        
    }

    void wander()
    {
        if (canMoveForward)
        {
            GetComponent<Rigidbody2D>().velocity = direction * speed * transform.right + GetComponent<Rigidbody2D>().velocity.y * Vector3.up;
        }
        else
        {
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }
    }

    void followEnemy()
    {
        if(Mathf.Sign(spottedEnemy.position.x - transform.position.x) == direction)
        {
            if (canMoveForward)
            {
                GetComponent<Rigidbody2D>().velocity = direction * speedWhenDected * transform.right + GetComponent<Rigidbody2D>().velocity.y * Vector3.up;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.y * Vector3.up;
            }
        }
        else
        {
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }

        if(Vector2.Distance(transform.position, spottedEnemy.position) <= jumpRadius)
        {
            Vector2 dir = new Vector2(spottedEnemy.position.x - transform.position.x, spottedEnemy.position.y - transform.position.y).normalized;
            GetComponent<Rigidbody2D>().velocity = dir * jumpSpeed + 2*Vector2.up;
            pushTime = 1f;
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

    IEnumerator checkForPlayer()
    {
        while(state == State.Wandering)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            if(hit != null)
            {
                spottedEnemy = hit.transform;
                state = State.EnemySpotted;
                yield break;
            }
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    public void push(Vector2 pushDirection)
    {
        GetComponent<Rigidbody2D>().velocity = pushDirection * receivedKnockback;
        stunTime = timeStunned;
        pushTime = timePushed;
    }

    enum State
    {
        EnemySpotted,
        Wandering
    }
}
