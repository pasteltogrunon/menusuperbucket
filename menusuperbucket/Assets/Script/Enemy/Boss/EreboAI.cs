using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EreboAI : MonoBehaviour
{
    private EreboAttackBase currentAttack;
    [SerializeField] private List<EreboAttackBase> possibleAttacks = new List<EreboAttackBase>();

    public Transform Target;
    public Transform minAreaVertex, maxAreaVertex;
    public Animator animator;

    public BoxCollider2D groundCheck;
    public LayerMask groundLayer;

    private float direction
    {
        get => Target.position.x - transform.position.x > 0 ? -1 : 1;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Añadir ataques al jefe
        //possibleAttacks.Add(GetComponent<EreboShootProjectile>());
        //possibleAttacks.Add(GetComponent<EreboDash>());
        //possibleAttacks.Add(GetComponent<EreboJumpDash>());
        //possibleAttacks.Add(GetComponent<EreboFisting>());
        //possibleAttacks.Add(GetComponent<EreboTeleport>());
        //possibleAttacks.Add(GetComponent<EreboSpawnBombs>());

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
        transform.localScale = new Vector3(direction, 1, 1);

        int randomIndex = Random.Range(0, possibleAttacks.Count);
        currentAttack = possibleAttacks[randomIndex];
        currentAttack.StartAttack();
    }

    public void OnAttackFinished()
    {
        // Elegir el siguiente ataque
        StartNextAttack();
    }

    public void endAttack()
    {
        currentAttack.StopAllCoroutines();
    }

}
