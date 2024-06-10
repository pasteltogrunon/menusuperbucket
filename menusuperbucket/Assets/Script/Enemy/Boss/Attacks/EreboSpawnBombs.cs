using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboSpawnBombs : EreboAttackBase
{
    [SerializeField] float delay = 0.5f;
    [SerializeField] int maxBombs = 3;
    [SerializeField] float height = 5;
    [SerializeField] float sleepTime = 1;
    [SerializeField] float minDistanceToPlayer = 3;

    [SerializeField] GameObject bombPrefab;

    public override void StartAttack()
    {
        StartCoroutine(Spawn());
    }

    public override void Execute()
    {
        // Lógica continua del ataque si es necesario
    }

    IEnumerator Spawn()
    {
        bossAI.animator.Play("StartCharging");
        yield return new WaitForSeconds(healthDelayScale(delay));
        for(int i = 0; i<Random.Range(1, maxBombs+1); i++)
        {
            Vector2 spawnPos = new Vector2(Random.Range(bossAI.minAreaVertex.position.x, bossAI.maxAreaVertex.position.x),
                Random.Range(bossAI.minAreaVertex.position.y, bossAI.maxAreaVertex.position.y));
            if(Vector2.Distance(bossAI.Target.position, spawnPos) >= minDistanceToPlayer)
            {
                Instantiate(bombPrefab, spawnPos, Quaternion.identity);
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
