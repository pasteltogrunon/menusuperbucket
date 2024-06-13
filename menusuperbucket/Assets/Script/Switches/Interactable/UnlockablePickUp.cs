using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnlockablePickUp : MonoBehaviour, IInteractable
{
    [SerializeField] UnlockManager.Unlockable unlockable;
    [SerializeField] UnityEvent unityEvent;

    public void interact()
    {
        UnlockManager.unlock(unlockable);
        unityEvent?.Invoke();
        Destroy(gameObject);
    }
}
