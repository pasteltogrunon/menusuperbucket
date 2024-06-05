using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboJumpDash : EreboAttackBase
{
    [SerializeField] float jumpDelay = 0.2f;
    [SerializeField] float jumpForce = 10;
    [SerializeField] float dashDelay = 0.5f;
    [SerializeField] float dashSpeed = 15;
    [SerializeField] AnimationCurve dashSpeedProfile;
    [SerializeField] float dashTime = 0.3f;
    [SerializeField] float sleepTime = 1;

    [SerializeField] ParticleSystem preattack;

    public override void StartAttack()
    {
        StartCoroutine(JumpDash());
    }

    public override void Execute()
    {
        // Lógica continua del ataque si es necesario
    }

    IEnumerator JumpDash()
    {
        GetComponent<Rigidbody2D>().velocity = jumpForce * Vector2.up;
        yield return new WaitForSeconds(jumpDelay);
        Vector2 direction = targetDirection;
        preattack.Play();
        yield return new WaitForSeconds(dashDelay);

        for (float t = 0; t<dashTime; t+=Time.deltaTime)
        {
            GetComponent<Rigidbody2D>().velocity = direction * dashSpeed * dashSpeedProfile.Evaluate(t / dashTime);
            yield return null;
        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(sleepTime);
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
