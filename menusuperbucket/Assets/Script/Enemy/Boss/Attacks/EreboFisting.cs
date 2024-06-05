using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboFisting : EreboAttackBase
{
    public GameObject fistPrefab;

    [SerializeField] float projectileStartSpeed = 40;
    [SerializeField] float duration = 0.3f;
    [SerializeField] AnimationCurve speedProfile;
    [SerializeField] float delayTime = 0.3f;
    [SerializeField] float sleepTime = 1;

    private Vector3 firePoint
    {
        get => (bossAI.Target.position - transform.position).normalized + transform.position + 2 * Vector3.up;
    }

    public override void StartAttack()
    {
        // Lógica para iniciar el ataque
        StartCoroutine(ShootFist());
    }

    public override void Execute()
    {
        // Lógica continua del ataque si es necesario
    }

    IEnumerator ShootFist()
    {
        Rigidbody2D fistRB = Instantiate(fistPrefab, firePoint, Quaternion.identity).GetComponent<Rigidbody2D>();
        fistRB.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(healthDelayScale(delayTime));
        fistRB.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector2 direction = (bossAI.Target.position - firePoint).normalized;


        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            fistRB.velocity = direction * projectileStartSpeed * speedProfile.Evaluate(t/duration);
            yield return null;
        }

        //fistRB.GetComponent<Collider2D>().enabled = false;
        Destroy(fistRB.gameObject);

        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
