using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessController : MonoBehaviour
{
    Material darkness;
    bool hasCandil = false;

    // Start is called before the first frame update
    void Start()
    {
        darkness = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        darkness.SetVector("_Player_World_Position", DimensionSwap.Instance.Astralis.transform.position);
    }

    public void unlockCandil()
    {
        hasCandil = true;
    }
}
