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

    // Update is called once per frame
    void Update()
    {
        if(InputManager.Attack)
        {
            if(index < sprites.Length)
                GetComponent<SpriteRenderer>().sprite = sprites[index];

            index++;
        }
    }
}
