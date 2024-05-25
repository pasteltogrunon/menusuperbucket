using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingFlyingEnemyAI : MonoBehaviour, IPushable
{

    [SerializeField] float timeStunned = 0.4f;
    [SerializeField] float timePushed = 0.1f;
    [SerializeField] float receivedKnockback = 5;
    [SerializeField] float drag = 3;

    [Header("Wandering")]
    [SerializeField] float speed = 2f;
    [SerializeField] float wanderingRadius = 5f;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 7f;
    [SerializeField] float speedWhenDected = 3;
    [SerializeField] float detectionInterval = 0.3f;
    [SerializeField] LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField] float attackRadius = 4f;
    [SerializeField] float minRadius = 3f;
    [SerializeField] float attackCooldown = 4;
    [SerializeField] float shotSpeed = 4;
    [SerializeField] GameObject projectile;

    State state = State.Wandering;

    Transform spottedEnemy;
    Vector2 randomPosition;
    Vector2 velDirection;
    Vector2 startPosition;

    //Time being pushed and time being unable to do things
    float stunTime = 0;
    float pushTime = 0;

    float attackTimer = 0;

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
        if (pushTime > 0)
        {
            //If pushed we do not countdown stun
            pushTime -= Time.deltaTime;
            return;
        }

        if (stunTime > 0)
        {
            GetComponent<Rigidbody2D>().velocity -= drag * GetComponent<Rigidbody2D>().velocity * Time.deltaTime;
            stunTime -= Time.deltaTime;
            return;
        }


        switch (state)
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
        if(Vector2.Distance(transform.position, randomPosition) < 0.5f )
        {
            randomPosition = Random.insideUnitCircle * wanderingRadius + startPosition;
            velDirection = ((Vector2)transform.position - randomPosition).normalized;


            direction = Mathf.FloorToInt(Mathf.Sign(velDirection.x));
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }

        GetComponent<Rigidbody2D>().velocity = velDirection * speed;
    }

    void followEnemy()
    {
        if (attackCooldown > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (Mathf.Sign(spottedEnemy.position.x - transform.position.x) == direction)
        {
            float distance = (spottedEnemy.position - transform.position).magnitude;
            if (distance > minRadius && distance < attackRadius)
            {
                GetComponent<Rigidbody2D>().velocity -= drag * GetComponent<Rigidbody2D>().velocity * Time.deltaTime;
                if(attackTimer <= 0)
                {
                    Vector2 dir = new Vector2(spottedEnemy.position.x - transform.position.x, spottedEnemy.position.y - transform.position.y + 1.5f).normalized;

                    Instantiate(projectile, transform.position + new Vector3(dir.x, dir.y, 0), Quaternion.identity)
                        .GetComponent<Rigidbody2D>().velocity = dir * shotSpeed;

                    attackTimer = attackCooldown;
                }
            }
            else if(distance <= minRadius)
            {
                GetComponent<Rigidbody2D>().velocity = -speedWhenDected * (spottedEnemy.position - transform.position).normalized;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = speedWhenDected * (spottedEnemy.position - transform.position).normalized;
            }
        }
        else
        {
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }

    }


    IEnumerator checkForPlayer()
    {
        while (state == State.Wandering)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
            if (hit != null)
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
