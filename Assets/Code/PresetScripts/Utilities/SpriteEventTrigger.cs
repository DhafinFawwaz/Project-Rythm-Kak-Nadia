using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteEventTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent _onMouseDown;

    void OnMouseDown()
    {
        _onMouseDown?.Invoke();
    }
}
