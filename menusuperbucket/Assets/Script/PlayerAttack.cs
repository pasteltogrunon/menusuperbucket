using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int damage = 5;
    [SerializeField] float attackMargin = 0.4f;
    [SerializeField] float secondAttackDelay = 0.1f;
    [SerializeField] BoxCollider2D attackHitbox;

    Animator animator;

    int attackCount = 0;
    float attackTimer = 0;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            attack();
        }

        if(attackCount > 0)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer > attackMargin)
            {
                attackTimer = 0;
                attackCount = 0;
            }
        }
    }

    void attack()
    {
        if(attackCount == 0 || attackCount == 1 && attackTimer > secondAttackDelay)
        {
            Collider2D[] hit = Physics2D.OverlapBoxAll(attackHitbox.transform.position, attackHitbox.size, 0);

            foreach(Collider2D h in hit)
            {
                if(h.TryGetComponent(out HealthManager healthManager))
                {
                    healthManager.Health -= damage;
                }
            }

            if(attackCount == 0)
            {
                animator.Play("Astralis_Attack1");
            }
            else
            {
                animator.Play("Astralis_Attack2");
            }

            attackTimer = 0;
            attackCount++;

        }

    }
}
