using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] float timeBetweenCharacters = 0.1f; 
    [SerializeField] TMP_Text displayText;
    [SerializeField] Image displayImage;
    public static DialogueManager Instance;

    DialogueScene activeScene;

    DialogueInstruction activeInstruction;

    int instructionCount;
    int characterCount;
    float timer = 0;
    bool finishedLine;

    Transform dialogueSceneCaller;

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
            switch(activeInstruction.type)
            {
                case DialogueInstruction.InstructionType.Dialogue:
                    manageDialogueLine();
                    break;
                case DialogueInstruction.InstructionType.Wait:
                    manageWaitInstruction();
                    break;
                case DialogueInstruction.InstructionType.ShowImage:
                    manageImageLine();
                    break;
                case DialogueInstruction.InstructionType.Event:
                    managueEventInstruction();
                    break;
            }
        }
    }

    public void loadDialogueScene(DialogueScene scene)
    {
        if(activeScene == null)
        {
            activeScene = scene;

            InputManager.CinematicInputsLocked = true;
            if (DimensionSwap.Instance.activeCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) is CinemachineFramingTransposer framingTransposer)
            {
                framingTransposer.m_CameraDistance = 6;
            }

            instructionCount = 0;
            activeInstruction = activeScene.instructionList[instructionCount];

            characterCount = 0;
        }
    }

    public void loadDialogueScene(DialogueScene scene, Transform caller)
    {
        //If the scene contains an event, it must be called from here
        dialogueSceneCaller = caller;
        loadDialogueScene(scene);
    }

    void manageDialogueLine()
    {
        if (finishedLine)
        {
            if (InputManager.EnterLine)
            {
                nextInstruction();

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

    void manageWaitInstruction()
    {
        timer += Time.deltaTime;
        if(timer >= activeInstruction.waitTime)
        {
            nextInstruction();
        }
    }

    void manageImageLine()
    {
        if(displayImage.sprite != activeInstruction.image)
        {
            displayImage.sprite = activeInstruction.image;
            displayImage.gameObject.SetActive(true);
        }

        manageDialogueLine();
    }

    void managueEventInstruction()
    {
        dialogueSceneCaller.GetChild(activeInstruction.eventChildIndex).GetComponent<EventHolder>().unityEvent?.Invoke();

        nextInstruction();
    }

    void nextInstruction()
    {
        timer = 0;
        characterCount = 0;
        displayImage.gameObject.SetActive(false);

        instructionCount++;
        if(instructionCount < activeScene.instructionList.Length)
        {
            activeInstruction = activeScene.instructionList[instructionCount];
        }
        else
        {
            unLoadScene();
        }
    }

    public void unLoadScene()
    {
        InputManager.CinematicInputsLocked = false;

        if (DimensionSwap.Instance.activeCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) is CinemachineFramingTransposer framingTransposer)
        {
            framingTransposer.m_CameraDistance = 10;
        }

        displayText.text = null;
        activeScene = null;
        dialogueSceneCaller = null;
    }
}
