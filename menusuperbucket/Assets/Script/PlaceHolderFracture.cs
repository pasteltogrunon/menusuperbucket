using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderFracture : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
    }


    public bool addHitToFracture()
    {
        if (index < sprites.Length)
            GetComponent<SpriteRenderer>().sprite = sprites[index];

        index++;
        return index < sprites.Length;
    }
}
