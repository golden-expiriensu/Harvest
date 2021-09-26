using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

public class DBManager : MonoBehaviour
{
    protected SqliteConnection dbconn;
    protected SqliteCommand dbcmd;
    protected SqliteDataReader reader;
    protected string DatabaseName = "db.bytes";

    protected void OpenDB_And_CreateCommand()
    {
        string dbPath;
        if (Application.platform != RuntimePlatform.Android) dbPath = Path.Combine(Application.dataPath, DatabaseName);
        else dbPath = Path.Combine(Application.persistentDataPath, DatabaseName);

        if (!File.Exists(dbPath))
        {
            File.Create(dbPath).Dispose();
        }

        string connection = "URI=file:" + dbPath;

        if (File.ReadAllBytes(dbPath).Length == 0)
        {
            CreateGameTableInDB(connection);
            AddNewGameRowInDB(connection);

            CreatePlayerTableInDB(connection);
            AddNewPlayerRowInDB(connection);
        }

        dbconn = new SqliteConnection(connection);
        dbconn.Open();

        dbcmd = dbconn.CreateCommand();
    }

    private void CreateGameTableInDB(string connection)
    {
        string query = "CREATE TABLE IF NOT EXISTS game(" +
            "needCreateGame INTEGER," +
            "size_x INTEGER," +
            "size_y INTEGER," +
            "difficult INTEGER," +
            "maze TEXT," +
            "originalSeed TEXT," +
            "seed TEXT," +
            "needOriginalSeed INTEGER," +
            "lightingSeed INTEGER," +
            "spawn_x INTEGER," +
            "spawn_y INTEGER," +
            "fuelAmount REAL," +
            "healthAmount REAL," +
            "firstSave INTEGER);";

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void CreatePlayerTableInDB(string connection)
    {
        string query = "CREATE TABLE IF NOT EXISTS player(" +
            "maxStoryLvl INTEGER," +
            "currentStoryLvl INTEGER," +
            "joystickDeadZone REAL," +
            "joystickSize REAL," +
            "joystickIsDynamic INTEGER);";

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = query;
        dbcmd.ExecuteNonQuery();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void AddNewGameRowInDB(string connection)
    {
        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string query1 = "DELETE FROM game";
        dbcmd.CommandText = query1;
        dbcmd.ExecuteNonQuery();

        string query2 = "INSERT INTO game(needCreateGame, difficult, size_x, size_y, difficult, firstSave)" +
            "VALUES(1, 2, 5, 5, 1, 1);";
        dbcmd.CommandText = query2;

        dbcmd.ExecuteNonQuery();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void AddNewPlayerRowInDB(string connection)
    {
        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string query1 = "DELETE FROM player";
        dbcmd.CommandText = query1;
        dbcmd.ExecuteNonQuery();

        string query2 = "INSERT INTO player(maxStoryLvl, currentStoryLvl, joystickDeadZone, joystickSize, joystickIsDynamic)" +
            "VALUES(0, 0, 0.5, 0.7, 0);";
        dbcmd.CommandText = query2;

        dbcmd.ExecuteNonQuery();
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    protected void ExecuteCommand()
    {
        reader = dbcmd.ExecuteReader();
    }

    protected void CloseDB()
    {
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
}

