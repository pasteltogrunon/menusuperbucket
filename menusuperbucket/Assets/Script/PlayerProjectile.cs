using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] ParticleSystem destroyParticles;
    [SerializeField] ParticleSystem continuousParticles;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.TryGetComponent(out IProjectilable target))
        {
            target.trigger();
        }
        StartCoroutine(retire());
    }

    IEnumerator retire()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<Renderer>().enabled = false;
        destroyParticles.transform.parent = null;
        destroyParticles.Play();
        continuousParticles.Stop();
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
