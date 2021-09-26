using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerInMenu : UIManager
{
    public GameObject menu;
    public GameObject tapToPlay;
    private Button loadGameButton;
    public GameObject mapButton;

    protected override void Awake()
    {
        base.Awake();
        DBGame = GetComponent<DBGame>();
        DBPlayer = GetComponent<DBPlayer>();
        InitializeTuners();
    }

    private void Start()
    {
        //if (DBPlayer.GetCurrentLvl() == (int)GameManager.GameLvl.train)
        //{
        currentMenu = Instantiate(tapToPlay, menuCanvas.transform);//TODO: choose
        if (DBGame.IsItFirstSave()) mapButton.SetActive(false);
        else mapButton.SetActive(true);
        //}
        //else
        //{
        //    currentMenu = Instantiate(menu, menuCanvas.transform);
        //    currentMenuDeep = MenuDeep.first;

        //    loadGameButton = currentMenu.GetComponentsInChildren<Button>()[1];
        //    if (DBGame.IsItFirstSave()) loadGameButton.interactable = false;
        //    else loadGameButton.interactable = true;
        //}
    }

    public override void ProсessButtonClick(MenuButton.Type button)
    {
        if (button == MenuButton.Type.Back) SwitchMenu();
        else if (button == MenuButton.Type.NewGame) NewGame();
        else if (button == MenuButton.Type.CreateNewGame) CreateNewFreeGame();
        else if (button == MenuButton.Type.Size) TuneSize();
        else if (button == MenuButton.Type.Difficult) OpenDifficultWindow();
        else if (button == MenuButton.Type.LoadLastGame) LoadLastGame();
        else if (button == MenuButton.Type.OpenGlobalMap) OpenGlobalMap();
    }

    private void LoadLastGame()
    {
        if (!DBGame.GetNeedCreateGame())
        {
            DBGame.SetNeedCreateGame(false);
            DBGame.SetNeedOriginalSeed(false);
            SceneManager.LoadScene("NewGameScene"); 
        }
        else NewGame();
    }
}
