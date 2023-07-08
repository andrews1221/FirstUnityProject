using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressController : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetProgressBarMaxValue(int max)
    {
        slider.maxValue = max;
    }

    public void UpdateProgressBar()
    {
        slider.value += 1;
    }
    
}
