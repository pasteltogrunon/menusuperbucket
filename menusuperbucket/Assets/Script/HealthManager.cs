using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int MaxHealth = 10;
    [SerializeField] int health;

    [SerializeField] GameObject deathParticles;

    public int Health
    {
        get => health;
        set
        {
            if(value < health)
            {
                StartCoroutine(damageFX());
            }

            health = Mathf.Clamp(value, 0, MaxHealth);
            if(health == 0)
            {
                die();
            }
        }
    }

    
    void Awake()
    {
        health = MaxHealth;
    }
    
    void die()
    {
        if(deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    IEnumerator damageFX()
    {
        Material material = GetComponent<Renderer>().material;
        if(material.HasFloat("_Damage"))
        {
            for(float t = 0; t<= 0.2; t+= Time.deltaTime)
            {
                material.SetFloat("_Damage", 1 - t / 0.2f);
                yield return null;
            }
            material.SetFloat("_Damage", 0);
        }
    }
}
