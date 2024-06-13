using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperAI : GroundStandardAI
{
    [SerializeField] float attackDelay = 0.1f;
    [SerializeField] float attackDuration = 1f;

    protected override void attack()
    {
        StartCoroutine(jumpAttack());
    }

    IEnumerator jumpAttack()
    {
        GetComponent<Rigidbody2D>().velocity = -0.5f * enemyDirection.normalized;
        GetComponent<Animator>().Play("jump");
        pushTime = attackDelay + attackDuration;
        yield return new WaitForSeconds(attackDelay);
        GetComponent<Rigidbody2D>().velocity = attackSpeed * enemyDirection.normalized + 2 * Vector2.up;
    }
}
