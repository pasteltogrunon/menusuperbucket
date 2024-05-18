using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] int damage = 5;
    [SerializeField] BoxCollider2D attackHitbox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            attack();
        }
    }

    void attack()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(attackHitbox.transform.position, attackHitbox.size, 0);

        foreach(Collider2D h in hit)
        {
            if(h.TryGetComponent(out HealthManager healthManager))
            {
                healthManager.Health -= damage;
            }
        }
    }
}
