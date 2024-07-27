using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GSheetsReader
{
    string _url = "";
    string[][] _data = null;
    string _rawData = "";
    public string RawData => _rawData;
    
    public GSheetsReader(string url)
    {
        _url = url;
    }

    /// <summary>
    /// Read the CSV file from the URL. Basically a csv parser
    /// </summary>
    /// <returns></returns>
    public async Task<string[][]> Read()
    {
        string data = "";
        UnityWebRequest www = UnityWebRequest.Get(_url);
        www.useHttpContinue = false;
        www.SendWebRequest();

        while (!www.isDone)
        {
            await Task.Yield();
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            return null;
        }
        else
        {
            Debug.Log("Data loaded");
            data = www.downloadHandler.text;
        }

        _rawData = data;

        string[] lines = data.Split('\n');
        string[][] output = new string[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            output[i] = lines[i].Split(',');
        }

        _data = output;
        return output;
    }

    public string[] GetColumn(int index)
    {
        if (_data == null)
        {
            Debug.LogError("Data is not loaded yet");
            return null;
        }

        string[] column = new string[_data.Length];
        for (int i = 0; i < _data.Length; i++)
        {
            column[i] = _data[i][index];
        }

        return column;
    }

    public int[] GetColumnInt(int index)
    {
        if (_data == null)
        {
            Debug.LogError("Data is not loaded yet");
            return null;
        }

        int[] column = new int[_data.Length];
        for (int i = 0; i < _data.Length; i++)
        {
            column[i] = int.Parse(_data[i][index]);
        }

        return column;
    }
    

    public string[] GetRow(int index)
    {
        if (_data == null)
        {
            Debug.LogError("Data is not loaded yet");
            return null;
        }

        return _data[index];
    }

    public int[] GetRowInt(int index)
    {
        if (_data == null)
        {
            Debug.LogError("Data is not loaded yet");
            return null;
        }

        int[] row = new int[_data[index].Length];
        for (int i = 0; i < _data[index].Length; i++)
        {
            row[i] = int.Parse(_data[index][i]);
        }

        return row;
    }

}

[System.Serializable]
public class Rythm
{
    public bool IsClicked;
    public float Seconds;
    public int Lane;
    public float Length;
}

public class RythmReader : GSheetsReader
{
    public RythmReader(string url) : base(url)
    {
    }

    public async Task<Rythm[]> ReadRythm()
    {
        string[][] data = await Read();
        Rythm[] rythms = new Rythm[data.Length-1];
        for (int i = 1; i < data.Length; i++)
        {
            rythms[i-1] = new Rythm
            {
                Seconds = float.Parse(data[i][0]),
                Lane = int.Parse(data[i][1]),
                Length = float.Parse(data[i][2])
            };
        }

        return rythms;
    }
}


#if UNITY_EDITOR
// Draw the custom property drawer for the Rythm class, 3 fields in one line, and a checkbox for IsClicked
[CustomPropertyDrawer(typeof(Rythm))]
public class RythmDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        int space = 5;
        contentPosition.width = contentPosition.width/4 - space;
        EditorGUI.indentLevel = 0;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("Seconds"), GUIContent.none);
        contentPosition.x += contentPosition.width + space;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("Lane"), GUIContent.none);
        contentPosition.x += contentPosition.width + space;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("Length"), GUIContent.none);
        contentPosition.x += contentPosition.width + space;

        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("IsClicked"), GUIContent.none);
        
        EditorGUI.EndProperty();

    }
}
#endif