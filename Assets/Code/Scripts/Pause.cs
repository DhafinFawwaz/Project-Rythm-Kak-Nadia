using System.Collections;
using System.Collections.Generic;
using DhafinFawwaz.Tweener;
using Unity.VisualScripting;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] ImageTweener _fadeIn;
    [SerializeField] ImageTweener _fadeOut;
    [SerializeField] TransformTweener _scaleIn;
    [SerializeField] TransformTweener _scaleOut;
    [SerializeField] RythmGenerator _rythmGenerator;

    public void PauseGame()
    {
        _fadeIn.Color();
        _scaleIn.LocalScale();
        _rythmGenerator.Pause();
    }

    public void ResumeGame()
    {
        _fadeOut.Color();
        _scaleOut.LocalScale();
        _rythmGenerator.Resume();
    }
}
