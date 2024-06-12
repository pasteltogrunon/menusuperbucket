using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] AudioSource[] musics = new AudioSource[2];
    [SerializeField] float crossFadeTime;
    float timer;

    [SerializeField] AudioSource previousAudioSource;
    [SerializeField] AudioSource targetAudioSource;


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
    }

    public void askForTrack(int id)
    {
        if(previousAudioSource != null)
        {
            previousAudioSource.volume = 0;
        }
        previousAudioSource = targetAudioSource;
        targetAudioSource = musics[id];
        targetAudioSource.Play();
    }
}
