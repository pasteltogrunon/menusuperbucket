using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] GameObject activeForm;
    [SerializeField] GameObject pressedForm;

    private void Start()
    {
        transform.GetComponentInParent<CombinationLock>().resetCombination += resetCombination;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.GetComponent<PlayerController>() != null)
        {
            activeForm.SetActive(false);
            pressedForm.SetActive(true);
            transform.GetComponentInParent<CombinationLock>().activate(index);
        }
    }

    void resetCombination()
    {
        activeForm.SetActive(true);
        pressedForm.SetActive(false);
    }
}
