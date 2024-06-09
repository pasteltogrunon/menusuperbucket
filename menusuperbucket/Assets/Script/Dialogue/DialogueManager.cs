using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] float timeBetweenCharacters = 0.1f; // no funciona creo -ruchu
    [SerializeField] TMP_Text displayText;
    public static DialogueManager Instance;


    DialogueScene activeScene;

    DialogueInstruction activeInstruction;

    int instructionCount;
    int characterCount;
    float timer = 0;
    bool finishedLine;

    void Awake()
    {
        Instance = this; //nuts
    }


    void Update()
    {
        //Distinguir si hay una activa o no
        if(activeScene != null)
        {
            //Checkear el tipo de instruccion y actuar en consecuencia
            if(activeInstruction.type == DialogueInstruction.InstructionType.Dialogue)
            {
                manageDialogueLine();
            }
        }
    }

    public void loadDialogueScene(DialogueScene scene)
    {
        if(activeScene == null)
        {
            activeScene = scene;

            instructionCount = 0;
            activeInstruction = activeScene.instructionList[instructionCount];

            characterCount = 0;
        }
    }

    void manageDialogueLine()
    {
        if (finishedLine)
        {
            if (InputManager.EnterLine)
            {
                instructionCount++;
                if(instructionCount < activeScene.instructionList.Length)
                {
                    activeInstruction = activeScene.instructionList[instructionCount];
                    characterCount = 0;
                }
                else
                {
                    displayText.text = null;
                    activeScene = null;
                }

                finishedLine = false;
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= timeBetweenCharacters)
            {
                timer -= timeBetweenCharacters;
                characterCount++;

                displayText.text = activeInstruction.line.Substring(0, Mathf.Min(characterCount, activeInstruction.line.Length));
                if (characterCount == activeInstruction.line.Length)
                {
                    finishedLine = true;
                }
            }

        }
    }
}
