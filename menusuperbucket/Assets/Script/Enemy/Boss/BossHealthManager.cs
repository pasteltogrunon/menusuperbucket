using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthManager : HealthManager
{
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarCompanion;

    public int phase;
    [SerializeField] GameObject nextPhaseGameobject;
    [SerializeField] PlaceHolderFracture fracture;

    [SerializeField] AudioSource music;
    [SerializeField] AudioSource scalingMusic;

    [SerializeField] AudioSource transformationSound;

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float) Health / MaxHealth;
        healthBarCompanion.fillAmount = Mathf.Lerp(healthBarCompanion.fillAmount, (float) Health / MaxHealth, Time.deltaTime);
    }

    protected override void onDamage()
    {
        base.onDamage();

        if (phase == 1)
        {
            if(Health <= 20)
            {
                StartCoroutine(phase1End());
            }
        }
        else if(phase == 2)
        {
            if(Health <= 20)
            {
                Health = 20;
                if(!fracture.addHitToFracture())
                {
                    StartCoroutine(phase2End());
                }
            }
        }
    }


    IEnumerator phase1End()
    {
        GetComponent<EreboAI>().endAttack();
        GetComponent<EreboAI>().enabled = false;
        music.Stop();
        GetComponent<Animator>().Play("Phase1End");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(4f);
        CameraManager.cameraShake(1, 4, 3);
        yield return new WaitForSeconds(1f);
        nextPhaseGameobject.SetActive(true);
        nextPhaseGameobject.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    IEnumerator phase2End()
    {
        GetComponent<EreboAI>().endAttack();
        GetComponent<EreboAI>().enabled = false;
        StartCoroutine(fadeOutMusic(4));
        GetComponent<Animator>().Play("Phase2End");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(6.324f);
        scalingMusic.Play();
        yield return new WaitForSeconds(2f);
        transformationSound.Play();
        yield return new WaitForSeconds(10.676f);
        CameraManager.cameraShake(1, 4, 3);
        yield return new WaitForSeconds(1f);
        nextPhaseGameobject.SetActive(true);
        nextPhaseGameobject.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    IEnumerator fadeOutMusic(float fadeOutDuration)
    {
        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            setVolume(music, 1 - t / fadeOutDuration);
            yield return null;
        }

        music.Stop();
    }

    void setVolume(AudioSource source, float volume)
    {
        source.volume = Mathf.Clamp01(volume);
    }
}
