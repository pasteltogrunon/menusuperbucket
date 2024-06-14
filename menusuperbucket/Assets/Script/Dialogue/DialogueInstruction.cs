using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Instruction")]
public class DialogueInstruction : ScriptableObject
{
    public InstructionType type;

    [TextArea(3, 10)] public string line;
    public Color textColor = Color.white;
    public Sprite image;
    public int eventChildIndex;
    public float waitTime;

    public enum InstructionType
    {
        Dialogue,
        ShowImage,
        Wait,
        Event,
        SwapDimension
    }
}
