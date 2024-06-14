using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;

    void changeVolume(float fillAmount, string parameterName)
    {
        float value = fillAmount == 0 ? -100f : 20.0f * Mathf.Log10(fillAmount);
        mixer.SetFloat(parameterName, value);
    }

    public void changeMusicVolume(float fillAmount)
    {
        changeVolume(fillAmount, "MusicVolume");
    }

    public void changeAmbientVolume(float fillAmount)
    {
        changeVolume(fillAmount, "AmbientVolume");
    }

    public void changeEffectsVolume(float fillAmount)
    {
        changeVolume(fillAmount, "EffectsVolume");
    }
}
