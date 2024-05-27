using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedProjectile : Projectile
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float targetRadius = 1;
    [SerializeField] float acceleration = 2;
    [SerializeField] float maxSpeed = 5;

    protected override void onUpdate()
    {
        base.onUpdate();

        Collider2D hit = Physics2D.OverlapCircle(transform.position, targetRadius, targetLayer);
        if(hit != null)
        {
            GetComponent<Rigidbody2D>().velocity += acceleration * Time.deltaTime * ((Vector2)(hit.transform.position - transform.position)).normalized;
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * maxSpeed;
        }
    }

}
