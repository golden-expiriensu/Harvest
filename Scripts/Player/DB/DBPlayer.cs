using Mono.Data.Sqlite;

public class DBPlayer : DBManager
{
    public void SetCurrentLvl(int value)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE player SET currentStoryLvl = @currentStoryLvl";
        dbcmd.Parameters.Add(new SqliteParameter("@currentStoryLvl", value));
        ExecuteCommand();
        CloseDB();
    }

    public int GetCurrentLvl()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT currentStoryLvl FROM player";
        ExecuteCommand();

        reader.Read();
        int value = reader.GetInt32(0);

        CloseDB();
        return value;
    }

    public void SetMaxLvl(int value)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE player SET maxStoryLvl = @maxStoryLvl";
        dbcmd.Parameters.Add(new SqliteParameter("@maxStoryLvl", value));
        ExecuteCommand();
        CloseDB();
    }

    public int GetMaxLvl()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT maxStoryLvl FROM player";
        ExecuteCommand();

        reader.Read();
        int value = reader.GetInt32(0);

        CloseDB();
        return value;
    }

    public void SetJoystickDeadZone(float value)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE player SET joystickDeadZone = @joystickDeadZone";
        dbcmd.Parameters.Add(new SqliteParameter("@joystickDeadZone", value));
        ExecuteCommand();
        CloseDB();
    }

    public float GetJoystickDeadZone()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT joystickDeadZone FROM player";
        ExecuteCommand();

        reader.Read();
        float value = reader.GetFloat(0);

        CloseDB();
        return value;
    }

    public void SetJoystickSize(float value)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE player SET joystickSize = @joystickSize";
        dbcmd.Parameters.Add(new SqliteParameter("@joystickSize", value));
        ExecuteCommand();
        CloseDB();
    }

    public float GetJoystickSize()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT joystickSize FROM player";
        ExecuteCommand();

        reader.Read();
        float value = reader.GetFloat(0);

        CloseDB();
        return value;
    }

    public void SetJoystickIsDynamic(bool value)
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "UPDATE player SET joystickIsDynamic = @joystickIsDynamic";
        dbcmd.Parameters.Add(new SqliteParameter("@joystickIsDynamic", value));
        ExecuteCommand();
        CloseDB();
    }

    public bool GetJoystickIsDynamic()
    {
        OpenDB_And_CreateCommand();
        dbcmd.CommandText = "SELECT joystickIsDynamic FROM player";
        ExecuteCommand();

        reader.Read();
        bool value = reader.GetBoolean(0);

        CloseDB();
        return value;
    }

}
