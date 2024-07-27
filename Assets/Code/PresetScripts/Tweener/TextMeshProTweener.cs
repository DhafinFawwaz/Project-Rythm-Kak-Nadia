using System;
using TMPro;
using UnityEngine;

namespace DhafinFawwaz.Tweener
{
    public class TextMeshProTweener : Tweener
    {
        [Header("Values")]
        [SerializeField] TMP_Text _target;
        [SerializeField] long _start;
        [SerializeField] long _end;
        [SerializeField] Color _endColor;
        [SerializeField] string _numericFormat = "N0";
        [SerializeField] string _textFormat = "{0}";
        Coroutine[] _coroutines = new Coroutine[1];
        [SerializeField] bool _constantSpeed = false;
        [SerializeField] bool _hideTextOnAwake = false;
        void Awake()
        {
            if (_hideTextOnAwake) _target.maxVisibleCharacters = 0;
        }

        public override void Stop()
        {
            foreach (var c in _coroutines) StopCoroutineIfNotNull(c);
        }

        TextMeshProTweener SetStart(long start)
        {
            _start = start;
            return this;
        }
        TextMeshProTweener SetEnd(long end)
        {
            _end = end;
            return this;
        }
        TextMeshProTweener SetFormat(string format)
        {
            _numericFormat = format;
            return this;
        }

        [ContextMenu("Play Numeric")]
        public void Numeric()
        {
            _target.maxVisibleCharacters = int.MaxValue;
            StopCoroutineIfNotNull(_coroutines[0]);
            _coroutines[0] = TweenIfActive(
                x => _target.text = String.Format(_textFormat, x.ToString(_numericFormat)),
                _start,
                _end,
                _duration,
                LerpUnclamped
            );
        }

        long LerpUnclamped(long a, long b, float t)
        {
            return a + (long)((b - a) * t);
        }

        [ContextMenu("Play Text")]
        public void Text()
        {
            StopCoroutineIfNotNull(_coroutines[0]);
            _coroutines[0] = TweenIfActive(
                x => _target.maxVisibleCharacters = x,
                0,
                _target.text.Length,
                _constantSpeed ? _duration*_target.text.Length : _duration,
                LerpUnclamped
            );
        }

        [ContextMenu("Play Color")]
        public void Color()
        {
            StopCoroutineIfNotNull(_coroutines[0]);
            _coroutines[0] = TweenIfActive(
                x => _target.color = x,
                _target.color,
                _endColor,
                _duration,
                UnityEngine.Color.LerpUnclamped
            );
        }

        int LerpUnclamped(int a, int b, float t)
        {
            return a + (int)((b - a) * t);
        }

        [ContextMenu("Set To Max Visible Characters")]
        public void SetToMaxVisibleCharacters()
        {
            _target.maxVisibleCharacters = _target.text.Length;
        }
    }

}
