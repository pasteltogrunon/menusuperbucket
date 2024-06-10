using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour, IInteractable
{
    void IInteractable.interact()
    {
        DimensionSwap.Instance.swapDimension(transform);
    }
}
