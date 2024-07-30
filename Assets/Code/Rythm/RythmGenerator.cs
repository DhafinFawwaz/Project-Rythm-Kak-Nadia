using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class RythmGenerator : MonoBehaviour
{
    // Edit from 
    // https://docs.google.com/spreadsheets/d/1XRB83i0WHEzWbfra_RHFmiSnqaDigQL_XxclPv1AxdM/edit?gid=0#gid=0
    [SerializeField] string _apiURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQ6mmXZ_UJPAVFO2kA227Ij4dP982UMHeVhAXs5J6BxDTQxJn0eTrIR-K_qAqroNPVooe58RTMeTYBH/pub?output=csv";
    RythmReader _reader;
    
    [SerializeField] LineRenderer _linePrefab;
    [SerializeField] Transform[] _lanes;
    
    [Header("Data")]
    [SerializeField] float _earlyTolerance = 1f;
    [SerializeField] float _lateTolerance = 1f;
    [SerializeField] float _winTolerance = 3f;

    [SerializeField] float _speedMultiplier = 1;
    [SerializeField, TextArea] string _rawData;
    [SerializeField] bool _useEditorData = false;
    [SerializeField] Rythm[] _rythm;

    [Header("Win ")]
    [SerializeField] float[] _winTime = new float[3];
    bool[] _isWin = new bool[3];
    [SerializeField] UnityEvent<int> _onWin;
    [SerializeField] UnityEvent _onWinAll;

    [Header("Result")]
    [SerializeField] int _hitCount;
    public int HitCount => _hitCount;
    [SerializeField] int _wrongCount;
    public int WrongCount => _wrongCount;
    // [SerializeField] int _missCount;
    public int GetHitableCount(int level) {
        int count = 0;

        if(level == 0) {
            foreach (var rythm in _rythm)
                if(rythm.Seconds < _winTime[level]) count++;
        } else if(level == 1) {
            foreach (var rythm in _rythm)
                if(rythm.Seconds >= _winTime[level-1] && rythm.Seconds <= _winTime[level])
                    count++;
        } else if(level == 2) {
            foreach (var rythm in _rythm)
                if(rythm.Seconds >= _winTime[level-1])
                    count++;
        }

        return count;
    }


    [SerializeField] UnityEvent<Rythm[], float> _onRythmGenerated;
    void Start()
    {
        _videoTimeOffset = PlayerPrefs.GetFloat("VideoTimeOffset", 0);
        _reader = new RythmReader(_apiURL);
        GenerateRythm();
    }


    [Header("Settings Timing")]
    [SerializeField] float _minInput = 0;
    [SerializeField] float _maxInput = 20;
    [SerializeField] float _minOutput = -2;
    [SerializeField] float _maxOutput = 2;
    float remap(float value, float from1, float to1, float from2, float to2)
        => (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    public void SetVideoTimeOffset(float timeOffset)
    {
        timeOffset = remap(timeOffset, _minInput, _maxInput, _minOutput, _maxOutput);
        PlayerPrefs.SetFloat("VideoTimeOffset", timeOffset);
        _videoTimeOffset = timeOffset;
    }


    [SerializeField] List<LineRenderer> _lineList = new List<LineRenderer>();
    
    public async void GenerateRythm()
    {

        foreach (var line in _lineList) Destroy(line.gameObject);
        _lineList.Clear();

        
        if(_useEditorData) {
        }
        else {
            _rythm = await _reader.ReadRythm();
            _rawData = _reader.RawData;
            _speedMultiplier = int.Parse(_reader.GetRow(1)[3]);
        }

        
        for(int i = 0; i < _rythm.Length; i++)
        {
            float seconds = _rythm[i].Seconds;
            int lane = _rythm[i].Lane;
            float length = _rythm[i].Length;

            LineRenderer line = Instantiate(_linePrefab, _lanes[lane]);
            line.transform.localPosition = new Vector3(0, seconds * _speedMultiplier, 0);
            line.SetPosition(1, new Vector3(0, length * _speedMultiplier, 0));
            _lineList.Add(line);
        }

        _onRythmGenerated?.Invoke(_rythm, _speedMultiplier);
    }

    [SerializeField] double _elapsedTime;
    [SerializeField] float _offsetTime = 0.2f;
    Coroutine _moveCameraCoroutine;
    [ContextMenu("StartMovingCamera")]
    public void StartMovingCamera()
    {
        if(_moveCameraCoroutine != null) StopCoroutine(_moveCameraCoroutine);
        _moveCameraCoroutine = StartCoroutine(MoveCamera());
    }

    [SerializeField] float _videoTimeOffset = -0.1f;
    IEnumerator MoveCamera()
    {
        Transform cam = Camera.main.transform;
        while (true)
        {
            // _elapsedTime += Time.deltaTime;
            _elapsedTime = _videoPlayer.time + _videoTimeOffset;
            // if(!_audioSource.isPlaying && _elapsedTime >= 0) _audioSource.Play();
            
            cam.position = new Vector3(cam.position.x, (float)(_elapsedTime+_offsetTime) * _speedMultiplier, cam.position.z);
            if(!_updateSliderByDragging) UpdateSlider();
            
            // Check if all rythm has passed
            // if(_elapsedTime > _rythm[_rythm.Length-1].Seconds + _rythm[_rythm.Length-1].Length + _winTolerance) break;
            

            // Combo check
            UpdateCurrentRythm();


            // win check
            for(int i = 0; i < _winTime.Length; i++)
            {
                if(_isWin[i]) continue;
                if(_elapsedTime > _winTime[i])
                {
                    if(i == 2) // hard coded here
                    {
                        _isWin[i] = true;
                        int hitCount = 0;
                        foreach (var rythm in _rythm)
                        {
                            if(rythm.IsClicked) hitCount++;
                        }
                        int hitableCount = GetHitableCount(2);
                        if(hitableCount < hitableCount/2) { // lose
                            WinManager.IsWin = true;
                        } else {
                            WinManager.IsWin = false;
                        }
                        WinManager.HitCount = hitCount;
                        _onWinAll?.Invoke();
                        
                    } else {
                        _isWin[i] = true;
                        _onWin?.Invoke(i);
                        _hitCount = 0;
                        _wrongCount = 0;
                    }

                }
            }

            yield return null;
            if(_pause) yield return new WaitUntil(() => !_pause);
        }

    }

    [SerializeField] LineRenderer _currentRythm;
    [SerializeField] int _currentRythmIndex = 0;

    [SerializeField] float _missYTolerance = 1;
    void UpdateCurrentRythm()
    {
        if(_currentRythm == null) _currentRythm = FindNextNearestRythm(out _currentRythmIndex);
        float clickableY = _rythmClickables[0].transform.position.y;
        if(_currentRythm.transform.position.y < clickableY+_missYTolerance)
        {
            if(!_rythm[_currentRythmIndex].IsClicked) {
                _onMiss?.Invoke();
            }
            _currentRythm = FindNextNearestRythm(out _currentRythmIndex);
        }
    }

    void OnDrawGizmos()
    {
        if(_currentRythm == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_currentRythm.transform.position, 1f);
    }

    [SerializeField] UnityEvent _onMiss;
    [SerializeField] UnityEvent _onHit;
    LineRenderer FindNextNearestRythm(out int index)
    {
        index = 0;
        float clickableY = _rythmClickables[0].transform.position.y;
        
        float minDistance = float.MaxValue;

        int currentIndex = 0;
        foreach(var line in _lineList)
        {
            float diff = line.transform.position.y - clickableY;
            if(diff < 0) {
                currentIndex++;
                continue;
            }
            if(diff < minDistance)
            {
                minDistance = diff;
                _currentRythm = line;
                index = currentIndex;
            }

            currentIndex++;
        }

        return _currentRythm;
    }

    // Pause
    bool _pause = false;
    public void Pause()
    {
        _pause = true;
        _videoPlayer.Pause();
    }
    public void Resume()
    {
        _pause = false;
        _videoPlayer.Play();
    }
    //


    [Header("Debug")]
    [SerializeField] Image[] _rythmClickables;
    public void PointerDownLane(int lane)
    {
        // Find inside which rythm we are
        Rythm currentRythm = null;
        bool isHit = false;
        foreach (var rythm in _rythm)
        {
            if (rythm.Lane != lane) continue;

            // Case inside any part of the rythm including the unclickable part
            if ((_elapsedTime+_offsetTime) > rythm.Seconds && (_elapsedTime+_offsetTime) < rythm.Seconds + rythm.Length)
            {
                currentRythm = rythm;
                // if(currentRythm.IsClicked) return; // Already clicked
            }

            // Case earlier than the rythm
            if ((_elapsedTime+_offsetTime) < rythm.Seconds - _earlyTolerance/_speedMultiplier)
            {
                isHit = false;
                continue;
            }

            // Case later than the clickable part of the rythm
            if ((_elapsedTime+_offsetTime) > rythm.Seconds + _lateTolerance/_speedMultiplier) 
            {
                isHit = false;
                continue;
            }

            
            currentRythm = rythm;
            isHit = true;
            break;
        }

        if (isHit) // Hit
        {
            // _rythmClickables[lane].color = Color.green;
            Green(_rythmClickables[lane]);
            currentRythm.IsClicked = true;
            _hitCount++;
            _onHit?.Invoke();
        }
        else // Wrong
        {
            // _rythmClickables[lane].color = Color.red;
            Red(_rythmClickables[lane]);
            _wrongCount++;
            if(currentRythm != null) currentRythm.IsClicked = true; // Ensure can only wrong once for each rythm line
            _onMiss?.Invoke();
        }
    }


    [SerializeField] Sprite _normalSprite;
    [SerializeField] Sprite _greenSprite;
    [SerializeField] Sprite _redSprite;
    void Green(Image img)
    {
        img.transform.GetChild(1).GetComponent<Image>().sprite = _greenSprite;
    }

    void Red(Image img)
    {
        img.transform.GetChild(1).GetComponent<Image>().sprite = _redSprite;
    }

    public void PointerUpLane(int lane)
    {
        _rythmClickables[lane].transform.GetChild(1).GetComponent<Image>().sprite = _normalSprite;
    }


    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    [Header("Audio")]
    // [SerializeField] AudioSource _audioSource;
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] Slider _progressSlider;
    bool _updateSliderByDragging = false;
    public void ChangeTime(float normalizedTime)
    {
        // _elapsedTime = _audioSource.clip.length * normalizedTime;
        // _audioSource.time = _elapsedTime;

        // _elapsedTime = remap(_videoPlayer.length * normalizedTime, 0, _videoPlayer.length, -1, _videoPlayer.length);
        _elapsedTime = _videoPlayer.length * normalizedTime;
        _videoPlayer.time = _elapsedTime;
    }
    
    double remap(double s, double a1, double a2, double b1, double b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

    void UpdateSlider()
    {
        // _progressSlider.value = _elapsedTime / _audioSource.clip.length;
        _progressSlider.value = (float)(_elapsedTime / _videoPlayer.length);
    }

    public void OnSliderPointerDown()
    {
        _updateSliderByDragging = true;
        _progressSlider.onValueChanged.AddListener(ChangeTime);
    }

    public void OnSliderPointerUp()
    {
        _updateSliderByDragging = false;
        _progressSlider.onValueChanged.RemoveListener(ChangeTime);
    }


#if UNITY_EDITOR
    [ContextMenu("Add Custom")]
    public void AddCustom()
    {
        for(int i = 156; i < 173; i++) {
            _rythm[i].Seconds -= 0.1f;
        }
    }
#endif
}

