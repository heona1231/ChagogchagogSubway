//서현아 작성

using UnityEngine;
using UnityEngine.UI;

public class MinigameUi : MonoBehaviour
{
    [SerializeField] private Slider gaugeSlider;

    public void SetupSlider(float maxGauge)
    {
        if (gaugeSlider != null)
        {
            gaugeSlider.maxValue = maxGauge;
            gaugeSlider.value = maxGauge;
        }
    }

    public void UpdateSlider(float currentValue)
    {
        if (gaugeSlider != null)
        {
            gaugeSlider.value = currentValue;
        }
    }
}
