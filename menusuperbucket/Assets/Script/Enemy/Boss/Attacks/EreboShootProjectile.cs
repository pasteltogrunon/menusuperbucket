using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboShootProjectile : EreboAttackBase
{
    public GameObject projectilePrefab;

    [SerializeField] int projectileAmount = 4;
    [SerializeField] float intervalTime = 0.5f;
    [SerializeField] float projectileStartSpeed = 10;
    [SerializeField] float sleepTime = 1;

    private Vector3 firePoint
    {
        get => (bossAI.Target.position - transform.position).normalized + transform.position;
    }

    public override void StartAttack()
    {
        // L�gica para iniciar el ataque
        StartCoroutine(ShootFireballs(projectileAmount));
    }

    public override void Execute()
    {
        // L�gica continua del ataque si es necesario
    }

    IEnumerator ShootFireballs(int amount)
    {
        for(int i=0; i<projectileAmount; i++)
        {
            ShootFireball();
            yield return new WaitForSeconds(intervalTime);
        }
        yield return new WaitForSeconds(sleepTime);
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
