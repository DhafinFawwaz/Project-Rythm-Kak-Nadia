using System.Collections;
using System.Collections.Generic;
using DhafinFawwaz.Tweener;
using TMPro;
using UnityEngine;

public class WinAct : MonoBehaviour
{
    [SerializeField] TMP_Text _hitText;
    [SerializeField] TMP_Text _missText;
    [SerializeField] TMP_Text _menangText;

    [SerializeField] RythmGenerator _rythmGenerator;

    [Header("Tweener")]
    [SerializeField] ImageTweener _fadeIn;
    [SerializeField] ImageTweener _fadeOut;
    [SerializeField] TransformTweener _showTweener;
    [SerializeField] TransformTweener _hideTweener;

    [Header("Scene Transition")]
    [SerializeField] SceneTransition _sceneTransition;
    [SerializeField] GameObject _restart;
    [SerializeField] GameObject _continue;

    public void Show(int level)
    {
        _showTweener.LocalScale();
        _fadeIn.Color();
        _hitText.text = _rythmGenerator.HitCount.ToString();
        int hitableCount = _rythmGenerator.GetHitableCount(level);
        _missText.text = hitableCount - _rythmGenerator.HitCount + "";


        if(hitableCount < hitableCount/2) { // lose
            _restart.SetActive(true);
            _continue.SetActive(false);
            _menangText.text = "KALAH";
        } else {
            _restart.SetActive(false);
            _continue.SetActive(true);
            _menangText.text = "MENANG";
        }
    }


    public void Continue()
    {
        _hideTweener.LocalScale();
        _fadeOut.Color();
    }

    public void Home()
    {
        _sceneTransition.StartSceneTransition("MainMenu");
    }

    public void Restart()
    {
        _sceneTransition.StartSceneTransition(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
