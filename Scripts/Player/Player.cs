using UnityEngine;

public class Player : MonoBehaviour
{
    public UIManagerInGame UIManager { get; private set; }
    public Tractor Tractor;
    [HideInInspector] public Controls Controls;
    [HideInInspector] public DifficultManager DifficultManager;
    [HideInInspector] public DBGame DBGames;
    [HideInInspector] public DBPlayer DBPlayer;

    [HideInInspector] public SavingSystem.Saver GameSaver;
    [HideInInspector] public SavingSystem.Loader GameLoader;
    [HideInInspector] public SavingSystem.SeedHandler SeedHandler;

    public TrainingLevel TrainingLevel;
    public StoryGame StoryGame;


    public FieldConstructor Field;

    public bool IsGameOver { get; set; } = false;
    public int HarvestScore { get; private set; } = 0;
    public int HarvestScoreToWin { get; private set; }


    private void OnEnable()
    {
        UIManager = GetComponent<UIManagerInGame>();
        Controls = GetComponent<Controls>();
        DifficultManager = GetComponent<DifficultManager>();
        DBGames = GetComponent<DBGame>();
        DBPlayer = GetComponent<DBPlayer>();

        if (DBPlayer.GetCurrentLvl() == (int)GameManager.GameLvl.freeGame) TrainingLevel = GetComponent<TrainingLevel>();
        else if (DBPlayer.GetCurrentLvl() != (int)GameManager.GameLvl.freeGame) StoryGame = GetComponent<StoryGame>();
    }

    public void InitializeSavingSystem(SavingSystem.Saver GameSaver, SavingSystem.Loader GameLoader, SavingSystem.SeedHandler SeedHandler)
    {
        this.GameSaver = GameSaver;
        this.GameLoader = GameLoader;
        this.SeedHandler = SeedHandler;
    }

    public void AddHarvestScore(int amount)
    {
        HarvestScore += amount;
        UIManager.SetTextHarvestScore(HarvestScore);
    }

    public void SetHarvestScoreToWin(float value)
    {
        HarvestScoreToWin = Mathf.RoundToInt(value);
        UIManager.SetTextHarvestScore(HarvestScore);
    }

    public void GetLoot(Loot.Types loot)
    {
        if (loot == Loot.Types.Petrol) Tractor.AddFuel(Field.fuelDistributionFactor);
        else if (loot == Loot.Types.RepairBox) Tractor.AddHealth(Field.healthBenefitFactor);
    }

    public void SetGameOver()
    {
        UIManager.OpenGameOver();
    }
}
