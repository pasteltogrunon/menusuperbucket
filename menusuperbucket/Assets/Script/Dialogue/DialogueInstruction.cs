using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Instruction")]
public class DialogueInstruction : ScriptableObject
{
    public InstructionType type;
    [TextArea(100, 10)] public string line;

    public enum InstructionType
    {
        Dialogue,
        ShowImage
    }
}
