using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearerTarget : MonoBehaviour, IProjectilable
{
    [SerializeField] GameObject activatingObject;
    [SerializeField] GameObject deactivatingObject;

    public void trigger()
    {
        activatingObject.SetActive(true);
        deactivatingObject.SetActive(false);
        this.enabled = false;
    }

}
