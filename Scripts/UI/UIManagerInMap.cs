using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerInMap : UIManager
{
    public Canvas lvlButtonsPlace;
    Button[] lvls;

    protected override void Awake()
    {
        base.Awake();
        DBPlayer = GetComponent<DBPlayer>();
        DBGame = GetComponent<DBGame>();

        lvls = GetComponentsInChildren<Button>();
        for (int i = DBPlayer.GetMaxLvl() + 1; i < lvls.Length; i++)
        {
            lvls[i].interactable = false;
        }
    }

    public override void ProсessButtonClick(MenuButton.Type button)
    {
        base.ProсessButtonClick(button);
        if (button == MenuButton.Type.Back) SwitchMenu();
        else if (button == MenuButton.Type.MapLVL) ChooseLvl();
    }

    private void ChooseLvl()
    {
        DBPlayer.SetCurrentLvl(currentLvlOnMap);
        NewGame();
    }
}
