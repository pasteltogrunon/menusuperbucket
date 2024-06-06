using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AllActiveEvent : MonoBehaviour
{
    [SerializeField] UnityEvent unityEvent;

    [SerializeField] int neededAmount = 6;

    int actual = 0;

    public void addToCount(int number)
    {
        actual += number;
        if(actual >= neededAmount)
        {
            unityEvent?.Invoke();
        }
    }


}
