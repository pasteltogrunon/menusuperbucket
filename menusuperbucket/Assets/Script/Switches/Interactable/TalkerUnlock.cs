using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkerUnlock : MonoBehaviour, IInteractable
{
    [SerializeField] UnlockManager.Unlockable unlockable;

    [SerializeField] DialogueScene scene;


    public void interact()
    {
        UnlockManager.unlock(unlockable);
        DialogueManager.Instance.loadDialogueScene(scene, transform);
    }
}
