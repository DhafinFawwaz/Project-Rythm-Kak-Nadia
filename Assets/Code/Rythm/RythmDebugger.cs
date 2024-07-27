using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RythmDebugger : MonoBehaviour
{
    [SerializeField] RectTransform _parent;
    
    List<TextMeshProUGUI> _textList = new List<TextMeshProUGUI>();
    public void UpdateDebug(Rythm[] _rythm, float _speedMultiplier)
    {
        foreach (var text in _textList) Destroy(text.gameObject);
        _textList.Clear();

        TextMeshProUGUI secondsText = CreateText("Seconds\n");
        TextMeshProUGUI laneText = CreateText("Lane\n");
        TextMeshProUGUI lengthText = CreateText("Length\n");
        TextMeshProUGUI speedText = CreateText("Speed Multiplier\n");
        
        _textList.Add(secondsText);
        _textList.Add(laneText);
        _textList.Add(lengthText);
        _textList.Add(speedText);
        
        for (int i = 0; i < _rythm.Length; i++)
        {
            secondsText.text += $"{_rythm[i].Seconds}\n";
            laneText.text += $"{_rythm[i].Lane}\n";
            lengthText.text += $"{_rythm[i].Length}\n";
        }

        speedText.text += $"{_speedMultiplier}";
    }

    TextMeshProUGUI CreateText(string text)
    {
        GameObject go = new GameObject();
        go.transform.SetParent(_parent);
        go.transform.localScale = Vector3.one;
        TextMeshProUGUI textMesh = go.AddComponent<TextMeshProUGUI>();
        textMesh.text = text;
        textMesh.alignment = TextAlignmentOptions.Center;
        return textMesh;
    }
}
