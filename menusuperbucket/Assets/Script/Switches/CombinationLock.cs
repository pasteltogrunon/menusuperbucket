using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationLock : MonoBehaviour
{
    [SerializeField] int[] correctCombination = new int[4];
    List<int> currentCombination = new List<int>();

    [SerializeField] GameObject[] activatingObjects;
    [SerializeField] GameObject[] deactivatingObjects;

    public ResetCombination resetCombination;

    public void proceed()
    {
        foreach (GameObject activatingObject in activatingObjects)
        {
            activatingObject.SetActive(true);
        }

        foreach (GameObject deactivatingObject in deactivatingObjects)
        {
            deactivatingObject.SetActive(false);
        }

        this.enabled = false;
    }

    public void activate(int num)
    {
        currentCombination.Add(num);
        if(currentCombination.Count == correctCombination.Length)
        {
            bool correct = true;
            for(int i=0; i < correctCombination.Length; i++)
            {
                if(currentCombination[i] != correctCombination[i])
                {
                    correct = false;
                    break;
                }
            }

            if(!correct)
            {
                StartCoroutine(resetAfter(1));
            }
            else
            {
                proceed();
            }
        }
    }

    IEnumerator resetAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        resetCombination?.Invoke();
        currentCombination.Clear();
    }

    public delegate void ResetCombination();
}
