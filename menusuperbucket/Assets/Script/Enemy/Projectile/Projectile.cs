using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] LayerMask hitLayer;
    [SerializeField] float damageRadius = 1;
    [SerializeField] int damage = 5;

    [SerializeField] float lifetime = 5;

    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem continuousParticles;

    [SerializeField] AudioSource continuousSource;
    [SerializeField] AudioSource deathSource;

    bool dead;

    private void Start()
    {
        dead = false;
        StartCoroutine(scheduleDeath());
    }

    private void Update()
    {
        onUpdate();
    }

    protected virtual void onUpdate()
    {
        if(!dead)
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

                die();
            }
        }
    }

    IEnumerator scheduleDeath()
    {
        yield return new WaitForSeconds(lifetime);
        die();
    }

    protected virtual void die()
    {
        dead = true;

        continuousSource?.Stop();
        deathSource?.Play();

        if(deathSource != null)
            Destroy(deathSource, 4f);

        deathParticles.transform.SetParent(null);
        deathParticles.Play();

        continuousParticles.Stop();
        GetComponent<Renderer>().enabled = false;

        Destroy(gameObject, 2f);
    }
}
