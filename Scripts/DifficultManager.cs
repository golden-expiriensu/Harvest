using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultManager : MonoBehaviour
{
    [SerializeField] private Player _player;

    public DifficultLevel CurrentDifficultLevel { get; set; } = DifficultLevel.Medium;
    public float[] DifficultLevelFactors = new float[3] { 1.1f, 1f, 0.9f };

    public enum DifficultLevel
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public void SetRequirementsForPassing(List<Vector2Int> passingPath)
    {
        _player.Tractor.SetMaxFuel(Mathf.Clamp(
            CalculateDifficultFactor(CurrentDifficultLevel)
            * passingPath.Count
            * FuelFunction(_player.Field.CurrentSize),
            passingPath.Count, 999));

        _player.Tractor.SetMaxHealth(Mathf.Clamp(
            CalculateDifficultFactor(CurrentDifficultLevel)
            * HealthFunction(_player.Field.CurrentSize),
            1, 9));


        float ScoreToWin = 0;
        foreach (Vector2Int cell in passingPath)
            if (_player.Field.GetObjectOnCell(cell) != null)
                if (_player.Field.GetObjectOnCell(cell).TryGetComponent(out Harvest harvest))
                    ScoreToWin += harvest.benefinAmount;

        int maxHarvestAmount = 0;
        foreach (ObjectOnField objectOnField in _player.Field.ObjectsOnField)
            if (objectOnField != null)
                if (objectOnField.TryGetComponent(out Harvest harvest))
                    maxHarvestAmount += harvest.benefinAmount;

        ScoreToWin = Mathf.Clamp(
            (ScoreToWin * ScoreFunction(_player.Field.CurrentSize)) / CalculateDifficultFactor(CurrentDifficultLevel),
            ScoreToWin,
            maxHarvestAmount * 0.9f);

        _player.SetHarvestScoreToWin(ScoreToWin);
    }

    private float HealthFunction(Vector2Int size)
    {
        int x = size.x + size.y;
        float y = 4f * x / 20f + 1f / 3f;
        return y;
    }

    private float FuelFunction(Vector2Int size)
    {
        int x = size.x + size.y;
        float y = 13f / (1 + Mathf.Exp((25f - x) / 9f));
        return y;
    }

    private float ScoreFunction(Vector2Int size)
    {
        int x = size.x + size.y;
        float y = 14f / (1 + Mathf.Exp((35 - x) / 13));
        return y;
    }

    private float CalculateDifficultFactor(DifficultLevel level)
    {
        switch (level)
        {
            case DifficultLevel.Easy:
                return DifficultLevelFactors[0];
            case DifficultLevel.Medium:
                return DifficultLevelFactors[1];
            case DifficultLevel.Hard:
                return DifficultLevelFactors[2];
            default:
                return 1;
        }
    }

    public int CalculateVoidCells(Vector2Int size)
    {
        return 2 * size.x * size.y - 1;
    }

    public int CalculateFilledCells(Vector2Int size)
    {
        return 2 * (size.x * size.y - (size.x + size.y) + 1);
    }
}
