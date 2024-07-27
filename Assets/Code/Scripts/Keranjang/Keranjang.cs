using System.Collections;
using System.Collections.Generic;
using DhafinFawwaz.Tweener;
using UnityEngine;

public class Keranjang : MonoBehaviour
{
    [SerializeField] TransformTweener _tweener;
    [SerializeField] int _orderInLayer = 1;

    [Tooltip("Filter the trash name to be accepted by this trash can")]
    [SerializeField] string _trashName = "page10_0003_Sampah-10";
    public void OnTrashDropped(SpriteDragDrop spriteDragDrop)
    {
        if(spriteDragDrop.name != _trashName) return;
        spriteDragDrop.CancelTweenPosition();
        spriteDragDrop.enabled = false;
        _tweener.SetTarget(spriteDragDrop.transform)
            .SetEnd(transform.position)
            .Position();
        spriteDragDrop.GetComponent<SpriteRenderer>().sortingOrder = _orderInLayer;
    }
}
