using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboShootProjectile : EreboAttackBase
{
    public GameObject projectilePrefab;

    [SerializeField] int projectileAmount = 4;
    [SerializeField] float intervalTime = 0.5f;
    [SerializeField] float projectileStartSpeed = 10;
    [SerializeField] float delayTime = 0.3f;
    [SerializeField] float sleepTime = 1;

    private Vector3 firePoint
    {
        get => (bossAI.Target.position - transform.position).normalized + transform.position;
    }

    public override void StartAttack()
    {
        // Lógica para iniciar el ataque
        StartCoroutine(ShootFireballs(projectileAmount));
    }

    public override void Execute()
    {
        // Lógica continua del ataque si es necesario
    }

    IEnumerator ShootFireballs(int amount)
    {
        bossAI.animator.Play("StartCharging");
        yield return new WaitForSeconds(healthDelayScale(delayTime));

        bossAI.animator.Play("Shooting");
        for (int i=0; i<Random.Range(1, projectileAmount + 1); i++)
        {
            ShootFireball();
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void ShootFireball()
    {
        Instantiate(projectilePrefab, firePoint, Quaternion.identity).GetComponent<Rigidbody2D>().velocity 
            = targetDirection * projectileStartSpeed;
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
