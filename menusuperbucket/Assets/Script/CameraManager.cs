using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    public static void cameraShake(float duration, float amplitude, float frequency)
    {
        Instance.StartCoroutine(Instance.doCameraShake(duration, amplitude, frequency));
    }

    IEnumerator doCameraShake(float duration, float amplitude, float frequency)
    {
        var camera = (CinemachineVirtualCamera)CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitude;
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequency;
        yield return new WaitForSeconds(duration);
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
    }
}
