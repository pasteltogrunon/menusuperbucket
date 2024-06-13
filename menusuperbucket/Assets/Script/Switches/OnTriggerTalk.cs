using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTalk : MonoBehaviour
{
    DialogueScene dialogueScene;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent(out PlayerController player))
        {
            DialogueManager.Instance.loadDialogueScene(dialogueScene, transform);
        }
    }
}
