using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearerTarget : MonoBehaviour, IProjectilable
{
    [SerializeField] GameObject[] activatingObjects;
    [SerializeField] GameObject[] deactivatingObjects;

    public void trigger()
    {
        foreach(GameObject activatingObject in activatingObjects)
        {
            activatingObject.SetActive(true);
        }

        foreach (GameObject deactivatingObject in deactivatingObjects)
        {
            deactivatingObject.SetActive(false);
        }

        this.enabled = false;
    }

}
