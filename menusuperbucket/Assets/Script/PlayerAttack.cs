using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int damage = 5;
    [SerializeField] BoxCollider2D attackHitbox;

    Animator animator;

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
    }

    void attack()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(attackHitbox.transform.position, attackHitbox.size, 0);

        foreach(Collider2D h in hit)
        {
            if(h.TryGetComponent(out HealthManager healthManager))
            {
                healthManager.Health -= damage;
            }
        }

        animator.Play("Astralis_Attack1");
    }
}
