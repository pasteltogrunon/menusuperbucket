using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTarget : MonoBehaviour, IProjectilable
{
    [SerializeField] UnityEvent unityEvent;

    public void trigger()
    {
        unityEvent?.Invoke();
    }
}
