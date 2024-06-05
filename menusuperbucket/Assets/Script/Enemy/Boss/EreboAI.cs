using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboAI : MonoBehaviour
{
    private EreboAttackBase currentAttack;
    private List<EreboAttackBase> possibleAttacks = new List<EreboAttackBase>();

    public Transform Target;


    void Start()
    {
        // Añadir ataques al jefe
        possibleAttacks.Add(GetComponent<EreboShootProjectile>());
        possibleAttacks.Add(GetComponent<EreboDash>());
        possibleAttacks.Add(GetComponent<EreboJumpDash>());
        possibleAttacks.Add(GetComponent<EreboFisting>());

        // Iniciar el primer ataque
        StartNextAttack();
    }

    void Update()
    {
        if (currentAttack != null)
        {
            currentAttack.Execute();
        }
    }

    void StartNextAttack()
    {
        int randomIndex = Random.Range(0, possibleAttacks.Count);
        currentAttack = possibleAttacks[randomIndex];
        currentAttack.StartAttack();
    }

    public void OnAttackFinished()
    {
        // Elegir el siguiente ataque
        StartNextAttack();
    }

}
