using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteDragDrop : MonoBehaviour
{
    enum DragDropState { None, Enter, Down, Exit }
    DragDropState _state = DragDropState.None;
    Vector3 _initialPosition;
    Vector3 _offset;
    bool _isDragging = false;
    public bool IsDragging => _isDragging;
    [SerializeField] Vector3 _normalScale = Vector3.one;
    [SerializeField] Vector3 _hoverScale = Vector3.one * 1.2f;
    [SerializeField] Vector3 _downScale = Vector3.one * 1.4f;
    [SerializeField] float _tweenDuration = 0.15f;


    public UnityEvent OnDrop;

    void Awake()
    {
        _initialPosition = transform.position;
    }

#if UNITY_EDITOR
    void Reset()
    {
        OnDrop = new UnityEvent();
        UnityEditor.Events.UnityEventTools.AddFloatPersistentListener(OnDrop, GoToInitialPosition, 0.5f);
    }
#endif

    void OnMouseEnter()
    {
        if(!enabled) return;
        _state = DragDropState.Enter;
        StartCoroutine(TweenLocalScale(_hoverScale, _tweenDuration));
    }

    void OnMouseExit()
    {
        if(!enabled) return;
        _state = DragDropState.Exit;
        StartCoroutine(TweenLocalScale(_normalScale, _tweenDuration));
    }

    void OnMouseDown()
    {
        if(!enabled) return;
        CancelTweenPosition();
        StartCoroutine(TweenLocalScale(_downScale, _tweenDuration));
        _state = DragDropState.Down;
        _offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _isDragging = true;
    }

    void OnMouseUp()
    {
        if(!enabled) return;
        if(_state == DragDropState.Down)
            StartCoroutine(TweenLocalScale(_hoverScale, _tweenDuration));
        else
            StartCoroutine(TweenLocalScale(_normalScale, _tweenDuration));
        _isDragging = false;
        OnDrop?.Invoke();
    }

    void OnMouseDrag()
    {
        if(!enabled) return;
        if(_isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x + _offset.x, mousePos.y + _offset.y, transform.position.z);
        }
    }

    public void GoToInitialPosition(float duration = 0.5f)
    {
        StartCoroutine(TweenPosition(_initialPosition, duration));
    }


    public void CancelTweenPosition() => ++_posKey;
    byte _posKey = 0;
    IEnumerator TweenPosition(Vector3 end, float duration)
    {
        byte requirement = ++_posKey;
        Vector3 start = transform.position;
        float t = 0;
        while(t < 1 && requirement == _posKey)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, end, OutQuart(t));
            yield return null;
        }
        if(requirement == _posKey)
        {
            transform.position = end;
        }
    }

    public void CancelTweenLocalScale() => ++_scaleKey;
    byte _scaleKey = 0;
    IEnumerator TweenLocalScale(Vector3 end, float duration)
    {
        byte requirement = ++_scaleKey;
        Vector3 start = transform.localScale;
        float t = 0;
        while(t < 1 && requirement == _scaleKey)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.LerpUnclamped(start, end, OutBackCubic(t));
            yield return null;
        }
        if(requirement == _scaleKey)
        {
            transform.localScale = end;
        }
    }
    static float OutQuart(float x) => -((1-x)*(1-x)*(1-x)*(1-x)) + 1;
    static float OutBackCubic(float x) => -(x-1)*(x-1)*(x-1)*(x-1) 
        + 3*(x-1)*(x-1)*(x-1) + 3*(x-1)*(x-1) + 1;

}
