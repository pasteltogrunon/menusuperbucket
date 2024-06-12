using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboLaser : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float distance = 30;
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
        // Lógica continua del ataque si es necesario
    }

    IEnumerator Laser()
    {
        Vector2 direction = horizontalTargetDirection;
        bossAI.animator.Play("Charging");
        preLaser.Play();
        yield return new WaitForSeconds(delay);

        CameraManager.cameraShake(0.3f, 5, 3);
        bossAI.animator.Play("Shooting");

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.25f, direction, distance, playerLayer);
        if(hit)
        {
            if(hit.transform.TryGetComponent(out PlayerHurt hurt))
            {
                hurt.hurt(transform.position, 5);
            }
        }
        laser.Play();
        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
