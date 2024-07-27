using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderUIToggle : MonoBehaviour
{
    [SerializeField] SliderUI _sliderUI;

    public void Toggle()
    {
        if(_sliderUI.normalizedValue == 0)
        {
            _sliderUI.normalizedValue = 1;
        }
        else
        {
            _sliderUI.normalizedValue = 0;
        }
    }
}
