using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] float radius = 2;
    [SerializeField] LayerMask interactLayer;

    void Update()
    {
        if(InputManager.Interact)
        {
            interact();
        }
    }

    void interact()
    {
        //Computing it 3D so it can be in the background
        Collider[] hit = Physics.OverlapSphere(transform.position, radius, interactLayer);

        if(hit.Length != 0 && hit[0].TryGetComponent(out IInteractable interactable))
        {
            interactable.interact();
        }
    }
}
