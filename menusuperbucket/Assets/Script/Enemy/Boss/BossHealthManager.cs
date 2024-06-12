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
                    nextPhaseGameobject.SetActive(true);
                    nextPhaseGameobject.transform.position = transform.position;
                    gameObject.SetActive(false);
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
}
