using UnityEngine;

public class GameManager : MonoBehaviour
{ 
    [HideInInspector] public Player player;
    public enum GameLvl
    {
        train = 0,
        freeGame = 11
    }

    private void Awake() => player = GetComponent<Player>();

    private void Start()
    {
        if (player.DBPlayer.GetCurrentLvl() == (int)GameLvl.train)
        {
            player.TrainingLevel.enabled = true;
            if (player.DBGames.GetNeedCreateGame())
            {
                player.DBGames.SetGameSizes(player.TrainingLevel.LvlSize);
                player.DBGames.SetGameDifficult(player.TrainingLevel.LvlDifficult);
                player.Field.CreateGame();
            }
            else player.GameLoader.LoadGame();
        }
        else if (player.DBPlayer.GetCurrentLvl() != (int)GameLvl.freeGame)
        {
            if (player.DBGames.GetNeedCreateGame())
            {
                player.DBGames.SetGameSizes(player.StoryGame.StoryLvlSize[player.DBPlayer.GetCurrentLvl()]);
                player.DBGames.SetGameDifficult(player.StoryGame.StoryLvlDifficult[player.DBPlayer.GetCurrentLvl()]);
                player.Field.CreateGame();
            }
            else player.GameLoader.LoadGame();
        }
        else
        {
            if (player.DBGames.GetNeedCreateGame()) player.Field.CreateGame();
            else player.GameLoader.LoadGame();
        }
    }
}
