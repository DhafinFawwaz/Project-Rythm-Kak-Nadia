using UnityEngine;
using UnityEngine.UI;

namespace DhafinFawwaz.Tweener
{
    public class ImageTweener : Tweener
    {
        [Header("Values")]
        [SerializeField] Image _target;
        [SerializeField] Color _endColor;
        [Range(0, 1)]
        [SerializeField] float _endFill;
        Coroutine[] _coroutines = new Coroutine[2];

        public ImageTweener SetEndColor(Color end)
        {
            _endColor = end;
            return this;
        }

        public ImageTweener SetEndFill(float fill)
        {
            _endFill = fill;
            return this;
        }

        public void SetEndFillValue(float fill)
        {
            _endFill = fill;
        }

        public override void Stop()
        {
            foreach (var c in _coroutines) StopCoroutineIfNotNull(c);
        }

        public void SetTargetAlpha(float alpha)
        {
            _target.color = new Color(_target.color.r, _target.color.g, _target.color.b, alpha);
        }
        public void SetTargetRed(float red)
        {
            _target.color = new Color(red, _target.color.g, _target.color.b, _target.color.a);
        }

        [ContextMenu("Play Color")]
        public void Color()
        {
            StopCoroutineIfNotNull(_coroutines[0]);
            _coroutines[0] = TweenIfActive<Color>(
                x => _target.color = x,
                _target.color,
                _endColor,
                _duration,
                UnityEngine.Color.LerpUnclamped
            );
        }

        [ContextMenu("Play Fill")]
        public void Fill()
        {
            StopCoroutineIfNotNull(_coroutines[1]);
            _coroutines[1] = TweenIfActive<float>(
                x => _target.fillAmount = x,
                _target.fillAmount,
                _endFill,
                _duration,
                Mathf.LerpUnclamped
            );
        }
    }

}
