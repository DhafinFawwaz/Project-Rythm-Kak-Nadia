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

    [Header("Result")]
    [SerializeField] int _hitCount;
    [SerializeField] int _wrongCount;
    [SerializeField] int _missCount;

    [SerializeField] UnityEvent<Rythm[], float> _onRythmGenerated;
    void Start()
    {
        _reader = new RythmReader(_apiURL);
        GenerateRythm();
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
    IEnumerator MoveCamera()
    {
        Transform cam = Camera.main.transform;
        while (true)
        {
            _elapsedTime += Time.deltaTime;
            // if(!_audioSource.isPlaying && _elapsedTime >= 0) _audioSource.Play();
            
            cam.position = new Vector3(cam.position.x, (float)(_elapsedTime+_offsetTime) * _speedMultiplier, cam.position.z);
            if(!_updateSliderByDragging) UpdateSlider();
            
            // Check if all rythm has passed
            // if(_elapsedTime > _rythm[_rythm.Length-1].Seconds + _rythm[_rythm.Length-1].Length + _winTolerance) break;
            
            yield return null;
        }
        _missCount = _rythm.Length - _hitCount;

    }


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
            _rythmClickables[lane].color = Color.green;
            currentRythm.IsClicked = true;
            _hitCount++;
        }
        else // Wrong
        {
            _rythmClickables[lane].color = Color.red;
            _wrongCount++;
            if(currentRythm != null) currentRythm.IsClicked = true; // Ensure can only wrong once for each rythm line
        }
    }

    public void PointerUpLane(int lane)
    {
        _rythmClickables[lane].color = Color.white;
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
        Rythm[] longer = new Rythm[250];
        for(int i = 0; i < _rythm.Length; i++)
        {
            longer[i] = _rythm[i];
        }

        float current = 152f;
        for(int i = 188; i < 250; i++)
        {
            longer[i].Lane = ((i+1) % 2) * 3;
            longer[i].Seconds = 152f;
            current += 0.125f;
        }
    }
#endif
}

