using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpriteDropReceiver : MonoBehaviour
{
    enum DropReceiverState { None, Enter }
    DropReceiverState _state = DropReceiverState.None;
    [SerializeField] Transform _target;
    [SerializeField] Vector3 _normalScale = Vector3.one;
    [SerializeField] Vector3 _enterScale = Vector3.one * 1.1f;
    [SerializeField] float _tweenDuration = 0.25f;
    [SerializeField] UnityEvent<SpriteDragDrop> _onDrop;
    SpriteDragDrop _dragDrop;

    void StartCoroutineIfActive(IEnumerator routine)
    {
        if(gameObject.activeInHierarchy) StartCoroutine(routine);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent(out SpriteDragDrop dragDrop) && dragDrop.IsDragging)
        {
            _dragDrop = dragDrop;
            _state = DropReceiverState.Enter;
            StartCoroutineIfActive(TweenLocalScale(_enterScale, _tweenDuration));
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        _dragDrop = null;
        _state = DropReceiverState.None;
        StartCoroutineIfActive(TweenLocalScale(_normalScale, _tweenDuration));
    }


    bool _isTouchUp => Input.GetMouseButtonUp(0) 
    || (Input.touchCount >= 1 && Input.GetTouch(0).phase == TouchPhase.Ended);
    void Update()
    {
        if(_isTouchUp && _state == DropReceiverState.Enter)
        {
            if(_dragDrop != null) 
            {
                _onDrop?.Invoke(_dragDrop);
                _dragDrop = null;
                _state = DropReceiverState.None;
                StartCoroutineIfActive(TweenLocalScale(_normalScale, _tweenDuration));
            }
        }
    }

    void CancelTweenLocalScale() => ++_scaleKey;
    byte _scaleKey = 0;
    IEnumerator TweenLocalScale(Vector3 end, float duration)
    {
        byte requirement = ++_scaleKey;
        Vector3 start = _target.localScale;
        float t = 0;
        while(t < 1 && requirement == _scaleKey)
        {
            t += Time.deltaTime / duration;
            _target.localScale = Vector3.LerpUnclamped(start, end, OutBackCubic(t));
            yield return null;
        }
        if(requirement == _scaleKey)
        {
            _target.localScale = end;
        }
    }
    static float OutQuart(float x) => -((1-x)*(1-x)*(1-x)*(1-x)) + 1;
    static float OutBackCubic(float x) => -(x-1)*(x-1)*(x-1)*(x-1) 
        + 3*(x-1)*(x-1)*(x-1) + 3*(x-1)*(x-1) + 1;
}
