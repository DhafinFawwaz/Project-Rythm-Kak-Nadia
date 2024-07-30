using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DhafinFawwaz.Tweener;

public class LevelManager : MonoBehaviour
{
    [SerializeField] SlidingWindow _slidingWindow;
    [SerializeField] SceneTransition _sceneTransition;
    [SerializeField] ImageTweener[] _pageFade;
    public void LoadGame()
    {
        if(_slidingWindow.Page != 0) return;
        _sceneTransition.StartSceneTransition(_slidingWindow.Page.ToString());
    }

    public void ToMainMenu()
    {
        _sceneTransition.StartSceneTransition("MainMenu");
    }

    public void OnPageChange(int page)
    {
        for(int i = 0; i < _pageFade.Length; i++)
        {
            if(i == page)
            {
                _pageFade[i].SetEndColor(new Color(1, 1, 1, 1)).Color();
            } else {
                _pageFade[i].SetEndColor(new Color(1, 1, 1, 0)).Color();
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToMainMenu();
        }
    }
    
}
