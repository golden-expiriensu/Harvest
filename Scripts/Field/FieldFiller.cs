using System.Collections.Generic;
using UnityEngine;

public class FieldFiller : MonoBehaviour
{
    public Player _player;

    private MazeBoolCellToFieldPrefabTransformator _mazeBoolCellToFieldPrefabTransformator;
    private StartAndFinishCreator _startAndFinishCreator;

    #region prefabs

    //harvests
    public  GameObject[] ostovsPrefab;
    public  GameObject[] harvestPrefabs;
    public  float[] chancesToGenereteCorrespondingHarvest;
    //obstacles
    public  GameObject[] fencePrefabs;
    public  float[] chancesToGenereteCorrespondingFence;
    public  GameObject[] obstaclePrefabs;
    public  float[] chancesToGenereteCorrespondingObstacle;
    public  GameObject[] outMazeObjectsPrefabs;
    public  float[] chancesToGenereteCorrespondingOutMazeObject;
    //bonuses
    public  GameObject gasCanisterPrefab;
    public  float chanceToGenereteGasCanister;
    public  GameObject repairBoxPrefab;
    public  float chanceToGenereteRepairBox;
    //check points
    public  GameObject startPrefab;
    public  GameObject finishPrefab;

    #endregion

    public bool[,] CurrentMaze { get; set; }

    //decor in the background
    [SerializeField] private int forestOutOfMazeLenght = 5;
    [SerializeField] private float forestOutOfMazeChance = 0.5f;
    
    //start and finish
    private int _checkPointsSizes = 4;

    private void Awake()
    {
        _mazeBoolCellToFieldPrefabTransformator = new MazeBoolCellToFieldPrefabTransformator(_player, this);
        _startAndFinishCreator = new StartAndFinishCreator(_player, this);
    }

    public void CreateField(Vector2Int size, bool[,] maze)
    {
        CurrentMaze = maze;

        Vector2Int realSize = size * 2 + Vector2Int.one;
        _player.Field.SetFieldSize(realSize);

        _startAndFinishCreator.AddFinishAndStart(maze);
        _player.DBGames.SetGameSpawn(_player.Field.Start);
        AddBonuses(size, maze);


        for (int i = 0; i < realSize.x; i++)
            for (int k = 0; k < realSize.y; k++)
                if (_player.Field.IsCellFree(new Vector2Int(i, k)))
                    _player.Field.AddObjectInCell(_mazeBoolCellToFieldPrefabTransformator.TransformBoolMazeCellIntoPrefab(maze, new Vector2Int(i, k), size), new Vector2Int(i, k));

        AddObjectsOutOfMaze(size);
    }

    public void FillField(Vector2Int size, List<string> seed)
    {
        Vector2Int realSize = size * 2 + Vector2Int.one;
        _player.Field.SetFieldSize(realSize);

        int n = 0;

        for (int i = 0; i < realSize.x; i++)
            for (int k = 0; k < realSize.y; k++)
            {
                _player.Field.AddObjectInCell(_player.SeedHandler.SeedDecoder(seed[n], new Vector2Int(i, k)), new Vector2Int(i, k));

                n++;
            }

        AddObjectsOutOfMaze(size);
    }

    private void AddObjectsOutOfMaze(Vector2Int size)
    {
        Vector2Int realSize = size * 2 + Vector2Int.one;
        for (int i = -forestOutOfMazeLenght; i < realSize.x + forestOutOfMazeLenght; i++)
            for (int k = -forestOutOfMazeLenght; k < realSize.y + forestOutOfMazeLenght; k++)
            {
                if (i >= 0 && i < realSize.x && k >= 0 && k < realSize.y || IsVectorLocatedOnStartOrFinishPlace(new Vector2Int(i, k)))
                {
                    continue;
                }
                if (Random.value < 1 - forestOutOfMazeChance) continue;

                int chose = GetElementFromArrayByItChance(chancesToGenereteCorrespondingOutMazeObject);

                GameObject obj;
                switch (chose)
                {
                    case 0:
                        obj = Instantiate(outMazeObjectsPrefabs[0], new Vector3(i, 0, k), Quaternion.identity);
                        break;
                    case 1:
                        obj = Instantiate(outMazeObjectsPrefabs[1], new Vector3(i, 0, k), Quaternion.identity);
                        break;
                    default:
                        continue;
                }
                obj.gameObject.isStatic = true;
                obj.GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 0 + 90 * Random.Range(1, 4), 0);
            }
    }

    //                |
    //TODO: ugly shit | here
    //               \|/
    //                V
    private bool IsVectorLocatedOnStartOrFinishPlace(Vector2Int vector)
    {
        Vector2Int[] forbidenPlace = new Vector2Int[2] { _player.Field.Start, _player.Field.Finish };
        Vector2Int realSize = _player.Field.CurrentSize * 2 + Vector2Int.one;
        for (int i = 0; i < 2; i++)
        {
            if (forbidenPlace[i].x == 0 || forbidenPlace[i].x + 1 == realSize.x)
            {
                if (forbidenPlace[i].x == 0)
                {
                    if (vector.x > -_checkPointsSizes && vector.x < 0
                        && vector.y > forbidenPlace[i].y - Mathf.Ceil(_checkPointsSizes / 2f)
                        && vector.y < forbidenPlace[i].y + Mathf.Ceil(_checkPointsSizes / 2f))
                        return true;
                }
                else
                {
                    if (vector.x > forbidenPlace[i].x && vector.x < forbidenPlace[i].x + _checkPointsSizes
                        && vector.y > forbidenPlace[i].y - Mathf.Ceil(_checkPointsSizes / 2f)
                        && vector.y < forbidenPlace[i].y + Mathf.Ceil(_checkPointsSizes / 2f))
                        return true;
                }
            }
            else
            {
                if (forbidenPlace[i].y == 0)
                {
                    if (vector.y > -_checkPointsSizes && vector.y < 0
                        && vector.x > forbidenPlace[i].x - Mathf.Ceil(_checkPointsSizes / 2f)
                        && vector.x < forbidenPlace[i].x + Mathf.Ceil(_checkPointsSizes / 2f))
                        return true;
                }
                else
                {
                    if (vector.y < forbidenPlace[i].y + _checkPointsSizes && vector.y > forbidenPlace[i].y
                        && vector.x > forbidenPlace[i].x - Mathf.Ceil(_checkPointsSizes / 2f)
                        && vector.x < forbidenPlace[i].x + Mathf.Ceil(_checkPointsSizes / 2f))
                        return true;
                }
            }
        }

        return false;
    }

    private void AddBonuses(Vector2Int size, bool[,] maze)
    {
        List<Vector2Int> freeCells = new List<Vector2Int>();
        for (int i = 1; i < maze.GetLength(0) - 2; i++)
            for (int k = 1; k < maze.GetLength(1) - 2; k++)
                if (!maze[i, k]) freeCells.Add(new Vector2Int(i, k));

        int gasCanisterAmount = Mathf.FloorToInt((2 * size.x * size.y - 1) * chanceToGenereteGasCanister);//TODO transfer to difficult manager
        int repairBoxAmount = Mathf.FloorToInt((2 * size.x * size.y - 1) * chanceToGenereteRepairBox);

        while (gasCanisterAmount > 0)
        {
            int i = Random.Range(0, freeCells.Count);
            _player.Field.AddObjectInCell(gasCanisterPrefab, freeCells[i]);
            freeCells.RemoveAt(i);
            gasCanisterAmount--;
        }
        while (repairBoxAmount > 0)
        {
            int i = Random.Range(0, freeCells.Count);
            _player.Field.AddObjectInCell(repairBoxPrefab, freeCells[i]);
            freeCells.RemoveAt(i);
            repairBoxAmount--;
        }
    }

    //                |
    //TODO: this func | fully copied from MazeBoolCellToFieldPrefabTransformator
    //               \|/
    //                V
    private int GetElementFromArrayByItChance(float[] chances)
    {
        float value = Random.value;
        float currentSumChance = 0;

        for (int i = 0; i < chances.Length; i++)
        {
            currentSumChance += chances[i];
            if (currentSumChance >= value)
                return i;
        }
        return chances.Length;
    }
}

public class StartAndFinishCreator
{
    private Player _player;
    private FieldFiller _fieldFiller;

    public StartAndFinishCreator(Player player, FieldFiller fieldFiller)
    {
        _player = player;
        _fieldFiller = fieldFiller;
    }

    //                |
    //TODO: ugly shit | here
    //               \|/
    //                V
    public void AddFinishAndStart(bool[,] maze)
    {
        if (Random.value > 0.5)
        {
            List<Vector2Int> freeTopCells = new List<Vector2Int>();
            List<Vector2Int> freeBottomCells = new List<Vector2Int>();
            for (int i = 1; i < maze.GetLength(0) - 1; i++)
            {
                if (maze[i, 1] == false)
                    freeTopCells.Add(new Vector2Int(i, 0));
                if (maze[i, maze.GetLength(1) - 2] == false)
                    freeBottomCells.Add(new Vector2Int(i, maze.GetLength(1) - 1));
            }

            if (Random.value > 0.5)
            {
                _player.SeedHandler.StartAndFinishSeed = 0;
                CreateStart(freeTopCells).eulerAngles = new Vector3(0, 180, 0);
                CreateFinish(freeBottomCells).eulerAngles = new Vector3(0, 0, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                _player.SeedHandler.StartAndFinishSeed = 1;
                CreateStart(freeBottomCells).eulerAngles = new Vector3(0, 0, 0);
                CreateFinish(freeTopCells).eulerAngles = new Vector3(0, 180, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
        else
        {
            List<Vector2Int> freeLeftCells = new List<Vector2Int>();
            List<Vector2Int> freeRightCells = new List<Vector2Int>();
            for (int i = 1; i < maze.GetLength(1) - 1; i++)
            {
                if (maze[1, i] == false)
                    freeLeftCells.Add(new Vector2Int(0, i));
                if (maze[maze.GetLength(0) - 2, i] == false)
                    freeRightCells.Add(new Vector2Int(maze.GetLength(0) - 1, i));
            }

            if (Random.value > 0.5)
            {
                _player.SeedHandler.StartAndFinishSeed = 2;
                CreateStart(freeLeftCells).eulerAngles = new Vector3(0, 270, 0);
                CreateFinish(freeRightCells).eulerAngles = new Vector3(0, 90, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 270, 0);
            }
            else
            {
                _player.SeedHandler.StartAndFinishSeed = 3;
                CreateStart(freeRightCells).eulerAngles = new Vector3(0, 90, 0);
                CreateFinish(freeLeftCells).eulerAngles = new Vector3(0, 270, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
    }

    private Transform CreateStart(List<Vector2Int> freeCells)
    {
        _player.Field.Start = freeCells[Random.Range(0, freeCells.Count)];
        _player.Field.ReserveCell(_player.Field.Start);
        _player.Field.AddObjectInCell(_fieldFiller.startPrefab, _player.Field.Start);
        return _player.Field.GetObjectOnCell(_player.Field.Start).GetComponentsInChildren<Transform>()[1];
    }

    private Transform CreateFinish(List<Vector2Int> freeCells)
    {
        _player.Field.Finish = freeCells[Random.Range(0, freeCells.Count)];
        _player.Field.AddObjectInCell(_fieldFiller.finishPrefab, _player.Field.Finish);
        return _player.Field.GetObjectOnCell(_player.Field.Finish).GetComponentsInChildren<Transform>()[1];
    }
}

public class MazeBoolCellToFieldPrefabTransformator
{
    private Player _player;
    private FieldFiller _fieldFiller;

    public MazeBoolCellToFieldPrefabTransformator(Player player, FieldFiller fieldFiller)
    {
        _player = player;
        _fieldFiller = fieldFiller;
    }

    public GameObject TransformBoolMazeCellIntoPrefab(bool[,] maze, Vector2Int coord, Vector2Int size)
    {
        _player.SeedHandler.needToRotatePrefabOn90DegreesThenInstantiated = false;
        if (IsItBorderCell(ref coord, size)) return PlaceObstacles();

        if (maze[coord.x, coord.y] == false) //empty cell
            return PlaceHarvest();
        else // (maze[coord.x, coord.y] == true) //wall
            return SetWalls(maze, coord);
    }

    private bool IsItBorderCell(ref Vector2Int coord, Vector2Int size)
    {
        return ((coord.x != _player.Field.Start.x || coord.y != _player.Field.Start.y)
                    && (coord.x != _player.Field.Finish.x || coord.y != _player.Field.Finish.y)) &&
                    (coord.x == 0 || coord.y == 0 || coord.x == (size.x * 2) || coord.y == (size.y * 2));
    }

    private GameObject SetWalls(bool[,] maze, Vector2Int coord)
    {
        float value = Random.value;
        if (value < 0.85) //TODO: magic number change to DifficultManager.SomeFunc()
            return PlaceObstacles();
        else
        {
            if (NeedToPlaceObstacles(maze, coord))
                return PlaceObstacles();
            else return PlaceFences();
        }
    }

    private bool NeedToPlaceObstacles(bool[,] maze, Vector2Int coord)
    {
        int leftWall = maze[coord.x - 1, coord.y] == true ? 1 : 0;
        int rightWall = maze[coord.x + 1, coord.y] == true ? 1 : 0;
        int topWall = maze[coord.x, coord.y + 1] == true ? 1 : 0;
        int bottomWall = maze[coord.x, coord.y - 1] == true ? 1 : 0;

        int wallNeighborsAmount = leftWall + rightWall + topWall + bottomWall;

        DetermineWillBeItNessesaryToRotatePrefabAfterwards(leftWall);

        return wallNeighborsAmount != 2 || !(leftWall == rightWall || topWall == bottomWall);
    }

    private void DetermineWillBeItNessesaryToRotatePrefabAfterwards(int leftWall)
    {
        if (leftWall == 1) _player.SeedHandler.needToRotatePrefabOn90DegreesThenInstantiated = false;
        else _player.SeedHandler.needToRotatePrefabOn90DegreesThenInstantiated = true;
    }

    private GameObject PlaceFences()
    {
        int chose = GetElementFromArrayByItChance(_fieldFiller.chancesToGenereteCorrespondingFence);

        if (IndexIsContainedInPrefabsArray(chose, _fieldFiller.fencePrefabs))
            return _fieldFiller.fencePrefabs[chose];
        else return null;
    }

    private GameObject PlaceObstacles()
    {
        int chose = GetElementFromArrayByItChance(_fieldFiller.chancesToGenereteCorrespondingObstacle);

        if (IndexIsContainedInPrefabsArray(chose, _fieldFiller.obstaclePrefabs))
            return _fieldFiller.obstaclePrefabs[chose];
        else return null;
    }

    private GameObject PlaceHarvest()
    {
        int chose = GetElementFromArrayByItChance(_fieldFiller.chancesToGenereteCorrespondingHarvest);

        if (IndexIsContainedInPrefabsArray(chose, _fieldFiller.harvestPrefabs))
            return _fieldFiller.harvestPrefabs[chose];
        else return null;
    }

    private bool IndexIsContainedInPrefabsArray(int index, GameObject[] array)
    {
        return (index >= 0 && index < array.Length);
    }

    //not depend from difficult
    private int GetElementFromArrayByItChance(float[] chances)
    {
        float value = Random.value;
        float currentSumChance = 0;

        for (int i = 0; i < chances.Length; i++)
        {
            currentSumChance += chances[i];
            if (currentSumChance >= value)
                return i;
        }
        return chances.Length;
    }
}
