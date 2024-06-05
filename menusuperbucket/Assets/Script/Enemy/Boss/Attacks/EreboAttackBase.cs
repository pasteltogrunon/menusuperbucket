using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EreboAttackBase : MonoBehaviour
{
    protected EreboAI bossAI;

    void Awake()
    {
        bossAI = GetComponent<EreboAI>();
    }

    public abstract void StartAttack();
    public abstract void Execute();


    protected Vector2 targetDirection
    {
        get => (bossAI.Target.position - transform.position).normalized;
    }

    protected Vector2 horizontalTargetDirection
    {
        get => Mathf.Sign(bossAI.Target.position.x - transform.position.x) * Vector2.right;
    }
}
