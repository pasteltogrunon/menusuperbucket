using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderFracture : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    [SerializeField] Material dimension4mat;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        dimension4mat.SetFloat("_Phase", 0);
    }


    public bool addHitToFracture()
    {
        if (index < sprites.Length)
            GetComponent<SpriteRenderer>().sprite = sprites[index];
        else
        {
            StartCoroutine(changeDimension());
        }

        index++;
        return index < sprites.Length + 1;
    }

    IEnumerator changeDimension()
    {
        yield return new WaitForSeconds(6);
        GetComponent<SpriteRenderer>().sprite = null;
        float duration = 1;
        for(float t= 0; t< duration; t+=Time.deltaTime)
        {
            dimension4mat.SetFloat("_Phase", t / duration);
            yield return null;
        }
        dimension4mat.SetFloat("_Phase", 1);

    }
}
