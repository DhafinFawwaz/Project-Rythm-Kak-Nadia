using Unity.VisualScripting;
using UnityEngine;

namespace DhafinFawwaz.Tweener
{
    public class TransformTweener : Tweener
    {
        [Header("Values")]
        [SerializeField] Transform _target;
        [SerializeField] Vector3 _end;
        Coroutine[] _coroutines = new Coroutine[3];

        public override void Stop()
        {
            foreach (var c in _coroutines) StopCoroutineIfNotNull(c);
        }

        public TransformTweener SetEnd(Vector3 end)
        {
            _end = end;
            return this;
        }

        public TransformTweener SetTarget(Transform target)
        {
            _target = target;
            return this;
        }

        [ContextMenu("Play Position")]
        public void Position()
        {
            StopCoroutineIfNotNull(_coroutines[2]);
            _coroutines[2] = TweenIfActive(
                x => _target.position = x,
                _target.position,
                _end,
                _duration,
                Vector3.LerpUnclamped
            );
        }

        [ContextMenu("Play LocalPosition")]
        public void LocalPosition()
        {
            StopCoroutineIfNotNull(_coroutines[2]);
            _coroutines[2] = TweenIfActive(
                x => _target.localPosition = x,
                _target.localPosition,
                _end,
                _duration,
                Vector3.LerpUnclamped
            );
        }

        [ContextMenu("Play EulerAngles")]
        public void EulerAngles()
        {
            StopCoroutineIfNotNull(_coroutines[1]);
            _coroutines[1] = TweenIfActive(
                x => _target.rotation = Quaternion.Euler(x),
                _target.eulerAngles,
                _end,
                _duration,
                Vector3.SlerpUnclamped
            );
        }

        [ContextMenu("Play LocalEulerAngles")]
        public void LocalEulerAngles()
        {
            StopCoroutineIfNotNull(_coroutines[1]);
            _coroutines[1] = TweenIfActive(
                x => _target.localEulerAngles = x,
                _target.localEulerAngles,
                _end,
                _duration,
                Vector3.LerpUnclamped
            );
        }

        [ContextMenu("Play LocalScale")]
        public void LocalScale()
        {
            StopCoroutineIfNotNull(_coroutines[0]);
            _coroutines[0] = TweenIfActive(
                x => _target.localScale = x,
                _target.localScale,
                _end,
                _duration,
                Vector3.LerpUnclamped
            );
        }


#if UNITY_EDITOR
        void Reset()
        {
            _target = transform;
        }
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.green;
            
            // position
            Gizmos.DrawLine(_target.position, _target.position + _end);

            if(_target.TryGetComponent<Renderer>(out Renderer r))
            {
                Bounds b = r.localBounds;
                Vector3 _initialSize = new Vector3(b.size.x*transform.localScale.x, b.size.y*transform.localScale.y, b.size.z*transform.localScale.z);

                // position
                Gizmos.DrawWireCube(b.center + _end, _initialSize);
                

                // localScale
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(b.center, new Vector3(b.size.x*_end.x, b.size.y*_end.y, b.size.z*_end.z));
                
                // eulerAngles, draw rotated cube
                Gizmos.color = Color.blue;
                DrawRotatedCubeGizmos(b.center, _initialSize, _end.z);
            }

            
        }

        void DrawRotatedCubeGizmos(Vector2 center, Vector2 size, float z)
        {
            Vector3 topLeft = new Vector3(center.x - size.x/2, center.y + size.y/2);
            topLeft = Quaternion.AngleAxis(z, Vector3.forward) * topLeft;
            Vector3 topRight = new Vector3(center.x + size.x/2, center.y + size.y/2);
            topRight = Quaternion.AngleAxis(z, Vector3.forward) * topRight;
            Vector3 bottomLeft = new Vector3(center.x - size.x/2, center.y - size.y/2);
            bottomLeft = Quaternion.AngleAxis(z, Vector3.forward) * bottomLeft;
            Vector3 bottomRight = new Vector3(center.x + size.x/2, center.y - size.y/2);
            bottomRight = Quaternion.AngleAxis(z, Vector3.forward) * bottomRight;

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
#endif
    }

}
