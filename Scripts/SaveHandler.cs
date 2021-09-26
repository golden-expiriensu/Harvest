using System.Collections.Generic;
using UnityEngine;

namespace SavingSystem
{
    public class SaveHandler : MonoBehaviour
    {
        private Player _player;
        public Saver Saver { get; private set; }
        public Loader Loader { get; private set; }
        public SeedHandler SeedHandler;

        private void Awake()
        {
            _player = GetComponent<Player>();

            SeedHandler = new SeedHandler(_player);
            Saver = new Saver(_player, SeedHandler);
            Loader = new Loader(_player, SeedHandler);

            _player.Field.OnGameCreated += SeedHandler.UpdateFieldSeed;
            _player.Field.OnGameCreated += Saver.SaveGame;

            _player.InitializeSavingSystem(Saver, Loader, SeedHandler);
        }
    }

    public class Saver
    {
        private Player _player;
        private SeedHandler _seedHandler;

        public Saver(Player player, SeedHandler seedHandler)
        {
            _player = player;
            _seedHandler = seedHandler;
        }

        public void SaveGame()
        {
            Debug.Log("saving...");

            if (_player.DBGames.IsItFirstSave())
            {
                InitializeCreatedGame();
            }

            _seedHandler.UpdateFieldSeed();

            WriteParametersInDB();
        }

        private void InitializeCreatedGame()
        {
            _player.DBGames.SetGameOriginalSeed(_seedHandler.CurrentSeed);
            _player.DBGames.SetIsItFirstSave(false);
        }

        private void WriteParametersInDB()
        {
            _player.DBGames.SetFuelAmount(_player.Tractor.СurrentFuel);
            _player.DBGames.SetHealthAmount(_player.Tractor.СurrentHealth);
            _player.DBGames.SetGameSpawn(new Vector2Int((int)_player.Tractor.thisTransform.position.x, (int)_player.Tractor.thisTransform.position.z));
            _player.DBGames.SetGameSeed(_seedHandler.CurrentSeed);
            _player.DBGames.SetGameSizes(_player.Field.CurrentSize);
            _player.DBGames.SetGameMaze(_player.Field.constructor.CurrentMaze);
        }
    }

    public class Loader
    {
        private Player _player;
        private SeedHandler _seedHandler;

        public Loader(Player player, SeedHandler seedHandler)
        {
            _player = player;
            _seedHandler = seedHandler;
        }
        public void LoadGame()
        {
            Debug.Log("loading...");

            _player.DifficultManager.SetRequirementsForPassing(_player.Field.FillFieldForLoadingGame());
            if (!_player.DBGames.GetIsNeedSeedForRestart())
            {
                _player.Tractor.SetСurrentFuel(_player.DBGames.GetFuelAmount());
                _player.Tractor.SetСurrentHealth(_player.DBGames.GetHealthAmount());
            }
            
            _player.Field.TeleportPlayerToSpawn();

            _player.Field.Finish_conflict_with_v2int = _player.Field.GetObjectOnCell(_player.Field.Finish).GetComponent<Finish>();
            _player.Field.Finish_conflict_with_v2int.SetConnectionWithPlayer(_player);
        }
    }

    public class SeedHandler
    {
        private Player _player;
        public List<string> CurrentSeed { get; set; } = new List<string>();
        public int StartAndFinishSeed { get; set; }

        public bool needToRotatePrefabOn90DegreesThenInstantiated;

        public SeedHandler(Player player)
        {
            _player = player;
        }

        public void ClearSeed() => CurrentSeed.Clear();
        public void AddObjectInSeed(GameObject gameObj) => CurrentSeed.Add(SeedCoder(gameObj));

        public void UpdateFieldSeed()
        {
            ClearSeed();

            for (int i = 0; i < _player.Field.ObjectsOnField.GetLength(0); i++)
                for (int k = 0; k < _player.Field.ObjectsOnField.GetLength(1); k++)
                    AddObjectInSeed(_player.Field.GameObjectsOnField[i, k]);
        }

        private string SeedCoder(GameObject gameObj)
        {
            if (gameObj == _player.Field.constructor.obstaclePrefabs[0])
                return "o.0";
            else if (gameObj == _player.Field.constructor.obstaclePrefabs[1])
                return "o.1";
            else if (gameObj == _player.Field.constructor.obstaclePrefabs[2])
                return "o.2";
            else if (gameObj == _player.Field.constructor.harvestPrefabs[0])
                return "h.0";
            else if (gameObj == _player.Field.constructor.harvestPrefabs[1])
                return "h.1";
            else if (gameObj == _player.Field.constructor.harvestPrefabs[2])
                return "h.2";
            else if (gameObj == _player.Field.constructor.ostovsPrefab[0])
                return "ost.0";
            else if (gameObj == _player.Field.constructor.ostovsPrefab[1])
                return "ost.1";
            else if (gameObj == _player.Field.constructor.fencePrefabs[0])
                return "f.0";
            else if (gameObj == _player.Field.constructor.fencePrefabs[1])
                return "f.1";
            else if (gameObj == _player.Field.constructor.fencePrefabs[2])
                return "f.2";
            else if (gameObj == _player.Field.constructor.fencePrefabs[3])
                return "f.3";
            else if (gameObj == _player.Field.constructor.gasCanisterPrefab)
                return "g.0";
            else if (gameObj == _player.Field.constructor.repairBoxPrefab)
                return "r.0";
            else if (gameObj == _player.Field.constructor.startPrefab)
                return "s." + StartAndFinishSeed + "." + _player.Field.Start.x + "." + _player.Field.Start.y;
            else if (gameObj == _player.Field.constructor.finishPrefab)
                return "e." + StartAndFinishSeed + "." + _player.Field.Finish.x + "." + _player.Field.Finish.y;
            else return "n.0";
        }

        public GameObject SeedDecoder(string seed, Vector2Int coord)
        {
            needToRotatePrefabOn90DegreesThenInstantiated = false;
            if (seed == "o.0")
                return _player.Field.constructor.obstaclePrefabs[0];
            else if (seed == "o.1")
                return _player.Field.constructor.obstaclePrefabs[1];
            else if (seed == "o.2")
                return _player.Field.constructor.obstaclePrefabs[2];
            else if (seed == "h.0")
                return _player.Field.constructor.harvestPrefabs[0];
            else if (seed == "h.1")
                return _player.Field.constructor.harvestPrefabs[1];
            else if (seed == "h.2")
                return _player.Field.constructor.harvestPrefabs[2];
            else if (seed == "ost.0")
                return _player.Field.constructor.ostovsPrefab[0];
            else if (seed == "ost.1")
                return _player.Field.constructor.ostovsPrefab[1];
            else if (seed == "f.0")//TODO: do rotate in seed, not that
            {
                if (!HaveLeftWall(coord)) needToRotatePrefabOn90DegreesThenInstantiated = true;
                return _player.Field.constructor.fencePrefabs[0];
            }
            else if (seed == "f.1")
            {
                if (!HaveLeftWall(coord)) needToRotatePrefabOn90DegreesThenInstantiated = true;
                return _player.Field.constructor.fencePrefabs[1];
            }
            else if (seed == "f.2")
            {
                if (!HaveLeftWall(coord)) needToRotatePrefabOn90DegreesThenInstantiated = true;
                return _player.Field.constructor.fencePrefabs[2];
            }
            else if (seed == "f.3")
            {
                if (!HaveLeftWall(coord)) needToRotatePrefabOn90DegreesThenInstantiated = true;
                return _player.Field.constructor.fencePrefabs[3];
            }
            else if (seed == "g.0")
                return _player.Field.constructor.gasCanisterPrefab;
            else if (seed == "r.0")
                return _player.Field.constructor.repairBoxPrefab;
            else if (seed.Remove(1) == "s")
            {
                string[] splitedSeed = seed.Split('.');
                StartAndFinishSeed = System.Convert.ToInt32(splitedSeed[1]);
                _player.Field.Start = new Vector2Int
                {
                    x = System.Convert.ToInt32(splitedSeed[2]),
                    y = System.Convert.ToInt32(splitedSeed[3])
                };
                return _player.Field.constructor.startPrefab;
            }
            else if (seed.Remove(1) == "e")
            {
                string[] splitedSeed = seed.Split('.');
                StartAndFinishSeed = System.Convert.ToInt32(splitedSeed[1]);
                _player.Field.Finish = new Vector2Int
                {
                    x = System.Convert.ToInt32(splitedSeed[2]),
                    y = System.Convert.ToInt32(splitedSeed[3])
                };
                return _player.Field.constructor.finishPrefab;
            }
            else return null;
        }
        public bool HaveLeftWall(Vector2Int coord)
        {
            return _player.Field.maze[coord.x - 1, coord.y];
        }
    }
}