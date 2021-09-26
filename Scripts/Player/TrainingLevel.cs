using UnityEngine;

public class TrainingLevel : MonoBehaviour
{
    Player player;
    public Transform trainCanvas;
    public GameObject moveHelpLabel;
    public GameObject scoreHelpLabel;
    public GameObject finishHelpLabel;
    GameObject currentHelpLabel;
    TrainState currentState;

    private enum TrainState
    {
        move,
        score,
        finish
    }

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public readonly Vector2Int LvlSize = new Vector2Int(5, 5);
    public readonly DifficultManager.DifficultLevel LvlDifficult = DifficultManager.DifficultLevel.Easy;

    private void Start()
    {
        player.UIManager.pauseButton.enabled = false;
        AddMoveHelpLabel();
        currentState = TrainState.move;
    }

    private void Update()
    {
        if (currentState == TrainState.move && player.Tractor.isMoving)
        {
            RemoveHelpLabel();
            AddScoreHelpLabel();
            currentState = TrainState.score;
        }

        if (currentState == TrainState.score && player.HarvestScore >= player.HarvestScoreToWin)
        {
            RemoveHelpLabel();
            AddFinishHelpLabel();
            currentState = TrainState.finish;
        }
    }

    private void AddMoveHelpLabel() => currentHelpLabel = Instantiate(moveHelpLabel, trainCanvas);
    private void AddScoreHelpLabel() => currentHelpLabel = Instantiate(scoreHelpLabel, trainCanvas);
    private void AddFinishHelpLabel() => currentHelpLabel = Instantiate(finishHelpLabel, trainCanvas);

    private void RemoveHelpLabel() => Destroy(currentHelpLabel);
}
