using UnityEngine;

public class Finish : MonoBehaviour
{
    private Player player;

    private Animation scoreLabelDoAngry;

    public void SetConnectionWithPlayer(Player player)
    {
        this.player = player;
        scoreLabelDoAngry = player.UIManager.harvestScoreTransform.GetComponent<Animation>();
    }

    public void PlayerStepOnFinish()
    {
        if (player.HarvestScore >= player.HarvestScoreToWin)
            player.UIManager.Win();
        else
            HelpPlayerToGetThatHeDumb();
    }

    private void HelpPlayerToGetThatHeDumb()
    {
        scoreLabelDoAngry.Play("scoreLabel");
    }
}
