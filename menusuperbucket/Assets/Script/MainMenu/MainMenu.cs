using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Image blackScreen;
    [SerializeField] Image mythicalChicken;
    [SerializeField] Image mythicalChicken2;
    [SerializeField] AudioSource MainMenuMusic;
    [SerializeField] AudioSource Pco;

    [SerializeField] AnimationCurve curve;

    bool transitioning;

    public void Start()
    {
        StartCoroutine(startMainMenu());
    }

    public void LoadGame()
    {
        if(!transitioning)
            StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator startMainMenu()
    {
        blackScreen.gameObject.SetActive(true);
        mythicalChicken.gameObject.SetActive(true);

        Material mat = mythicalChicken.material;
        Pco.Play();
        mat.SetFloat("_Phase", 0);

        for(float t = 0; t < 4f; t += Time.deltaTime)
        {
            mythicalChicken.color = new Color(1, 1, 1, Mathf.Clamp01( t * 2));
            mythicalChicken.transform.localScale = (1 + 0.2f * curve.Evaluate(t/4)) * Vector3.one * 0.2f;
            yield return null;
        }
        yield return new WaitForSeconds(0.6f);

        mythicalChicken.transform.localScale = Vector3.one * 0.2f;
        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            mat.SetFloat("_Phase", t*5);
            yield return null;
        }
        mat.SetFloat("_Phase", 1);

        mythicalChicken2.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(2f);

        mythicalChicken.gameObject.SetActive(false);


        MainMenuMusic.Play();
        for (float t = 0; t < 2; t += Time.deltaTime)
        {
            blackScreen.color = new Color(0, 0, 0, 1-t * 0.5f);
            MainMenuMusic.volume = t * 0.5f;
            yield return null;
        }
    }

    IEnumerator LoadYourAsyncScene()
    {
        transitioning = true;

        for (float t = 0; t < 2; t += Time.deltaTime)
        {
            blackScreen.color = new Color(0, 0, 0, t*0.5f);
            MainMenuMusic.volume = 1-(t * 0.5f);
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        transitioning = false;
    }
}
