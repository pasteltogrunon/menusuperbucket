using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboSuperLaser : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float distance = 30;
    [SerializeField] float duration = 2;
    [SerializeField] float sleepTime = 1;

    [SerializeField] ParticleSystem laser;
    [SerializeField] ParticleSystem preLaser;

    [SerializeField] LayerMask playerLayer;

    public override void StartAttack()
    {
        StartCoroutine(Laser());
    }

    public override void Execute()
    {
        // L�gica continua del ataque si es necesario
    }

    IEnumerator Laser()
    {
        Vector2 direction = horizontalTargetDirection;
        bossAI.animator.Play("PreShooting");
        preLaser.Play();
        yield return new WaitForSeconds(delay);
        bossAI.animator.Play("Shooting");
        laser.Play();
        for(float t = 0; t < duration; t+=Time.deltaTime)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.25f, direction, distance, playerLayer);
            if(hit)
            {
                if(hit.transform.TryGetComponent(out PlayerHurt hurt))
                {
                    hurt.hurt(transform.position, 5);
                }
            }
            yield return null;
        }
        bossAI.animator.Play("Charging");
        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}