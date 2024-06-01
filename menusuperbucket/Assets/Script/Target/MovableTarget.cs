using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTarget : MonoBehaviour, IProjectilable
{
    [SerializeField] float time = 1;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Vector3 target;

    [SerializeField] Transform movingObject;

    public void trigger()
    {
        StartCoroutine(move());
    }

    IEnumerator move()
    {
        Vector3 position =  movingObject.localPosition;
        for(float t=0; t <= time; t+=Time.deltaTime)
        {
            movingObject.localPosition = position + curve.Evaluate(t / time) * (target - position);
            yield return null;
        }
        movingObject.localPosition = position + curve.Evaluate(1) * (target - position);
        this.enabled = false;
    }
}
