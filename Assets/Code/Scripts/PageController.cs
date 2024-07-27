using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class PageController : MonoBehaviour
{
    int _currentPage = -1;
    int _pageAmount = 0;

    public UnityEvent OnNextInLastPage;

    [SerializeField] VideoPlayer[] _videoPlayers;

    [SerializeField] SceneTransition _sceneTransition;
    [SerializeField] int[] _delayedPages;
    bool _isDelayFinished;
    public void SetDelayFinished(bool isFinished)
    {
        _isDelayFinished = isFinished;
    }

    public Action<int> OnPageDelayed;

    void Awake()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        while(_videoPlayers.Any(x => !x.isPrepared))
        {
            yield return new WaitForSecondsRealtime(0.3f);
        }

        StartCoroutine(_sceneTransition.InAnimation());

        _pageAmount = transform.childCount;
        for (int i = _pageAmount-1; i >= 0; i--)
        {
            if(transform.GetChild(i).gameObject.activeInHierarchy)
            {
                _currentPage = i;
                break;
            }
        }
        if(_currentPage == -1)
        {
            _currentPage = 0;
            transform.GetChild(_currentPage).gameObject.SetActive(true);
        }
        yield return null;
    }

    public void NextPage()
    {
        if (_currentPage < _pageAmount - 1)
        {
            if(_delayedPages.Contains(_currentPage+1))
            {
                StartCoroutine(DelayedNextPage(_currentPage+1));
                return;
            }
            transform.GetChild(_currentPage).gameObject.SetActive(false);
            _currentPage++;
            transform.GetChild(_currentPage).gameObject.SetActive(true);
        }
        else
        {
            OnNextInLastPage?.Invoke();
        }
    }

    IEnumerator DelayedNextPage(int page)
    {
        _isDelayFinished = false;
        OnPageDelayed?.Invoke(page);
        while(!_isDelayFinished) yield return null;
        
        transform.GetChild(_currentPage).gameObject.SetActive(false);
        _currentPage = page;
        transform.GetChild(_currentPage).gameObject.SetActive(true);
    }

    public void PreviousPage()
    {
        if (_currentPage > 0)
        {
            if(_delayedPages.Contains(_currentPage-1))
            {
                StartCoroutine(DelayedNextPage(_currentPage-1));
                return;
            }

            transform.GetChild(_currentPage).gameObject.SetActive(false);
            _currentPage--;
            transform.GetChild(_currentPage).gameObject.SetActive(true);
        }
    }
}
