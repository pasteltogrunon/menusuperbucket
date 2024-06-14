using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    // Update is called once per frame
    void Update()
    {
        if(InputManager.Pause)
        {
            togglePauseMenu();
        }
    }

    public void togglePauseMenu()
    {
        if(optionsMenu.activeInHierarchy)
        {
            optionsMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            optionsMenu.SetActive(true);
            Time.timeScale = 0;
        }

    }
}
