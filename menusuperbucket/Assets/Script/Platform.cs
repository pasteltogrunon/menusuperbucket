using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] float lifetime = 2;
    [SerializeField] float duration = 3;
    [SerializeField] float verticalSpeed = 0.5f;

    float time = 0;
    Material material;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= verticalSpeed * Time.deltaTime * Vector3.up;
        time += Time.deltaTime;

        if(time > lifetime)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        if(time > duration)
        {
            Destroy(gameObject);
        }

        material.SetFloat("_Phase", time);
    }
}
