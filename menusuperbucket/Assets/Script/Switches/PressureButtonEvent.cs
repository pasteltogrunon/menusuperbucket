using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressureButtonEvent : MonoBehaviour
{
    [SerializeField] UnityEvent unityEvent;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.GetComponent<PlayerController>() != null)
        {
            unityEvent?.Invoke();
        }
    }

}
