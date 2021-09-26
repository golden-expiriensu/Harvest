using System;
using UnityEngine;

public class SizeTuner : ValueTuner
{
    private void Start()
    {
        if (type == Type.Size_x)
        {
            scrollbar.value = UIManager.currentXSizeValue;
            text.text = "width = " + UIManager.CurrentSize.x;
        }
        else
        {
            scrollbar.value = UIManager.currentYSizeValue;
            text.text = "height = " + UIManager.CurrentSize.y;
        }
    }

    public void TuneSize()
    {
        if (type == Type.Size_x)
        {
            UIManager.SetXSize(CalculateSize(scrollbar.value));
            UIManager.currentXSizeValue = scrollbar.value;
            text.text = "width = " + UIManager.CurrentSize.x;
        }
        else
        {
            UIManager.SetYSize(CalculateSize(scrollbar.value));
            UIManager.currentYSizeValue = scrollbar.value;
            text.text = "height = " + UIManager.CurrentSize.y;
        }
    }

    private int CalculateSize(float value)
    {
        int size;
        if (value <= 0.5f)
            size = 5 + (int)(10 * value);
        else
            size = 10 + Mathf.RoundToInt(50f * (value - 0.5f));

        return size;
    }
}
