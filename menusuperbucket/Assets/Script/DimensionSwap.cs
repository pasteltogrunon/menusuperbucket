using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionSwap : MonoBehaviour
{
    public static DimensionSwap Instance;

    bool past = false;

    [SerializeField] GameObject Prometeus;
    [SerializeField] GameObject Astralis;


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
        Astralis.SetActive(past);
        past = !past;

        if(past)
        {
            Prometeus.transform.position = Astralis.transform.position;
        }
        else
        {
            Astralis.transform.position = Prometeus.transform.position;
        }
    }
}
