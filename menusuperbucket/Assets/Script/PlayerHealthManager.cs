using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : HealthManager
{
    [SerializeField] Character character;


    protected override void die()
    {
        if(deathParticles != null)
        {
            Instantiate(deathParticles, transform.position, Quaternion.identity);
        }

        StartCoroutine(respawn());
    }

    IEnumerator respawn()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<PlayerHurt>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        DialogueManager.Instance.unLoadScene();

        MusicManager.Instance.deathMusic();

        yield return new WaitForSeconds(7);

        GetComponent<HealthManager>().Health = GetComponent<HealthManager>().MaxHealth;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<PlayerController>().enabled = true;
        GetComponent<PlayerHurt>().enabled = true;
        GetComponent<Renderer>().enabled = true;

        switch (character)
        {
            case Character.Astralis:
                transform.position = DimensionSwap.Instance.AstralisSpawnPos;
                break;
            case Character.Prometeus:
                transform.position = DimensionSwap.Instance.PrometeusSpawnPos;
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }

    public enum Character
    {
        Astralis,
        Prometeus,
        Fusion
    }
}
