using System.Collections;
using System.Collections.Generic;
using DhafinFawwaz.Tweener;
using TMPro;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public static int HitCount = 0;
    public static int Combo = 0;
    public static bool IsWin = true;

    [SerializeField] TMP_Text _menangKalahText;

    [SerializeField] SceneTransition _sceneTransition;

    [SerializeField] TextMeshProTweener _hitTextTweener;
    [SerializeField] TextMeshProTweener _comboTextTweener;
    void Awake()
    {
        if(IsWin)
        {
            _menangKalahText.text = "Menang";
        } else {
            _menangKalahText.text = "Kalah";
        }
        _hitTextTweener.SetStart(0).SetEnd(HitCount).Numeric();
        _comboTextTweener.SetStart(0).SetEnd(Combo).Numeric();

        HitCount = 0;
        Combo = 0;
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
