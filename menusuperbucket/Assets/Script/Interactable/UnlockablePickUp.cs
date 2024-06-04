using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockablePickUp : MonoBehaviour, IInteractable
{
    [SerializeField] UnlockManager.Unlockable unlockable;

    public void interact()
    {
        UnlockManager.unlock(unlockable);
        Destroy(gameObject);
    }
}
