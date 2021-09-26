using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxTuner : ValueTuner
{
    private Player player;
    public Toggle checkBox;

    private void Start()
    {
        player = GetComponentInParent<Player>();

        if (type == Type.JoystickDynamicCheckBox)
        {
            checkBox.isOn = player.DBPlayer.GetJoystickIsDynamic();
        }
    }

    public void TuneIsDynamic()
    {
        checkBox.isOn = !checkBox.isOn;
        player.UIManager.SetDynamicJoystickStatus(checkBox);
    }
}
