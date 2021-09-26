using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Advertisements;

public class UIManagerInGame : UIManager
{
    private Player player;

    public Canvas harvestScoreCanvas;
    public Transform harvestScoreTransform;
    private TextMeshProUGUI textHarvestScore;

    public Canvas healthCanvas;
    private Slider textHealthSlider;

    public Canvas fuelCanvas;
    private Slider textFuelSlider;

    public GameObject settingsPrefab;
    public GameObject sensivitySliderPrefab;

    public bool menuOpen = false;
    public RectTransform openMenuButton;
    public Image pauseButton;
    public Sprite openedMenuPauseButton;
    public Sprite closedMenuPauseButton;
    public Button OpenMenuButton;

    public Canvas gameOverCanvas;
    public GameObject gameOverPrefab;
    public GameObject winPrefab;


    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<Player>();

        textHarvestScore = harvestScoreCanvas.GetComponentInChildren<TextMeshProUGUI>();

        textHealthSlider = healthCanvas.GetComponentInChildren<Slider>();
        textHealthSlider.value = 1;

        textFuelSlider = fuelCanvas.GetComponentInChildren<Slider>();
        textFuelSlider.value = 1;

        DBGame = player.DBGames;
        DBPlayer = player.DBPlayer;
        InitializeTuners();
    }

    private void Start()
    {
        if (Advertisement.isSupported)
            Advertisement.Initialize("4103597");

        OpenMenuButton.interactable = true;
        textHarvestScore.text = "0/" + player.HarvestScoreToWin.ToString();
    }

    public void SetTextHealthSlider(float value) => textHealthSlider.value = value;

    public void SetTextFuelSlider(float value) => textFuelSlider.value = value;

    public void SetTextHarvestScore(float score) => textHarvestScore.text = score.ToString() + "/" + player.HarvestScoreToWin.ToString();

    public override void SwitchMenu()
    {
        if (menuOpen)
        {
            switch (currentMenuDeep)
            {
                case MenuDeep.first:
                    Destroy(currentMenu);
                    pauseButton.sprite = closedMenuPauseButton;
                    menuOpen = false;
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
        else
        {
            currentMenu = Instantiate(menuPrefab, menuCanvas.transform);
            currentMenuDeep = MenuDeep.first;
            pauseButton.sprite = openedMenuPauseButton;
            menuOpen = true;
        }
    }

    private void OpenJoystickSettings()
    {
        Destroy(currentMenu);
        currentMenu = Instantiate(sensivitySliderPrefab, menuCanvas.transform);
        currentMenuDeep = MenuDeep.second;
    }

    private void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    private void RestartLevel()
    {
        player.DBGames.SetNeedOriginalSeed(true);
        SceneManager.LoadScene("NewGameScene");
    }

    private void SaveLevel()
    {
        player.GameSaver.SaveGame();
        currentMenuDeep = MenuDeep.first;
        SwitchMenu();
    }

    public override void ProсessButtonClick(MenuButton.Type button)
    {
        base.ProсessButtonClick(button);
        if (button == MenuButton.Type.Back) SwitchMenu();
        else if (button == MenuButton.Type.Settings) OpenJoystickSettings();
        else if (button == MenuButton.Type.MainMenu) GoToMenu();
        else if (button == MenuButton.Type.RecoverByAdd) RecoverByAds();
        else if (button == MenuButton.Type.Restart) RestartLevel();
        else if (button == MenuButton.Type.Save) SaveLevel();
        else if (button == MenuButton.Type.OpenGlobalMap) OpenGlobalMap();
    }

    public void OpenGameOver()
    {
        currentMenu = Instantiate(gameOverPrefab, gameOverCanvas.transform);
        OpenMenuButton.interactable = false;
        menuOpen = true;
    }

    private void RecoverByAds()
    {
        if (Advertisement.IsReady())
            Advertisement.Show("Rewarded_Android");
        player.Tractor.Recover();
        SetTextHealthSlider(player.Tractor.СurrentHealth);
        SetTextFuelSlider(player.Tractor.СurrentFuel);
        Destroy(currentMenu);
        menuOpen = false;
        OpenMenuButton.interactable = true;
    }

    public void Win()
    {
        currentMenu = Instantiate(winPrefab, gameOverCanvas.transform);
        currentMenu.GetComponentInChildren<TextMeshProUGUI>().text = "Congratulations\nYour score: " + player.HarvestScore;
        int level = player.DBPlayer.GetCurrentLvl();
        if (level != (int)GameManager.GameLvl.freeGame)
        {
            player.DBGames.SetNeedCreateGame(true);
            player.DBPlayer.SetCurrentLvl(++level);
            if (player.DBPlayer.GetMaxLvl() < level) player.DBPlayer.SetMaxLvl(level);
        }
        OpenMenuButton.interactable = false;
        menuOpen = true;
    }

    public void ProсessTunerValue(MenuButton.Type tuner, Scrollbar scrollbar, TextMeshProUGUI text)
    {
        if (tuner == MenuButton.Type.Sensivity) TuneSensivity(scrollbar, text);
        else if (tuner == MenuButton.Type.JoystickSize) TuneJoystickSize(scrollbar, text);
    }

    public void ProсessCheckBox(MenuButton.Type type, Toggle toggle)
    {
        if (type == MenuButton.Type.JoystickDynamicCheckBox) SetDynamicJoystickStatus(toggle);
    }

    private void TuneJoystickSize(Scrollbar scrollbar, TextMeshProUGUI text)
    {
        player.Controls.SetJoystickSize(scrollbar.value);
        text.text = "Joystick size = " + Math.Round(player.Controls.CurrentSize, 2);
    }

    public void TuneSensivity(Scrollbar scrollbar, TextMeshProUGUI text)
    {
        player.Controls.SetDeathZone(scrollbar.value);
        text.text = "Joystick dead zone = " + Math.Round(player.Controls.CurrentDeadZone, 2);
    }

    public void SetDynamicJoystickStatus(Toggle toggle)
    {
        player.Controls.SetJoystickDynamicStatus(toggle.isOn);
    }
}
