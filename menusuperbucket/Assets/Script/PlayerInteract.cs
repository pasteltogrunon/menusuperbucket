using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] float radius = 2;
    [SerializeField] LayerMask interactLayer;

    [SerializeField] ParticleSystem interactableParticles;

    void Update()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, radius, interactLayer);

        if (hit.Length != 0 && hit[0].TryGetComponent(out IInteractable interactable))
        {
            interactableParticles.transform.position = hit[0].transform.position;
            if (!interactableParticles.isPlaying)
                interactableParticles.Play();

            if (InputManager.Interact)
                interactable.interact();
        }
        else
        {
            interactableParticles.Stop();
        }
    }
        
}
