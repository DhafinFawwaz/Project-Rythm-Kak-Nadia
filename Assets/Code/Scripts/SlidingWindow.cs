using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlidingWindow : MonoBehaviour
{
    bool _isDragging = false;
    [SerializeField] float _speed = 0.5f;
    [SerializeField] float _pageLength = 0.1088f;
    [SerializeField] int _maxPage = 2;
    int _page = 0;
    public int Page => -_page;
    [SerializeField] Vector2 _mouseClickAreaViewport = new Vector2(0.9f, 0.9f);
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3[] corners = new Vector3[4];

        if(_mainCam == null) _mainCam = Camera.main;
        corners[0] = _mainCam.ViewportToWorldPoint(new Vector3(_mouseClickAreaViewport.x, _mouseClickAreaViewport.y, 0));
        corners[1] = _mainCam.ViewportToWorldPoint(new Vector3(1-_mouseClickAreaViewport.x, _mouseClickAreaViewport.y, 0));
        corners[2] = _mainCam.ViewportToWorldPoint(new Vector3(1-_mouseClickAreaViewport.x, 1-_mouseClickAreaViewport.y, 0));
        corners[3] = _mainCam.ViewportToWorldPoint(new Vector3(_mouseClickAreaViewport.x, 1-_mouseClickAreaViewport.y, 0));

        for(int i = 0; i < 4; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i+1)%4]);
        }
    }

    Camera _mainCam;
    void Awake()
    {
        _mainCam = Camera.main;
    }

    bool _isMouseInClickArea {get {
        Vector2 mousePos = _mainCam.ScreenToViewportPoint(Input.mousePosition);
        return mousePos.x > 1-_mouseClickAreaViewport.x && mousePos.x < _mouseClickAreaViewport.x && mousePos.y > 1-_mouseClickAreaViewport.y && mousePos.y < _mouseClickAreaViewport.y;
    }}

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && _isMouseInClickArea)
        {
            _isDragging = true;
            OnDragStart();
        }


        if(Input.GetMouseButtonUp(0) && _isDragging)
        {
            _isDragging = false;
            OnDragEnd();
        }
        if(_isDragging)
        {
            transform.position += new Vector3(Input.GetAxis("Mouse X")*_speed, 0, 0);
        }

        if(transform.localPosition.x > 0)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }

        if(transform.localPosition.x < -_pageLength*(_maxPage-1))
        {
            transform.localPosition = new Vector3(-_pageLength*(_maxPage-1), 0, 0);
        }
    }

    void OnDragStart()
    {
        _key++;
    }

    [SerializeField] UnityEvent<int> _onPageChange;
    public void GoToPage(int page)
    {
        _page = page;
        StartCoroutine(TaskLocalPosition(transform, transform.localPosition, new Vector3(_page*_pageLength, 0, 0), 0.35f, Ease.OutQuart));
        _onPageChange?.Invoke(-_page);
    }

    public void NextPage()
    {
        if(-_page < _maxPage-1)
        {
            GoToPage(_page-1);
        }
    }

    public void PrevPage()
    {
        if(-_page > 0)
        {
            GoToPage(_page+1);
        }
    }

    void OnDragEnd()
    {
        float landedPage = transform.localPosition.x/_pageLength;
        _page = Mathf.RoundToInt(landedPage);
        _onPageChange?.Invoke(-_page);
        StartCoroutine(TaskLocalPosition(transform, transform.localPosition, new Vector3(_page*_pageLength, 0, 0), 0.35f, Ease.OutQuart));
    }


    byte _key = 0;
    IEnumerator TaskLocalPosition(Transform trans, Vector3 start, Vector3 end, float duration, Ease.Function easeFunction)
    {
        byte requirement = ++_key;
        float startTime = Time.time;
        float t = (Time.time-startTime)/duration;
        while (t <= 1 && requirement == _key)
        {
            t = Mathf.Clamp((Time.time-startTime)/duration, 0, 2);
            trans.localPosition = Vector3.LerpUnclamped(start, end, easeFunction(t));
            yield return null;
        }
        trans.localPosition = end;
    }
    
}
