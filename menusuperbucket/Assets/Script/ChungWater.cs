using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChungWater : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out HealthManager healthManager))
        {
            //Instant kill
            healthManager.Health = 0;
        }
    }
}
