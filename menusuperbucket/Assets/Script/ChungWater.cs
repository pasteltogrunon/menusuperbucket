using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChungWater : MonoBehaviour
{
    [SerializeField] Transform respawnPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerHurt playerHurt))
        {
            playerHurt.hurtAlways(transform.position, 5);
            if(playerHurt.GetComponent<HealthManager>().Health != 0)
                collision.transform.position = respawnPosition.position;
        }
        else if (collision.TryGetComponent(out HealthManager healthManager))
        {
            //Instant kill
            healthManager.Health = 0;
        }
    }
}
