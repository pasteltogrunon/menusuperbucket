using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerHurt : MonoBehaviour
{
    [SerializeField] float invulnerableTime = 0.5f;
    [SerializeField] float stunTime = 0.5f;
    [SerializeField] Vector2 damageKnockback;
    [SerializeField] LayerMask enemyLayer;

    [Header("FX")]
    [SerializeField] Volume damageVolume;
    [SerializeField] AudioSource hurtSource;
    [SerializeField] AudioClip[] hurtSounds = new AudioClip[3];

    bool _isInvulnerable = false;

    //Sets the stun also
    public bool IsInvulnerable
    {
        get => _isInvulnerable;
        set
        {
            _isInvulnerable = value;
            GetComponent<PlayerController>().Stunned = value;
        }
    }

    BoxCollider2D hitbox;

    void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        //Only compute if not invulnerable
        if (IsInvulnerable) return;

        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x, transform.position.y) + hitbox.offset, hitbox.size, 0, enemyLayer);

        if(hit.Length != 0)
        {
            hurt(hit[0].transform.position, 5);
        }

    }

    public void hurt(Vector2 pos, int damage)
    {
        if (IsInvulnerable) return;

        hurtAlways(pos, damage);
    }

    public void hurtAlways(Vector2 pos, int damage)
    {
        //Health damage
        GetComponent<HealthManager>().Health -= damage;

        //Damage knockback
        float enemyDirection = Mathf.Sign(transform.position.x - pos.x);
        GetComponent<Rigidbody2D>().velocity = new Vector2(damageKnockback.x * enemyDirection, damageKnockback.y);
        hurtSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
        //Invulnerability
        StartCoroutine(damageCooldown());
    }


    IEnumerator damageCooldown()
    {
        IsInvulnerable = true;
        bool cleanedStun = false;

        Material material = GetComponent<Renderer>().material;
        for(float t = 0; t<= invulnerableTime; t+= Time.deltaTime)
        {
            if(t > stunTime && !cleanedStun)
            {
                GetComponent<PlayerController>().Stunned = false;
                cleanedStun = true;
            }

            if(material.HasFloat("_Invulnerable"))
            {
                material.SetFloat("_Invulnerable", 1 - t / invulnerableTime);
            }

            if(damageVolume != null)
                damageVolume.weight = 1 - t / invulnerableTime;

            yield return null;
        }

        if (material.HasFloat("_Invulnerable"))
        {
            material.SetFloat("_Invulnerable", 0);
        }
        _isInvulnerable = false;

    }
}
