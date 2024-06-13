using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundStandardAI : MonoBehaviour, IPushable
{

    [SerializeField] float timeStunned = 0.4f;
    [SerializeField] float timePushed = 0.1f;
    [SerializeField] float receivedKnockback = 5;


    [Header("Wander")]
    [SerializeField] float wanderSpeed = 2f;
    [SerializeField] float raycastOffset = 1.3f;
    [SerializeField] float raycastDown = 1.3f;
    [SerializeField] LayerMask groundLayer;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 7f;
    [SerializeField] float minRadius = 0;
    [SerializeField] protected float speedWhenDected = 3;
    [SerializeField] float detectionInterval = 0.3f;
    [SerializeField] LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField] float attackRadius = 3f;
    [SerializeField] float attackCooldown = 3f;
    [SerializeField] protected float attackSpeed = 3;


    State state = State.Wandering;

    Transform spottedEnemy;

    Animator animator;

    protected Vector2 enemyDirection
    {
        get => new Vector2(spottedEnemy.position.x - transform.position.x, spottedEnemy.position.y - transform.position.y);
    }

    //Time being pushed and time being unable to do things
    protected float stunTime = 0;
    protected float pushTime = 0;

    float attackTimer = 0;

    int direction = -1;

    void Awake()
    {
        direction = -Mathf.FloorToInt(transform.localScale.x);
        state = State.Wandering;

        TryGetComponent(out animator);

    }

    void Start()
    {
        StartCoroutine(checkForPlayer());
    }

    private void OnEnable()
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

            animator?.SetFloat("Speed", 0);

            return;
        }

        if (stunTime > 0)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * GetComponent<Rigidbody2D>().velocity.y;
            stunTime -= Time.deltaTime;

            animator?.SetFloat("Speed", 0);

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
       

        animator?.SetFloat("Speed", Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
        
    }

    void wander()
    {
        if (canMoveForward)
        {
            GetComponent<Rigidbody2D>().velocity = direction * wanderSpeed * transform.right + GetComponent<Rigidbody2D>().velocity.y * Vector3.up;
        }
        else
        {
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }
    }

    void followEnemy()
    {
        if(attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if(Mathf.Sign(spottedEnemy.position.x - transform.position.x) == direction)
        {
            //Stay in the zone between attackRadius and minRadius
            if(enemyDirection.magnitude > attackRadius)
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
            else if(enemyDirection.magnitude < minRadius)
            {
                if (canMoveBackwards)
                {
                    GetComponent<Rigidbody2D>().velocity = -direction * speedWhenDected * transform.right + GetComponent<Rigidbody2D>().velocity.y * Vector3.up;
                }
                else
                {
                    GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.y * Vector3.up;
                }
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity -= new Vector2(2 * GetComponent<Rigidbody2D>().velocity.x * Time.deltaTime, 0);
                //If in the zone, attack
                if (attackTimer <= 0)
                {
                    attack();
                    attackTimer = attackCooldown;
                }
            }
        }
        else
        {
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }
    }

    protected virtual void attack()
    {
        pushTime = 1f;
        GetComponent<Rigidbody2D>().velocity = attackSpeed * enemyDirection.normalized + 2 * Vector2.up;
    }

    bool canMoveForward
    {
        get => !Physics2D.Raycast(transform.position, direction * transform.right, raycastOffset, groundLayer) &&
            Physics2D.Raycast(transform.position + direction * raycastOffset * transform.right, Vector2.down, raycastDown, groundLayer);
    }
    bool canMoveBackwards
    {
        get => !Physics2D.Raycast(transform.position, -direction * transform.right, raycastOffset, groundLayer) &&
            Physics2D.Raycast(transform.position - direction * raycastOffset * transform.right, Vector2.down, raycastDown, groundLayer);
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
        stunTime = Mathf.Max(timeStunned, stunTime);
        pushTime = Mathf.Max(timePushed, pushTime);
    }

    enum State
    {
        EnemySpotted,
        Wandering
    }
}
