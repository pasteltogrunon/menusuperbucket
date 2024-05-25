using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerAI : GroundStandardAI
{
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] float attackDuration = 0.3f;

    protected override void attack()
    {
        StartCoroutine(dashAttack());
    }

    IEnumerator dashAttack()
    {
        pushTime = attackDelay + attackDuration;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(attackDelay);
        GetComponent<Rigidbody2D>().velocity = enemyDirection.normalized * attackSpeed;
        yield return new WaitForSeconds(attackDuration);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
