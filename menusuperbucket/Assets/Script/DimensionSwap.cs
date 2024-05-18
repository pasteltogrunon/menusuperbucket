using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionSwap : MonoBehaviour
{
    public static DimensionSwap Instance;

    bool past = false;

    [SerializeField] GameObject Prometeus;
    [SerializeField] GameObject Astralis;


    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
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
