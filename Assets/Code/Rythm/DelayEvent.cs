using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{

    [SerializeField] UnityEvent _onDelayFinished;


    public void Call(float delay)
    {
        StartCoroutine(Delay(delay));
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _onDelayFinished?.Invoke();
    }
}
