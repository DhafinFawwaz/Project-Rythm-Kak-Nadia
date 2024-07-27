using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnAwake : MonoBehaviour
{
    [SerializeField] UnityEvent _onAwake;
    [SerializeField] UnityEvent _onEnable;
    [SerializeField] UnityEvent _onDisable;
    [SerializeField] float _delay = 0;
    void Awake()
    {
        if(_delay > 0) Invoke("OnAwakeDelayed", _delay);
        else _onAwake?.Invoke();
    }

    void OnAwakeDelayed()
    {
        _onAwake?.Invoke();
    }

    void OnEnable()
    {
        if(_delay > 0) Invoke("OnEnableDelayed", _delay);
        else _onEnable?.Invoke();
    }

    void OnEnableDelayed()
    {
        _onEnable?.Invoke();
    }


    void OnDisable()
    {
        if(_delay > 0) Invoke("OnDisableDelayed", _delay);
        else _onDisable?.Invoke();
    }

    void OnDisableDelayed()
    {
        _onDisable?.Invoke();
    }
}
