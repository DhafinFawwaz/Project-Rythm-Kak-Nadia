using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputEventTrigger : MonoBehaviour
{
    [SerializeField] KeyCode _keyCode;
    [SerializeField] UnityEvent _onKeyDown;
    [SerializeField] UnityEvent _onKeyUp;

    void Update()
    {
        if(Input.GetKeyDown(_keyCode))
        {
            _onKeyDown.Invoke();
        }
        if(Input.GetKeyUp(_keyCode))
        {
            _onKeyUp.Invoke();
        }
    }
}
