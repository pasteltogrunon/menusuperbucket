using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionSwap : MonoBehaviour
{
    public static DimensionSwap Instance;

    bool past = false;

    [SerializeField] GameObject Prometeus;
    [SerializeField] GameObject Astralis;

    [SerializeField] GameObject Past;
    [SerializeField] GameObject Future;

    [SerializeField] CinemachineVirtualCamera AstralisCam;
    [SerializeField] CinemachineVirtualCamera PrometeusCam;


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {

    }

    public void swapDimension()
    {
        Prometeus.SetActive(!past);
        Past.SetActive(!past);
        Astralis.SetActive(past);
        Future.SetActive(past);
        past = !past;

        if(past)
        {
            PrometeusCam.Priority = 11;
        }
        else
        {
            PrometeusCam.Priority = 9;
        }
    }
}
