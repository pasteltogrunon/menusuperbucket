using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthManager : HealthManager
{
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBarCompanion;

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (float) Health / MaxHealth;
        healthBarCompanion.fillAmount = Mathf.Lerp(healthBarCompanion.fillAmount, (float) Health / MaxHealth, Time.deltaTime);
    }
}
