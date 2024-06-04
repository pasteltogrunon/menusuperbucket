using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public static UnlockManager Instance;

    [SerializeField] PlayerController Astralis;
    [SerializeField] PlayerController Prometheus;

    private void Awake()
    {
        Instance = this;
    }

    public static void unlock(Unlockable unlockable)
    {
        switch(unlockable)
        {
            case Unlockable.Grapple:
                Instance.Prometheus.CanGrapple = true;
                break;
            default:
                break;
        }
    }

    public enum Unlockable
    {
        Grapple
    }
}
