using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionSwap : MonoBehaviour
{
    public static DimensionSwap Instance;

    bool past = false;
    public CinemachineVirtualCamera activeCamera;

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

    [HideInInspector] public Vector2 AstralisSpawnPos;
    [HideInInspector] public Vector2 PrometeusSpawnPos;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        past = true;
        RenderSettings.fog = true;

        AstralisSpawnPos = Astralis.transform.position;
        PrometeusSpawnPos = Prometeus.transform.position;

        swapDimensionManagement();
    }

    void Update()
    {

    }

    public void swapDimension(Transform shrine)
    {
        if(past)
        {
            PrometeusSpawnPos = shrine.position;
        }
        else
        {
            AstralisSpawnPos = shrine.position;
        }

        StartCoroutine(swapAnimation());

    }

    IEnumerator swapAnimation()
    {
        InputManager.CinematicInputsLocked = true;
        CinemachineComponentBase componentBase = activeCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (activeCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) is CinemachineFramingTransposer)
        {
            (componentBase as CinemachineFramingTransposer).m_CameraDistance = 4;
        }
        for (float t = 0; t < 0.5f; t+=Time.deltaTime)
        {
            twirlMaterial?.SetFloat("_Twirl_Strength", - 10*t * (t - 1));
            yield return null;
        }

        (componentBase as CinemachineFramingTransposer).m_CameraDistance = 10;

        swapDimensionManagement();

        for (float t = 0.5f; t <1; t += Time.deltaTime)
        {
            twirlMaterial?.SetFloat("_Twirl_Strength", -10 * t * (t - 1));
            yield return null;
        }
        twirlMaterial?.SetFloat("_Twirl_Strength", 0);
        InputManager.CinematicInputsLocked = false;
    }

    void swapDimensionManagement()
    {
        Prometeus.SetActive(!past);
        Past.SetActive(!past);
        Astralis.SetActive(past);
        Future.SetActive(past);
        past = !past;

        if (past)
        {
            //To Prometheus
            PrometeusCam.Priority = 11;
            activeCamera = PrometeusCam;
            RenderSettings.ambientLight = pastColor;
            RenderSettings.fogEndDistance = 300;
            RenderSettings.fogColor = pastFogColor;
            Prometeus.GetComponent<HealthManager>().Health = Prometeus.GetComponent<HealthManager>().MaxHealth;
        }
        else
        {
            //To Astralis
            PrometeusCam.Priority = 9;
            activeCamera = AstralisCam;
            RenderSettings.ambientLight = futureColor;
            RenderSettings.fogEndDistance = 30;
            RenderSettings.fogColor = futureFogColor;
            Astralis.GetComponent<HealthManager>().Health = Astralis.GetComponent<HealthManager>().MaxHealth;
        }

    }
}
