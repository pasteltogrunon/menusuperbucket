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
            case Unlockable.Platform:
                Instance.Prometheus.CanPlatform = true;
                break;
            case Unlockable.PrometeoHealth:
                Instance.Prometheus.GetComponent<HealthManager>().MaxHealth += 5;
                Instance.Prometheus.GetComponent<HealthManager>().Health += 5;
                break;
            case Unlockable.AstralisHealth:
                Instance.Astralis.GetComponent<HealthManager>().MaxHealth += 5;
                Instance.Astralis.GetComponent<HealthManager>().Health += 5;
                break;
            case Unlockable.DoubleJump:
                Instance.Astralis.GetComponent<PlayerMovement>().canDoubleJump = true;
                break;
            case Unlockable.WallJump:
                Instance.Astralis.GetComponent<PlayerMovement>().canWallJump = true;
                break;
            default:
                break;
        }
    }

    public enum Unlockable
    {
        Grapple,
        Platform,
        PrometeoHealth,
        AstralisHealth,
        DoubleJump,
        WallJump
    }
}
