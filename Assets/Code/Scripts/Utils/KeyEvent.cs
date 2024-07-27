using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyEvent : MonoBehaviour
{
    [SerializeField] KeyCode _key;
    [SerializeField] UnityEvent _onKeyDown;
    void Update()
    {
        if(Input.GetKeyDown(_key))
        {
            _onKeyDown?.Invoke();
        }
    }
}
