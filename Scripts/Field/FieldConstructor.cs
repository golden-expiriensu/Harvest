using System.Collections.Generic;
using UnityEngine;

public class FieldConstructor : MonoBehaviour
{
    public Player _player;
    public FieldFiller constructor;
    private MazeGenerator mazeGenerator;

    private Vector2Int fieldSize;
    public ObjectOnField[,] ObjectsOnField { get; private set; }
    public GameObject[,] GameObjectsOnField { get; private set; }

    public Vector2Int Start { get; set; }
    public Vector2Int Finish { get; set; }

    public Vector2Int CurrentSize { get; set; }

    public GameObject VoidObject;

    public int healthBenefitFactor = 1; //repair kit add healthFactor health amount
    public int fuelDistributionFactor = 10; //gas canister add fuelDistributionFactor fuel amount

    //TODO rename/delete this shit
    public Finish Finish_conflict_with_v2int { get; set; }

    public bool[,] maze;

    public delegate void GameCreate();
    public event GameCreate OnGameCreated;

    private void Awake()
    {
        mazeGenerator = GetComponent<MazeGenerator>();
        constructor = GetComponent<FieldFiller>();
    }

    public void CreateGame()
    {
        _player.DBGames.SetNeedCreateGame(false);
        _player.DifficultManager.SetRequirementsForPassing(FillFieldForCreatingGame());
        _player.DBGames.SetNeedOriginalSeed(true);
        TeleportPlayerToSpawn();
        OnGameCreated.Invoke();

        Finish_conflict_with_v2int = GetObjectOnCell(Finish).GetComponent<Finish>();
        Finish_conflict_with_v2int.SetConnectionWithPlayer(_player);
    }

    private List<Vector2Int> FillFieldForCreatingGame()
    {
        Vector2Int size = _player.DBGames.GetGameSizes();
        _player.DifficultManager.CurrentDifficultLevel = _player.DBGames.GetGameDifficult();

        maze = mazeGenerator.GenerateMaze(new int[] { size.x, size.y });
        GameObjectsOnField = new GameObject[maze.GetLength(0), maze.GetLength(1)];

        CurrentSize = size;
        constructor.CreateField(new Vector2Int(size.x, size.y), maze);
        _player.DBGames.SetIsItFirstSave(true);

        return GetComponent<MazePasser>().NegrKalian(maze, Start, Finish);
    }

    public List<Vector2Int> FillFieldForLoadingGame()
    {
        _player.DifficultManager.CurrentDifficultLevel = _player.DBGames.GetGameDifficult();

        Vector2Int size = _player.DBGames.GetGameSizes();
        CurrentSize = size;

        _player.DBGames.GetGameMaze(out maze, size);
        constructor.CurrentMaze = maze;

        List<string> seed;
        if (_player.DBGames.GetIsNeedSeedForRestart())
            _player.DBGames.GetGameOriginalSeed(out seed);
        else
            _player.DBGames.GetGameSeed(out seed);
        _player.SeedHandler.CurrentSeed = seed;

        GameObjectsOnField = new GameObject[maze.GetLength(0), maze.GetLength(1)];

        constructor.FillField(new Vector2Int(size.x, size.y), seed);
        RotateStartTractorBySeed(_player.Field.Start);
        RotateFinishBySeed(_player.Field.Finish);

        return GetComponent<MazePasser>().NegrKalian(maze, Start, Finish);
    }

    public void TeleportPlayerToSpawn()
    {
        Vector2Int spawn;
        if (_player.DBGames.GetIsNeedSeedForRestart())
            spawn = Start;
        else spawn = _player.DBGames.GetGameSpawn();
        _player.Tractor.thisTransform.position = new Vector3(spawn.x, _player.Tractor.thisTransform.position.y, spawn.y);
        _player.Tractor.startPosition = new Vector3(spawn.x, _player.Tractor.thisTransform.position.y, spawn.y);
        _player.Controls.currentCell = _player.Tractor.startPosition;
    }

    public bool IsCellAvailable(Vector2Int cell)
    {
        if (cell.x < 0 || cell.y < 0 || cell.x >= fieldSize.x || cell.y >= fieldSize.y
            || (ObjectsOnField[cell.x, cell.y] != null && !ObjectsOnField[cell.x, cell.y].destroyable))
            return false;
        return true;
    }

    public bool IsCellFree(Vector2Int cell)
    {
        if (ObjectsOnField[cell.x, cell.y] != null) return false;
        return true;
    }

    public void MoveTractorToCell(Vector2Int vector2)
    {
        if (vector2 == Finish) Finish_conflict_with_v2int.PlayerStepOnFinish();
        else
        {
            if (ObjectsOnField[vector2.x, vector2.y] != null
                && vector2 != Start
                && vector2 != Finish)
            {
                if (ObjectsOnField[vector2.x, vector2.y].TryGetComponent(out Demolishable demolishable))
                    demolishable.Demolish(_player);
            }
            _player.Tractor.ReduceFuel(1);
            if (_player.Tractor.СurrentHealth <= 0) _player.Tractor.Broke();
        }
    }

    public void AddObjectInCell(GameObject gameObject, Vector2Int vector2)
    {
        if (gameObject != null &&
            IsCellAvailable(vector2))
        {
            GameObjectsOnField[vector2.x, vector2.y] = gameObject;

            ObjectsOnField[vector2.x, vector2.y] = Instantiate(gameObject, new Vector3(vector2.x, 0, vector2.y), Quaternion.identity).GetComponentInChildren<ObjectOnField>();
            if (_player.SeedHandler.needToRotatePrefabOn90DegreesThenInstantiated)
            {
                Transform tr = ObjectsOnField[vector2.x, vector2.y].gameObject.GetComponentsInChildren<Transform>()[1];
                tr.eulerAngles = new Vector3(0, tr.eulerAngles.y + 90, 0);
            }
            ObjectsOnField[vector2.x, vector2.y].gameObject.isStatic = true;
        }
    }

    public void RemoveObjectFromFieldMemory(Vector2Int vector2)
    {
        ObjectsOnField[vector2.x, vector2.y] = null;
        GameObjectsOnField[vector2.x, vector2.y] = null;
    }

    public ObjectOnField GetObjectOnCell(Vector2Int coord)
    {
        return ObjectsOnField[coord.x, coord.y];
    }

    public void ReserveCell(Vector2Int vector2)
    {
        ObjectsOnField[vector2.x, vector2.y] = Instantiate(VoidObject, new Vector3(vector2.x, 0, vector2.y), Quaternion.identity).GetComponent<ObjectOnField>();
    }

    public void SetFieldSize(Vector2Int newSize)
    {
        fieldSize = newSize;
        ObjectsOnField = new ObjectOnField[fieldSize.x, fieldSize.y];
    }

    private void RotateStartTractorBySeed(Vector2Int coord)
    {
        _player.Field.Start = coord;

        switch (_player.SeedHandler.StartAndFinishSeed)
        {
            case 0:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 180, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 180, 0);
                break;
            case 1:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 0, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 2:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 270, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 270, 0);
                break;
            case 3:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0);
                _player.Tractor.CurrentModelTransform.eulerAngles = new Vector3(0, 90, 0);
                break;
        }
    }

    private void RotateFinishBySeed(Vector2Int coord)
    {
        _player.Field.Finish = coord;
        switch (_player.SeedHandler.StartAndFinishSeed)
        {
            case 0:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 0, 0);
                break;
            case 1:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 180, 0);
                break;
            case 2:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 90, 0);
                break;
            case 3:
                _player.Field.GetObjectOnCell(coord).GetComponentsInChildren<Transform>()[1].eulerAngles = new Vector3(0, 270, 0);
                break;
        }
    }
}
