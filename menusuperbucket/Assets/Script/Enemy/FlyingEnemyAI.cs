using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour, IPushable
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
    [SerializeField] float minRadius = 2f;
    [SerializeField] LayerMask playerLayer;

    [Header("Attack")]
    [SerializeField] float attackRadius = 3f;
    [SerializeField] float attackSpeed = 10;
    [SerializeField] float attackStunTime = 2.5f;
    [SerializeField] float attackCooldown = 2.5f;

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
        if (Mathf.Sign(spottedEnemy.position.x - transform.position.x) == direction)
        {
            float distance = (spottedEnemy.position - transform.position).magnitude;
            if (distance > minRadius && distance < attackRadius)
            {
                GetComponent<Rigidbody2D>().velocity -= drag * GetComponent<Rigidbody2D>().velocity * Time.deltaTime;
                if (attackTimer <= 0)
                {
                    Vector2 dir = new Vector2(spottedEnemy.position.x - transform.position.x, spottedEnemy.position.y - transform.position.y + 1.5f).normalized;
                    GetComponent<Rigidbody2D>().velocity = dir * attackSpeed;
                    pushTime = 0.2f;
                    stunTime = attackStunTime;

                    attackTimer = attackCooldown;
                }
            }
            else if (distance <= minRadius)
            {
                GetComponent<Rigidbody2D>().velocity = -speedWhenDected * (spottedEnemy.position - transform.position).normalized;
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = speedWhenDected * (spottedEnemy.position - transform.position).normalized;
            }
            GetComponent<Rigidbody2D>().velocity = speedWhenDected * (spottedEnemy.position - transform.position).normalized;
        }
        else
        {
            transform.localScale = new Vector3(direction, 1, 1);
            direction = -direction;
        }

        if (Vector2.Distance(transform.position, spottedEnemy.position) <= attackRadius)
        {
            Vector2 dir = new Vector2(spottedEnemy.position.x - transform.position.x, spottedEnemy.position.y - transform.position.y + 1.5f).normalized;
            GetComponent<Rigidbody2D>().velocity = dir * attackSpeed;
            pushTime = 0.2f;
            stunTime = attackStunTime;
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
