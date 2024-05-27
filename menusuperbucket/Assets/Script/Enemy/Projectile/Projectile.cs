using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask hitLayer;
    [SerializeField] float damageRadius = 1;
    [SerializeField] int damage = 5;

    [SerializeField] float lifetime = 5;

    private void Start()
    {
        StartCoroutine(scheduleDeath());
    }

    private void Update()
    {
        onUpdate();
    }

    protected virtual void onUpdate()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, damageRadius, hitLayer);
        if(hit.Length != 0)
        {
            foreach(Collider2D h in hit)
            {
                if(h.TryGetComponent(out PlayerHurt playerHealth))
                {
                    playerHealth.hurt(transform.position, 5);
                }
                else if(h.TryGetComponent(out HealthManager health))
                {
                    health.Health -= 5;
                }
            }
            Destroy(gameObject);
        }
    }

    IEnumerator scheduleDeath()
    {
        yield return new WaitForSeconds(lifetime);
        die();
    }

    protected virtual void die()
    {
        Destroy(gameObject);
    }
}
