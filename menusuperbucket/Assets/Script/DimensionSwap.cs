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

    [SerializeField] Color futureColor;
    [SerializeField] Color pastColor;

    [SerializeField] Color futureFogColor;
    [SerializeField] Color pastFogColor;

    [SerializeField] Material twirlMaterial;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        past = true;
        Prometeus.SetActive(!past);
        Past.SetActive(!past);
        Astralis.SetActive(past);
        Future.SetActive(past);
        past = !past;


        RenderSettings.fog = true;
        RenderSettings.ambientLight = futureColor;
        RenderSettings.fogEndDistance = 30;
        RenderSettings.fogColor = futureFogColor;
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
            RenderSettings.ambientLight = pastColor;
            RenderSettings.fogEndDistance = 300;
            RenderSettings.fogColor = pastFogColor;
        }
        else
        {
            PrometeusCam.Priority = 9;
            RenderSettings.ambientLight = futureColor;
            RenderSettings.fogEndDistance = 30;
            RenderSettings.fogColor = futureFogColor;
        }

        StartCoroutine(swapAnimation());
    }

    IEnumerator swapAnimation()
    {
        for(float t = 0; t < 1; t+=Time.deltaTime)
        {
            twirlMaterial?.SetFloat("_Twirl_Strength", - 10*t * (t - 1));
            yield return null;
        }
        twirlMaterial?.SetFloat("_Twirl_Strength", 0);

    }
}
