using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerAI : GroundStandardAI
{
    [SerializeField] float attackDelay = 0.3f;
    [SerializeField] float attackDuration = 4f;
    [SerializeField] float attackAcceleration = 4f;
    [SerializeField] float attackBurstSpeed = 5f;

    [Space]

    [SerializeField] BoxCollider2D spinHitbox;

    [SerializeField] LayerMask invulnerableLayer;
    [SerializeField] LayerMask enemyLayer;

    protected override void attack()
    {
        StartCoroutine(spinAttack());
    }

    IEnumerator spinAttack()
    {
        pushTime = attackDelay + attackDuration;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Animator>().Play("preattack");

        yield return new WaitForSeconds(attackDelay);

        GetComponent<Rigidbody2D>().velocity = enemyDirection.normalized * attackBurstSpeed * Time.deltaTime;


        spinHitbox.gameObject.SetActive(true);
        gameObject.layer = 3;
        for(float t=0; t<= attackDuration; t+=Time.deltaTime)
        {
            GetComponent<Animator>().SetFloat("SpinTime", attackDuration - t);

            GetComponent<Rigidbody2D>().velocity += Mathf.Sign(enemyDirection.x) * attackAcceleration * Time.deltaTime * Vector2.right;
            if(GetComponent<Rigidbody2D>().velocity.magnitude >= attackSpeed)
            {
                GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * attackSpeed;
            }
            yield return null;
        }

        GetComponent<Animator>().SetFloat("SpinTime", 0);

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.layer = 7;
        spinHitbox.gameObject.SetActive(false);
    }
}
