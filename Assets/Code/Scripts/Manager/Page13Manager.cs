using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Page13Manager : MonoBehaviour
{
    [SerializeField] SpriteAnimator _babiAnimator;
    [SerializeField] GameObject[] _state;
    int _stateIndex = 0;


    public void NextState()
    {
        _state[_stateIndex].SetActive(false);
        _stateIndex++;
        if(_stateIndex >= _state.Length) return;
        _state[_stateIndex].SetActive(true);
    }

    [SerializeField] GameObject[] _ubiList;
    int _ubiIndex = 0;

    public void NextUbi()
    {
        _babiAnimator.Play();
        _ubiList[_ubiIndex].SetActive(false);
        _ubiIndex++;
        if(_ubiIndex >= _ubiList.Length) 
        {
            NextState();
            return;
        }
        _ubiList[_ubiIndex].SetActive(true);
    }

    [SerializeField] GameObject[] _apelList;
    int _apelIndex = 0;

    public void NextApel()
    {
        if(_apelIndex == _apelList.Length) return;
        _babiAnimator.Play();
        _apelList[_apelIndex].SetActive(false);
        _apelIndex++;
        if(_apelIndex >= _apelList.Length) return;
        _apelList[_apelIndex].SetActive(true);

        if(_apelIndex == _apelList.Length - 1) _onFinish?.Invoke();
    }
    [SerializeField] UnityEvent _onFinish;
}
