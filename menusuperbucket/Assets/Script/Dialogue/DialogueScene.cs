using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Scene")]
public class DialogueScene : ScriptableObject
{
    public DialogueInstruction[] instructionList;
}
