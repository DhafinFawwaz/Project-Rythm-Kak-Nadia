using UnityEngine;
using System.Collections;
using DhafinFawwaz.Tweener;

public class ScreenFadeSceneTransition : SceneTransition
{
    [Header("Animation Properties")]
    [SerializeField] ImageTweener _tweener;

    /// <summary>
    /// The out animation itself
    /// </summary>
    /// <returns></returns>
    public override IEnumerator OutAnimation()
    {
        _tweener.SetEndColor(new Color(0,0,0,1)).Color();

        bool isDone = false;        
        _tweener.OnceDone(() => {
            isDone = true;
        });

        yield return new WaitUntil(() => isDone);
    }

    /// <summary>
    /// The in animation itself
    /// </summary>
    /// <returns></returns>
    public override IEnumerator InAnimation()
    {
        _tweener.SetEndColor(new Color(0,0,0,0)).Color();

        bool isDone = false;        
        _tweener.OnceDone(() => {
            isDone = true;
        });

        yield return new WaitUntil(() => isDone);
    }


}
