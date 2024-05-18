using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] int maxHealth = 10;
    [SerializeField] int health;

    public int Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, maxHealth);
            if(health == 0)
            {
                die();
            }
        }
    }

    
    void Awake()
    {
        health = maxHealth;
    }
    
    void die()
    {
        Destroy(gameObject);
    }
}
