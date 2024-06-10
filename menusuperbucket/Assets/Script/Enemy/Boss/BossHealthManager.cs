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
                nextPhaseGameobject.SetActive(true);
                nextPhaseGameobject.transform.position = transform.position;
                gameObject.SetActive(false);
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

}
