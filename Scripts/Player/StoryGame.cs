using System.Collections.Generic;
using UnityEngine;

public class StoryGame : MonoBehaviour
{
    public readonly Dictionary<int, Vector2Int> StoryLvlSize = new Dictionary<int, Vector2Int>()
    {
        { 1,  new Vector2Int(7, 7) },
        { 2,  new Vector2Int(6, 10) },
        { 3,  new Vector2Int(10, 7) },
        { 4,  new Vector2Int(5, 20) },
        { 5,  new Vector2Int(11, 11) },
        { 6,  new Vector2Int(20, 7) },
        { 7,  new Vector2Int(5, 30) },
        { 8,  new Vector2Int(12, 12) },
        { 9,  new Vector2Int(10, 20) },
        { 10, new Vector2Int(15, 15) }
    };

    public readonly Dictionary<int, DifficultManager.DifficultLevel> StoryLvlDifficult = new Dictionary<int, DifficultManager.DifficultLevel>()
    {
        { 1,  DifficultManager.DifficultLevel.Easy },
        { 2,  DifficultManager.DifficultLevel.Easy },
        { 3,  DifficultManager.DifficultLevel.Medium },
        { 4,  DifficultManager.DifficultLevel.Medium },
        { 5,  DifficultManager.DifficultLevel.Medium },
        { 6,  DifficultManager.DifficultLevel.Medium },
        { 7,  DifficultManager.DifficultLevel.Medium },
        { 8,  DifficultManager.DifficultLevel.Hard },
        { 9,  DifficultManager.DifficultLevel.Hard },
        { 10, DifficultManager.DifficultLevel.Hard }
    };
}
