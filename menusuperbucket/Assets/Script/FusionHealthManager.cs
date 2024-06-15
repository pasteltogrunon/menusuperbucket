using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FusionHealthManager : HealthManager
{
    [SerializeField] HealthManager AstralisHealth;
    [SerializeField] HealthManager PrometeoHealth;

    [SerializeField] HealthManager[] EreboHealths;

    [SerializeField] UnityEvent onDeathEvent;

    // Start is called before the first frame update
    void OnEnable()
    {
        MaxHealth = AstralisHealth.MaxHealth + PrometeoHealth.MaxHealth;
        Health = MaxHealth;
    }


    protected override void die()
    {
        foreach(HealthManager health in EreboHealths)
        {
            health.Health = health.MaxHealth;
        }

        PrometeoHealth.transform.position = DimensionSwap.Instance.PrometeusSpawnPos;

        onDeathEvent?.Invoke();
    }
}
