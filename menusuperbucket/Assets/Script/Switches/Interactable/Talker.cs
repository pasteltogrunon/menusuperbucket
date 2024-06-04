using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talker : MonoBehaviour, IInteractable
{
    [SerializeField] DialogueScene scene;

    public void interact()
    {
        DialogueManager.Instance.loadDialogueScene(scene);
    }
}
