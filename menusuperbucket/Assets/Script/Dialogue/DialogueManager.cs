using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Distinguir si hay una activa o no

        //Mostrar el string letra por letra

        //Esperar para la siguiente línea
        Debug.Log("caca");
    }

    public void loadDialogueScene(DialogueScene scene)
    {

    }
}

public class DialogoPersonaje : MonoBehaviour
{
    DialogueScene scene;


    void interact()
    {
        DialogueManager.Instance.loadDialogueScene(scene);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
