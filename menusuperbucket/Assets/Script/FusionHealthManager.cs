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
        StartCoroutine(deathSequence());
    }

    IEnumerator deathSequence()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<PlayerController>().enabled = false;

        Instantiate(deathParticles, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(2f);
        foreach(HealthManager health in EreboHealths)
        {
            health.Health = health.MaxHealth;
        }

        PrometeoHealth.transform.position = DimensionSwap.Instance.PrometeusSpawnPos;

        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<PlayerController>().enabled = true;

        onDeathEvent?.Invoke();
    }
}
