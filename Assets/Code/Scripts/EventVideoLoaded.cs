using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using System.Linq;

public class EventVideoLoaded : MonoBehaviour
{
    [SerializeField] VideoPlayer[] _videoPlayers;
    [SerializeField] UnityEvent _onVideoLoaded;
    void Start()
    {
        StartCoroutine(WaitUntilVideoPlay());
    }

    bool isVideoReady(VideoPlayer vp)
    {
        return vp.isPlaying && vp.isPrepared && vp.frame > 1;
    }
    // [SerializeField] int readyCount = 0;
    IEnumerator WaitUntilVideoPlay()
    {
        foreach (var vp in _videoPlayers) vp.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        foreach (var vp in _videoPlayers) vp.gameObject.SetActive(true);
        

        foreach (var vp in _videoPlayers) vp.Play();

        yield return new WaitUntil(() => _videoPlayers.All(isVideoReady));

        // foreach (var vp in _videoPlayers) {
        //     vp.frameReady += (VideoPlayer source, long frameIdx) => {
        //         readyCount++;
        //         if (readyCount == _videoPlayers.Length) _onVideoLoaded.Invoke();
        //     };
        // }

        // yield return null;
        // yield return new WaitUntil(() => _videoPlayers.All(isVideoReady));

        _onVideoLoaded.Invoke();
    }
}
