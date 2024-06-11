using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboTeleportLaser : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float height = 5;
    [SerializeField] float sleepTime = 1;
    [SerializeField] float downSpeed = 10;

    [SerializeField] ParticleSystem laser;
    [SerializeField] ParticleSystem preLaser;

    [SerializeField] LayerMask playerLayer;

    public override void StartAttack()
    {
        StartCoroutine(Teleport());
    }

    public override void Execute()
    {
        // Lógica continua del ataque si es necesario
    }

    IEnumerator Teleport()
    {
        transform.position = bossAI.Target.position + height * Vector3.up;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        bossAI.animator.Play("PreShootingDown");
        preLaser.Play();
        yield return new WaitForSeconds(healthDelayScale(delay));
        bossAI.animator.Play("ShootingDown");
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.25f, Vector2.down, 30, playerLayer);

        if (hit)
        {
            if (hit.transform.TryGetComponent(out PlayerHurt hurt))
            {
                hurt.hurt(transform.position, 5);
            }
        }
        laser.Play();
        yield return new WaitForSeconds(healthDelayScale(sleepTime));

        transform.position = (bossAI.minAreaVertex.position.x + bossAI.maxAreaVertex.position.x) * 0.5f * Vector2.right + 2 * Vector2.up;

        GetComponent<Rigidbody2D>().gravityScale = 1;
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
