using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour
{
    [SerializeField] float invulnerableTime = 0.5f;
    [SerializeField] Vector2 damageKnockback;
    [SerializeField] LayerMask enemyLayer;

    bool _isInvulnerable = false;

    //Sets the stun also
    bool IsInvulnerable
    {
        get => _isInvulnerable;
        set
        {
            _isInvulnerable = value;
            GetComponent<PlayerController>().Stunned = value;
        }
    }

    BoxCollider2D hitbox;

    void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        //Only compute if not invulnerable
        if (IsInvulnerable) return;

        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y) + hitbox.offset, hitbox.size, 0, enemyLayer);

        if(hit.Length != 0)
        {
            //Health damage
            GetComponent<HealthManager>().Health -= 5;

            //Damage knockback
            float enemyDirection = Mathf.Sign(transform.position.x - hit[0].transform.position.x);
            GetComponent<Rigidbody2D>().velocity = new Vector2(damageKnockback.x * enemyDirection, damageKnockback.y + GetComponent<Rigidbody2D>().velocity.y);
            
            //Invulnerability
            StartCoroutine(damageCooldown());
        }

    }

    IEnumerator damageCooldown()
    {
        IsInvulnerable = true;
        yield return new WaitForSeconds(invulnerableTime);
        IsInvulnerable = false;
    }
}
