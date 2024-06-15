using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] AudioSource[] musics = new AudioSource[2];
    [SerializeField] AudioSource[] enemyMusics = new AudioSource[2];
    [SerializeField] AudioSource deathSource;
    [SerializeField] float crossFadeTime;
    [SerializeField] float enemyCrossFadeTime;
    bool dead = false;

    AudioSource previousAudioSource;
    AudioSource targetAudioSource;
    AudioSource enemyMusicSource;

    [Header("Enemy")]
    [SerializeField] Transform Astralis;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float musicRadius = 7;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }


    void Update()
    {
        if(!dead)
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

            if(enemyMusicSource != null)
            {
                if(Physics2D.OverlapCircle(Astralis.position, musicRadius, enemyLayer))
                {
                    enemyMusicSource.volume = Mathf.Clamp01(enemyMusicSource.volume + Time.deltaTime / enemyCrossFadeTime);
                }
                else
                {
                    enemyMusicSource.volume = Mathf.Clamp01(enemyMusicSource.volume - Time.deltaTime / enemyCrossFadeTime);
                }

                enemyMusicSource.time = Mathf.Min(targetAudioSource.time, enemyMusicSource.time);
            }

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
        enemyMusicSource?.Stop();
        targetAudioSource?.Play();

        if(id % 2 == 0)
        {
            //Astralis enemy themes
            enemyMusicSource = enemyMusics[id / 2];
            enemyMusicSource.volume = 0;
            enemyMusicSource.Play();
        }
        else
        {
            enemyMusicSource = null;
        }
    }

    public void deathMusic()
    {
        dead = true;
        setVolume(previousAudioSource, 0);
        setVolume(targetAudioSource, 0);
        setVolume(enemyMusicSource, 0);

        deathSource.Play();
        deathSource.volume = 1;

        StartCoroutine(fadeOutAfter(deathSource, 7, 7));
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

        if(source == deathSource)
        {
            dead = false;
        }
    }

    void setVolume(AudioSource source, float volume)
    {
        if(source != null)
        {
            source.volume = Mathf.Clamp01(volume);
        }
    }
}
