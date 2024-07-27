using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WindyBone : MonoBehaviour
{
    [SerializeField] float _period = 2f;
    [SerializeField] float _offsetTime = -0.25f;
    [SerializeField] List<BoneAngle> _boneList = new List<BoneAngle>();

    void Update()
    {
        float t = Time.time / _period;
        for (int i = 0; i < _boneList.Count; i++)
        {
            float angle = Mathf.Lerp(_boneList[i].MinimumAngle, _boneList[i].MaximumAngle, Sin01(t + i * _offsetTime));
            _boneList[i].Bone.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    float Sin01(float t)
    {
        return (Mathf.Sin(t * Mathf.PI * 2) + 1) * 0.5f;
    }

    float Cos01(float t)
    {
        return (Mathf.Cos(t * Mathf.PI * 2) + 1) * 0.5f;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if(Application.isPlaying) return;

        for (int i = 0; i < _boneList.Count; i++)
        {
            if (_boneList[i].Bone != null)
            {
                _boneList[i].InitialAngle = _boneList[i].Bone.localRotation.eulerAngles.z;
            }
        }
    }

    void Reset()
    {
        AssignBoneRecursive(transform, 0);
    }

    void AssignBoneRecursive(Transform t, int index)
    {
        try{
            SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
            if(sr != null)
            {
                Transform child = t.GetChild(0);
                if(child != null)
                {
                    while(_boneList.Count <= index)
                    {
                        _boneList.Add(new BoneAngle());
                    }
                    _boneList[index].Bone = child;
                    AssignBoneRecursive(t.GetChild(0), index+1);
                }
            }
            else
            {
                while(_boneList.Count <= index)
                {
                    _boneList.Add(new BoneAngle());
                }
                _boneList[index].Bone = t;
                AssignBoneRecursive(t.GetChild(0), index+1);
            }
        } catch {}
    }
#endif
}

[System.Serializable]
class BoneAngle
{
    public Transform Bone { get => _bone; set => _bone = value;}
    
    public float InitialAngle { get => _initialAngle; set => _initialAngle = value;}
    public float Delta => _delta;
    public float MinimumAngle => _initialAngle - _delta + _offsetAngle;
    public float MaximumAngle => _initialAngle + _delta + _offsetAngle;
    [SerializeField] Transform _bone;

    [ReadOnly]
    [SerializeField] float _initialAngle;
    [SerializeField] float _offsetAngle = 0;
    [SerializeField] float _delta = 5;
}