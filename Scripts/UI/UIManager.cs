using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Canvas menuCanvas;
    public GameObject menuPrefab;

    protected GameObject[] previousMenu;
    protected GameObject currentMenu;
    protected MenuDeep currentMenuDeep;

    public int currentLvlOnMap;

    protected DBGame DBGame;
    protected DBPlayer DBPlayer;
    //new game
    public GameObject newGameMenuPrefab;
    public GameObject SizePrefab;
    public GameObject DifficultPrefab;
    public DifficultManager.DifficultLevel DifficultLevel { get; protected set; }

    public Vector2Int CurrentSize { get; set; }
    public float currentXSizeValue = 0f;
    public float currentYSizeValue = 0f;

    protected virtual void Awake()
    {
        previousMenu = new GameObject[2];
    }


    public enum MenuDeep
    {
        first = 1,
        second = 2,
        third = 3,
        fourth = 4
    }

    protected void InitializeTuners()
    {
        DifficultLevel = DBGame.GetGameDifficult();

        Vector2Int size = DBGame.GetGameSizes();
        CurrentSize = size;

        currentXSizeValue = CalculateValue(CurrentSize.x);
        currentYSizeValue = CalculateValue(CurrentSize.y);
    }

    protected float CalculateValue(int size)
    {
        float value;
        if (size <= 10)
            value = (size - 5) / 10f;
        else
            value = (size - 10) / 50f + 0.5f;

        return value;
    }


    public virtual void SwitchMenu()
    {
        switch (currentMenuDeep)
        {
            case MenuDeep.first:
                break;
            case MenuDeep.second:
                Destroy(currentMenu);
                currentMenu = Instantiate(menuPrefab, menuCanvas.transform);
                currentMenuDeep = MenuDeep.first;
                break;
            case MenuDeep.third:
                Destroy(currentMenu);
                currentMenu = previousMenu[0];
                currentMenu.SetActive(true);
                currentMenuDeep = MenuDeep.second;
                break;
            case MenuDeep.fourth:
                Destroy(currentMenu);
                currentMenu = previousMenu[1];
                currentMenu.SetActive(true);
                currentMenuDeep = MenuDeep.third;
                break;
            default:
                break;
        }
    }

    public virtual void ProсessButtonClick(MenuButton.Type button)
    {
        if (button == MenuButton.Type.NewGame) NewGame();
        else if (button == MenuButton.Type.CreateNewGame) CreateNewFreeGame();
        else if (button == MenuButton.Type.Size) TuneSize();
        else if (button == MenuButton.Type.Difficult) OpenDifficultWindow();
    }

    protected void OpenGlobalMap()
    {
        SceneManager.LoadScene("StoryMap");
    }

    #region new game
    protected void NewGame()
    {
        if (DBPlayer.GetCurrentLvl() == (int)GameManager.GameLvl.train ||
            DBPlayer.GetCurrentLvl() != (int)GameManager.GameLvl.freeGame)
        {
            DBGame.SetNeedCreateGame(true);
            SceneManager.LoadScene("NewGameScene");
        }
        else
        {
            Destroy(currentMenu);
            currentMenu = Instantiate(newGameMenuPrefab, menuCanvas.transform);
            currentMenuDeep = MenuDeep.second;
        }
    }

    protected void OpenDifficultWindow()
    {
        Vector3 previosPos = currentMenu.transform.position;
        previousMenu[0] = currentMenu;
        currentMenu.SetActive(false);
        currentMenu = Instantiate(DifficultPrefab, previosPos, Quaternion.identity, menuCanvas.transform);

        currentMenuDeep = MenuDeep.third;
    }

    public void SetCurrentDifficultLevel(int difficult)
    {
        switch (difficult)
        {
            case 1:
                DifficultLevel = DifficultManager.DifficultLevel.Easy;
                break;
            case 2:
                DifficultLevel = DifficultManager.DifficultLevel.Medium;
                break;
            case 3:
                DifficultLevel = DifficultManager.DifficultLevel.Hard;
                break;
            default:
                DifficultLevel = DifficultManager.DifficultLevel.Easy;
                break;
        }
    }

    protected void SetDifficult() => DBGame.SetGameDifficult(DifficultLevel);

    protected void CreateNewFreeGame()
    {
        DBGame.SetNeedCreateGame(true);
        SetSize();
        SetDifficult();
        SceneManager.LoadScene("NewGameScene");
    }

    protected void TuneSize()
    {
        Vector3 previosPos = currentMenu.transform.position;
        previousMenu[0] = currentMenu;
        currentMenu.SetActive(false);
        currentMenu = Instantiate(SizePrefab, previosPos, Quaternion.identity, menuCanvas.transform);

        currentMenuDeep = MenuDeep.third;
    }

    public void SetXSize(int x) => CurrentSize = new Vector2Int(x, CurrentSize.y);
    public void SetYSize(int y) => CurrentSize = new Vector2Int(CurrentSize.x, y);

    protected void SetSize() => DBGame.SetGameSizes(CurrentSize);

    #endregion
}
