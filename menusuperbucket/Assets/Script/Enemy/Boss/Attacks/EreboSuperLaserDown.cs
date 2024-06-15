using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboSuperLaserDown : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] float distance = 30;
    [SerializeField] float duration = 2;
    [SerializeField] float sleepTime = 1;

    [SerializeField] ParticleSystem laser;
    [SerializeField] ParticleSystem preLaser;

    [SerializeField] AnimationCurve speedProfile;
    [SerializeField] float projectileStartSpeed = 10;
    [SerializeField] GameObject wavePrefab;

    [SerializeField] LayerMask playerLayer;

    [SerializeField] AudioSource source;



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
        Vector2 spawnPoint = bossAI.Target.position + 15 * Vector3.up;

        bossAI.animator.Play("PreShootingUp");
        preLaser.Play();
        preLaser.transform.position = spawnPoint;
        source.Play();
        yield return new WaitForSeconds(delay);

        CameraManager.cameraShake(duration, 5, 4);

        Rigidbody2D waveRightRB = Instantiate(wavePrefab, new Vector2(spawnPoint.x, 1), Quaternion.identity).GetComponent<Rigidbody2D>();
        Destroy(waveRightRB.gameObject, delay + duration + 0.1f);

        Rigidbody2D waveLeftRB = Instantiate(wavePrefab, new Vector2(spawnPoint.x, 1), Quaternion.identity).GetComponent<Rigidbody2D>();
        Destroy(waveLeftRB.gameObject, delay + duration + 0.1f);

        bossAI.animator.Play("ShootingUp");
        laser.transform.position = spawnPoint + 15 * Vector2.down;
        laser.Play();

        for(float t = 0; t < duration; t+=Time.deltaTime)
        {
            RaycastHit2D hit = Physics2D.CircleCast(spawnPoint, 0.25f, Vector2.down, distance, playerLayer);
            if(hit)
            {
                if(hit.transform.TryGetComponent(out PlayerHurt hurt))
                {
                    hurt.hurt(transform.position, 5);
                }
            }

            waveRightRB.velocity = projectileStartSpeed * speedProfile.Evaluate(t / duration) * Vector2.right;
            waveLeftRB.velocity = -projectileStartSpeed * speedProfile.Evaluate(t / duration) * Vector2.right;

            yield return null;
        }
        bossAI.animator.Play("Charging");
        yield return new WaitForSeconds(healthDelayScale(sleepTime));
        EndAttack();
    }

    void EndAttack()
    {
        bossAI.OnAttackFinished();
    }

}
