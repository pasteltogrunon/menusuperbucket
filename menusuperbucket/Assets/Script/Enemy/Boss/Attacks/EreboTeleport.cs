using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboTeleport : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float height = 5;
    [SerializeField] float sleepTime = 1;
    [SerializeField] float downSpeed = 10;

    [SerializeField] ParticleSystem teleportParticles;


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
        bossAI.animator.Play("StartCharging");
        teleportParticles.Play();

        yield return new WaitForSeconds(healthDelayScale(delay));
        bossAI.animator.Play("JumpAttack");
        GetComponent<Rigidbody2D>().velocity = 10 * Vector2.down;
        GetComponent<Rigidbody2D>().gravityScale = 1;
        while(!Physics2D.OverlapBox(bossAI.groundCheck.transform.position, bossAI.groundCheck.size, 0, bossAI.groundLayer))
        {
            yield return null;
        }
        bossAI.animator.Play("JumpAttackEnd");
        yield return new WaitForSeconds(healthDelayScale(sleepTime));

        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
