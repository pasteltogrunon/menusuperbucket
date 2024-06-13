using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboDash : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float dashSpeed = 15;
    [SerializeField] AnimationCurve dashSpeedProfile;
    [SerializeField] float dashTime = 0.3f;
    [SerializeField] float sleepTime = 1;

    [SerializeField] ParticleSystem preattack;
    [SerializeField] AudioSource dashSound;

    public override void StartAttack()
    {
        StartCoroutine(Dash());
    }

    public override void Execute()
    {
        // Lógica continua del ataque si es necesario
    }

    IEnumerator Dash()
    {
        Vector2 direction = horizontalTargetDirection;
        preattack.Play();
        bossAI.animator.Play("StartCharging");
        yield return new WaitForSeconds(healthDelayScale(delay));
        bossAI.animator.Play("Dashing");
        dashSound.Play();

        for (float t = 0; t<dashTime; t+=Time.deltaTime)
        {
            GetComponent<Rigidbody2D>().velocity = direction * dashSpeed * dashSpeedProfile.Evaluate(t / dashTime);
            yield return null;
        }
        bossAI.animator.Play("DashEnd");

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
