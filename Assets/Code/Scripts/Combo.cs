using System.Collections;
using System.Collections.Generic;
using DhafinFawwaz.Tweener;
using TMPro;
using UnityEngine;

public class Combo : MonoBehaviour
{
    [SerializeField] CanvasGroupTweener _fade;
    [SerializeField] TransformTweener _scale;

    [SerializeField] TMP_Text _comboText;


    int _currentCombo = 0;
    int _maxCombo = 0;
    public int MaxCombo => _maxCombo;
    public void IncrementCombo()
    {
        _currentCombo++;
        _maxCombo = Mathf.Max(_currentCombo, _maxCombo);
        WinManager.Combo = _maxCombo;
        _comboText.text = _currentCombo.ToString();
        Show();
    }

    public void Hide()
    {
        _fade.SetEnd(0).Alpha();
        _scale.SetEnd(Vector3.zero).LocalScale();
        _currentCombo = 0;
    }

    public void Show()
    {
        transform.localScale = Vector3.one * 1.4f;
        GetComponent<CanvasGroup>().alpha = 0;
        _fade.SetEnd(1).Alpha();
        _scale.SetEnd(Vector3.one).LocalScale();
    }
}
