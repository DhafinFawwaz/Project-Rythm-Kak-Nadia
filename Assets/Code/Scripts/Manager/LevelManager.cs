using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] SlidingWindow _slidingWindow;
    [SerializeField] SceneTransition _sceneTransition;
    public void LoadGame()
    {
        if(_slidingWindow.Page != 0) return;
        _sceneTransition.StartSceneTransition(_slidingWindow.Page.ToString());
    }

    public void ToMainMenu()
    {
        _sceneTransition.StartSceneTransition("MainMenu");
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToMainMenu();
        }
    }
    
}
