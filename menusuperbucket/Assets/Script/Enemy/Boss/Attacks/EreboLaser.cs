using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboLaser : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float distance = 10;
    [SerializeField] float sleepTime = 1;

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
        Vector2 direction = targetDirection;
        yield return new WaitForSeconds(healthDelayScale(delay));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, distance);
        if(hit)
        {
            if(hit.transform.TryGetComponent(out PlayerHurt hurt))
            {
                hurt.hurt(transform.position, 5);
            }
        }
        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
