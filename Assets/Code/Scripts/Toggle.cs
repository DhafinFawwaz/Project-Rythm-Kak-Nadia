using System.Collections;
using System.Collections.Generic;
using DhafinFawwaz.Tweener;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    [SerializeField] RectTransformTweener _onTween;
    [SerializeField] RectTransformTweener _offTween;

    bool _isOn = true;
    void Awake()
    {
        _isOn = PlayerPrefs.GetInt("Hint", 1) == 1 ? true : false;
        float onTemp = _onTween.Duration;
        float offTemp = _offTween.Duration;
        if(_isOn) _onTween.LocalPosition();
        else _offTween.LocalPosition();

        _onTween.Duration = onTemp;
        _offTween.Duration = offTemp;
    }
    public void ToggleHint()
    {
        _isOn = !_isOn;
        PlayerPrefs.SetInt("Hint", _isOn ? 1 : 0);
        if(_isOn) _onTween.LocalPosition();
        else _offTween.LocalPosition();
    }
}
