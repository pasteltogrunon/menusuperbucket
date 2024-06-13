using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTalk : MonoBehaviour
{
    [SerializeField] DialogueScene dialogueScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.TryGetComponent(out PlayerController player))
        {
            DialogueManager.Instance.loadDialogueScene(dialogueScene, transform);
        }
    }
}
