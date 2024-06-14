using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    [SerializeField] AudioSource[] ambients = new AudioSource[2];
    [SerializeField] float crossFadeTime;

    AudioSource previousAudioSource;
    AudioSource targetAudioSource;

    [SerializeField] string[] zoneNames = { "Temple", "Forest", "Lake", "Cave" };
    [SerializeField] TMP_Text zoneShower;
    [SerializeField] float fadeOutTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }


    void Update()
    {

        if(previousAudioSource != null && previousAudioSource.volume > 0)
        {
            previousAudioSource.volume = Mathf.Clamp01(previousAudioSource.volume - Time.deltaTime / crossFadeTime);
            if(previousAudioSource.volume == 0)
            {
                previousAudioSource.Stop();
            }
        }

        if(targetAudioSource != null && targetAudioSource.volume < 1)
        {
            targetAudioSource.volume = Mathf.Clamp01(targetAudioSource.volume + Time.deltaTime / crossFadeTime);
        }

        if(zoneShower.color.a > 0)
        {
            setAlpha(zoneShower.color.a - Time.deltaTime / fadeOutTime);
        }

    }

    public void askForZone(int id)
    {
        if(previousAudioSource != null)
        {
            previousAudioSource.volume = 0;
        }
        previousAudioSource = targetAudioSource;
        targetAudioSource = ambients[id];
        targetAudioSource.Play();

        zoneShower.text = zoneNames[id];
        zoneShower.color = new Color(1, 1, 1, 1);
    }


    IEnumerator fadeOutAfter(AudioSource source, float time, float fadeOutDuration)
    {
        yield return new WaitForSeconds(time);
        for(float t = 0; t<fadeOutDuration; t+=Time.deltaTime)
        {
            setVolume(source, 1 - t / fadeOutDuration);
            yield return null;
        }

        source.Stop();
    }

    void setVolume(AudioSource source, float volume)
    {
        if(source != null)
        {
            source.volume = Mathf.Clamp01(volume);
        }
    }

    void setAlpha(float alpha)
    {
        zoneShower.color = new Color(1, 1, 1, Mathf.Clamp01(alpha));
    }
}