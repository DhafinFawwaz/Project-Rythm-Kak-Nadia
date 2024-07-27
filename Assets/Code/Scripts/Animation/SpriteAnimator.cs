using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] _sprites;
    [SerializeField] float _durationEachFrame = 0.03f;
    [SerializeField] bool _loop = true;

    SpriteRenderer _rend;
    int _currentIndex = 0;
    [SerializeField] bool _playOnEnable = true;
    Coroutine _coroutine;

    void OnEnable()
    {
        _currentIndex = 0;
        _rend = GetComponent<SpriteRenderer>();
        if(_playOnEnable) StartCoroutine(Animate());
    }

    void OnDisable()
    {
        if(_coroutine != null) StopCoroutine(_coroutine);
    }

    public void Play()
    {
        if(_rend == null) _rend = GetComponent<SpriteRenderer>();
        SetSpriteIndex(0);
        if(_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Animate());
    }

    public void SetSpriteIndex(int index)
    {
        if(_rend == null) _rend = GetComponent<SpriteRenderer>();
        if(index < 0 || index >= _sprites.Length) throw new System.Exception("Index out of range");

        _currentIndex = index;
        _rend.sprite = _sprites[_currentIndex];
    }

    IEnumerator Animate()
    {
        while(true)
        {
            if(_currentIndex >= _sprites.Length) continue;

            _rend.sprite = _sprites[_currentIndex];
            yield return new WaitForSeconds(_durationEachFrame);
            _currentIndex++;
            if(_currentIndex >= _sprites.Length)
            {
                if(_loop)
                {
                    _currentIndex = 0;
                }
                else
                {
                    if(_coroutine != null) StopCoroutine(_coroutine);
                    break;
                }
            }
        }
    }
}
