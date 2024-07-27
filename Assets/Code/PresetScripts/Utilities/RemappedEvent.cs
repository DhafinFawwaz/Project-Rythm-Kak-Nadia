using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RemappedEvent : MonoBehaviour
{
    [SerializeField] float _min = 0;
    [SerializeField] float _max = 5;
    [SerializeField] float _remappedMin = 0;
    [SerializeField] float _remappedMax = 1;

    public float Remap(float value)
    {
        return Mathf.Lerp(_remappedMin, _remappedMax, Mathf.InverseLerp(_min, _max, value));
    }

    [SerializeField] UnityEvent<float> _event;

    public void Invoke(float value)
    {
        _event?.Invoke(Remap(value));
    }
}
