
using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DBGame : DBManager
{
    public void SetGameSizes(Vector2Int size)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET size_x = @size_x, size_y = @size_y";

        dbcmd.Parameters.Add(new SqliteParameter("size_x", size.x));
        dbcmd.Parameters.Add(new SqliteParameter("size_y", size.y));

        ExecuteCommand();
        CloseDB();
    }

    public void SetGameMaze(bool[,] maze)
    {
        string serializedMaze = SerializeMaze(maze);
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET maze = @maze";
        dbcmd.Parameters.Add(new SqliteParameter("maze", serializedMaze));
        ExecuteCommand();
        CloseDB();
    }

    public string SerializeMaze(bool[,] maze)
    {
        string serializedMaze = "";

        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int k = 0; k < maze.GetLength(1); k++)
                serializedMaze += (maze[i, k] ? 1 : 0).ToString() + "_";
            serializedMaze = serializedMaze.Remove(serializedMaze.Length - 1); //removing last "_"
            serializedMaze += "#";
        }
        serializedMaze = serializedMaze.Remove(serializedMaze.Length - 1); //removing last "#"

        return serializedMaze;
    }

    public bool[,] GetGameMaze(out bool[,] maze, Vector2Int size)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT maze FROM game";

        ExecuteCommand();
        reader.Read();
        maze = UnserializeMaze(reader.GetString(0), size);
        CloseDB();

        return maze;
    }

    public bool[,] UnserializeMaze(string serializedMaze, Vector2Int size)
    {
        bool[,] unserializedMaze = new bool[size.x * 2 + 1, size.y * 2 + 1];

        string[] stringRows = serializedMaze.Split('#');
        List<string[]> stringMaze = new List<string[]>();
        for (int i = 0; i < size.x * 2 + 1; i++)
            stringMaze.Add(stringRows[i].Split('_'));

        for (int i = 0; i < size.x * 2 + 1; i++)
            for (int k = 0; k < size.y * 2 + 1; k++)
            {
                unserializedMaze[i, k] = Convert.ToInt32(stringMaze[i][k]) == 1;
            }

        return unserializedMaze;
    }

    public Vector2Int GetGameSizes()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT size_x, size_y FROM game";

        ExecuteCommand();
        reader.Read();
        Vector2Int size = new Vector2Int
        {
            x = reader.GetInt32(0),
            y = reader.GetInt32(1)
        };
        CloseDB();

        return size;
    }

    private string SerializeSeed(List<string> seed)
    {
        string serializedSeed = "";
        for (int k = 0; k < seed.Count; k++)
            serializedSeed += seed[k] + "_";
        serializedSeed = serializedSeed.Remove(serializedSeed.Length - 1); //removing last "_"
        return serializedSeed;
    }

    public void SetGameSeed(List<string> seed)
    {
        string serializedSeed = SerializeSeed(seed);
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET seed = @seed";

        dbcmd.Parameters.Add(new SqliteParameter("seed", serializedSeed));

        ExecuteCommand();
        CloseDB();
    }

    public void SetGameOriginalSeed(List<string> seed)
    {
        string serializedSeed = SerializeSeed(seed);
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET originalSeed = @seed";

        dbcmd.Parameters.Add(new SqliteParameter("seed", serializedSeed));

        ExecuteCommand();
        CloseDB();
    }

    private List<string> UnserializeSeed(string seed)
    {
        List<string> unserializedSeed;
        unserializedSeed = seed.Split('_').ToList();
        return unserializedSeed;
    }

    public void GetGameSeed(out List<string> unserializedSeed)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT seed FROM game";
        ExecuteCommand();
        reader.Read();
        unserializedSeed = UnserializeSeed(reader.GetString(0));
        CloseDB();
    }

    public void GetGameOriginalSeed(out List<string> unserializedSeed)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT originalSeed FROM game";
        ExecuteCommand();
        reader.Read();
        unserializedSeed = UnserializeSeed(reader.GetString(0));
        CloseDB();
    }

    public void SetNeedOriginalSeed(bool isNeed)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET needOriginalSeed = @needOriginalSeed";
        dbcmd.Parameters.Add(new SqliteParameter("@needOriginalSeed", isNeed));
        ExecuteCommand();
        CloseDB();
    }

    public bool GetIsNeedSeedForRestart()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT needOriginalSeed FROM game";
        ExecuteCommand();

        reader.Read();
        bool isNeed = reader.GetBoolean(0);

        CloseDB();
        return isNeed;
    }

    public void SetIsItFirstSave(bool isNeed)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET firstSave = @needOriginalSeed";
        dbcmd.Parameters.Add(new SqliteParameter("@needOriginalSeed", isNeed));
        ExecuteCommand();
        CloseDB();
    }

    public bool IsItFirstSave()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT firstSave FROM game";
        ExecuteCommand();

        reader.Read();
        bool isNeed = reader.GetBoolean(0);

        CloseDB();
        return isNeed;
    }
    public void SetNeedCreateGame(bool need)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET needCreateGame = @need";
        dbcmd.Parameters.Add(new SqliteParameter("need", need));
        ExecuteCommand();
        CloseDB();
    }

    public bool GetNeedCreateGame()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT needCreateGame FROM game";
        ExecuteCommand();

        reader.Read();
        bool need = reader.GetBoolean(0);

        CloseDB();
        return need;
    }

    public void SetGameDifficult(DifficultManager.DifficultLevel difficult = DifficultManager.DifficultLevel.Easy)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET difficult = @difficult";
        dbcmd.Parameters.Add(new SqliteParameter("difficult", (int)difficult));
        ExecuteCommand();
        CloseDB();
    }

    public DifficultManager.DifficultLevel GetGameDifficult()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT difficult FROM game";
        ExecuteCommand();

        reader.Read();
        int difficult = reader.GetInt32(0);
        DifficultManager.DifficultLevel level;
        switch (difficult)
        {
            case 1:
                level = DifficultManager.DifficultLevel.Easy;
                break;
            case 2:
                level = DifficultManager.DifficultLevel.Medium;
                break;
            case 3:
                level = DifficultManager.DifficultLevel.Hard;
                break;
            default:
                level = DifficultManager.DifficultLevel.Easy;
                break;
        }

        CloseDB();
        return level;
    }

    public void SetGameLighting(int seed)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET lightingSeed = @lightingSeed";
        dbcmd.Parameters.Add(new SqliteParameter("@lightingSeed", seed));
        ExecuteCommand();
        CloseDB();
    }

    public int GetGameLighting()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT lightingSeed FROM game";
        ExecuteCommand();

        reader.Read();
        int seed = reader.GetInt32(0);

        CloseDB();
        return seed;
    }

    public void SetGameSpawn(Vector2Int spawn)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET spawn_x = @spawn_x, spawn_y = @spawn_y";
        dbcmd.Parameters.Add(new SqliteParameter("@spawn_x", spawn.x));
        dbcmd.Parameters.Add(new SqliteParameter("@spawn_y", spawn.y));
        ExecuteCommand();
        CloseDB();
    }


    public Vector2Int GetGameSpawn()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT spawn_x, spawn_y FROM game";
        ExecuteCommand();

        reader.Read();
        Vector2Int spawn = new Vector2Int
        {
            x = reader.GetInt32(0),
            y = reader.GetInt32(1)
        };

        CloseDB();
        return spawn;
    }

    public void SetFuelAmount(float amount)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET fuelAmount = @fuelAmount";
        dbcmd.Parameters.Add(new SqliteParameter("@fuelAmount", amount));
        ExecuteCommand();
        CloseDB();
    }

    public float GetFuelAmount()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT fuelAmount FROM game";
        ExecuteCommand();

        reader.Read();
        float fuelAmount = reader.GetFloat(0);

        CloseDB();
        return fuelAmount;
    }

    public void SetHealthAmount(float amount)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE game SET healthAmount = @healthAmount";
        dbcmd.Parameters.Add(new SqliteParameter("@healthAmount", amount));
        ExecuteCommand();
        CloseDB();
    }

    public float GetHealthAmount()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT healthAmount FROM game";
        ExecuteCommand();

        reader.Read();
        float healthAmount = reader.GetFloat(0);

        CloseDB();
        return healthAmount;
    }
}
