using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer;

    [ContextMenu("Play")]
    public void Play()
    {
        _videoPlayer.Play();
    }

    [ContextMenu("Prepare")]
    public void Prepare()
    {
        StartCoroutine(PrepareAndLog());
    }

    IEnumerator PrepareAndLog()
    {
        _videoPlayer.Prepare();
        while(!_videoPlayer.isPrepared)
        {
            yield return new WaitForSecondsRealtime(0.3f);
        }
        Debug.Log("Video Prepared!");
    }
}
