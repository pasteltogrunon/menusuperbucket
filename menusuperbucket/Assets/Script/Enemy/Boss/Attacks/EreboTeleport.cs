using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboTeleport : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float height = 5;
    [SerializeField] float sleepTime = 1;
    [SerializeField] float downSpeed = 10;

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
        yield return new WaitForSeconds(healthDelayScale(delay));
        GetComponent<Rigidbody2D>().velocity = 10 * Vector2.down;
        GetComponent<Rigidbody2D>().gravityScale = 1;
        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
