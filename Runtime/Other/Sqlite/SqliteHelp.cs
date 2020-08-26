using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;

public class SqliteHelp {
    private string DBPath;
    private DBAccess db;
    private static SqliteHelp sqliteHelp;
    private SqliteHelp() {
        DBPath = Application.persistentDataPath + "/" + "DataBase.db";
    }
    public static SqliteHelp GetInstance() {
        if (sqliteHelp == null) {
            sqliteHelp = new SqliteHelp();
        }
        return sqliteHelp;
    }


    public async UniTask<int> InitSQLite() {
        if (!FileTool.ExistsFile("DataBase.db"))
        {
            LWDebug.Log("复制数据库");
            var request = UnityWebRequest.Get(Application.streamingAssetsPath + "/DataBase.db");
            await request.SendWebRequest();
            FileTool.WriteByteToFile("DataBase.db", request.downloadHandler.data, Application.persistentDataPath);
           
        }
        db = new DBAccess("URI=file:" + DBPath);
        return 1;
    }
    // Use this for initialization

    public void Close() {
        if (db != null) {
            db.CloseSqlConnection();
        }
    }

    public List<Rank> QueryRankPage(int size, int index)
    {
        string sqlstr = string.Format("select * from RankTable order by score Desc limit {0} offset {0}*{1}", size, index - 1);//size:每页显示条数，index页码
        SqliteDataReader sdr = db.ExecuteQuery(sqlstr);
        
        List<Rank> ranks = new List<Rank>();
        while (sdr.Read())
        {
            Rank rank = new Rank
            {
                userId = sdr.GetString(sdr.GetOrdinal("userId")),
                score = sdr.GetString(sdr.GetOrdinal("score")),
                scoreType = sdr.GetString(sdr.GetOrdinal("scoreType")),
                device = sdr.GetString(sdr.GetOrdinal("device")),
                insterDate = sdr.GetString(sdr.GetOrdinal("insterDate"))
            };
            ranks.Add(rank);
        }

        sdr.Close();
        return ranks;
    }

    public Rank SelectRankData(string userid) {
        string sqlstr = string.Format("SELECT rowid,*,(SELECT count(* ) FROM RankTable b WHERE a.score <= b.score) AS rankValue FROM RankTable AS a WHERE userId = '{0}' ORDER BY rankValue", userid);//size:每页显示条数，index页码
        SqliteDataReader sdr = db.ExecuteQuery(sqlstr);
        Rank rank = null;


        while (sdr.Read())
        {
            rank = new Rank
            {
                userId = sdr.GetString(sdr.GetOrdinal("userId")),
                score = sdr.GetString(sdr.GetOrdinal("score")),
                scoreType = sdr.GetString(sdr.GetOrdinal("scoreType")),
                device = sdr.GetString(sdr.GetOrdinal("device")),
                insterDate = sdr.GetString(sdr.GetOrdinal("insterDate")),
                rankValue = sdr.GetInt32(sdr.GetOrdinal("rankValue")).ToString()
            };
        }

        sdr.Close();

        return rank;
    }



    /// <summary>
    /// 新增系统数据 注册时记录
    /// </summary>
    public void InsertSystemInfoTable(string deviceUniqueIdentifier, string graphicsDeviceID, string macAddress, string registerDate, string useDate)
    {
        db.InsertInto("SystemInfoTable", new string[] { deviceUniqueIdentifier, graphicsDeviceID, macAddress, registerDate, useDate, "0" }, true);
    }
    public List<SystemInfoTable> QuerySystemInfoTable()
    {
        string sqlstr = string.Format("SELECT * FROM SystemInfoTable WHERE isSync='{0}'", "0");
        SqliteDataReader sdr = db.ExecuteQuery(sqlstr);

        List<SystemInfoTable> tables = new List<SystemInfoTable>();
        while (sdr.Read())
        {
            SystemInfoTable table = new SystemInfoTable
            {
                id = sdr.GetInt32(sdr.GetOrdinal("id")).ToString(),
                deviceUniqueIdentifier = sdr.GetString(sdr.GetOrdinal("deviceUniqueIdentifier")),
                graphicsDeviceID = sdr.GetString(sdr.GetOrdinal("graphicsDeviceID")),
                macAddress = sdr.GetString(sdr.GetOrdinal("macAddress")),
                registerDate = sdr.GetString(sdr.GetOrdinal("registerDate")),
                useDate = sdr.GetString(sdr.GetOrdinal("useDate")),
                isSync = sdr.GetInt32(sdr.GetOrdinal("isSync"))
            };
            tables.Add(table);
        }
        sdr.Close();
        return tables;
    }
    public void UpdateSystemInfoTable(string []idArray) {
        for (int i = 0; i < idArray.Length; i++)
        {
            db.UpdateInto("SystemInfoTable", new string[] { "isSync" }, new string[] { "1" }, "id", idArray[i]);
        }
    }
    /// <summary>
    /// 新增启动数据
    /// </summary>
    public void InsertStartupTable(string startupDate,string ipAddress)
    {
        db.InsertInto("StartupTable", new string[] { startupDate, ipAddress, "0" },true);
    }

    public List<StartupTable> QueryStartupTable()
    {
        string sqlstr = string.Format("SELECT * FROM StartupTable WHERE isSync='{0}'", "0");
        SqliteDataReader sdr = db.ExecuteQuery(sqlstr);

        List<StartupTable> tables = new List<StartupTable>();
        while (sdr.Read())
        {
            StartupTable table = new StartupTable
            {
                id = sdr.GetInt32(sdr.GetOrdinal("id")).ToString(),
                startupDate = sdr.GetString(sdr.GetOrdinal("startupDate")),
                ipAddress = sdr.GetString(sdr.GetOrdinal("ipAddress")),
                isSync = sdr.GetInt32(sdr.GetOrdinal("isSync"))
            };
            tables.Add(table);
        }
        sdr.Close();
        return tables;
    }
    public void UpdateStartupTable(string[] idArray)
    {
        for (int i = 0; i < idArray.Length; i++)
        {
            db.UpdateInto("StartupTable", new string[] { "isSync" }, new string[] { "1" }, "id", idArray[i]);
        }


    }
    /// <summary>
    /// 新增使用进入场景数据
    /// </summary>
    public void InsertPlayInfoTable(string sceneId, string sceneName, string enterDate, string quitDate, string playTime)
    {
        db.InsertInto("PlayInfoTable", new string[] {  sceneId, sceneName, enterDate, quitDate, playTime, "0" }, true);
    }
    public List<PlayInfoTable> QueryPlayInfoTable()
    {
        
        string sqlstr = string.Format("SELECT * FROM PlayInfoTable WHERE isSync='{0}'", "0");
        SqliteDataReader sdr = db.ExecuteQuery(sqlstr);

        List<PlayInfoTable> tables = new List<PlayInfoTable>();
        while (sdr.Read())
        {
            PlayInfoTable table = new PlayInfoTable
            {
                id = sdr.GetInt32(sdr.GetOrdinal("id")).ToString(),
                sceneId = sdr.GetString(sdr.GetOrdinal("sceneId")),
                sceneName = sdr.GetString(sdr.GetOrdinal("sceneName")),
                enterDate = sdr.GetString(sdr.GetOrdinal("enterDate")),
                quitDate = sdr.GetString(sdr.GetOrdinal("quitDate")),
                playTime = sdr.GetString(sdr.GetOrdinal("playTime")),
                isSync = sdr.GetInt32(sdr.GetOrdinal("isSync"))
            };
            tables.Add(table);
        }
        sdr.Close();
        return tables;
    }
    public void UpdatePlayInfoTable(string[] idArray)
    {
        for (int i = 0; i < idArray.Length; i++)
        {
            db.UpdateInto("PlayInfoTable", new string[] { "isSync" }, new string[] { "1" }, "id", idArray[i]);
        }

    }
}
public class Rank {
    public string userId;
    public string score;
    public string scoreType;
    public string rankValue;
    public string device;
    public string insterDate;
    public override string ToString()
    {
        return userId + " | " + score + " | " + scoreType + " | " + rankValue + " | " + device + " | " + insterDate ;
    }
}

public class PlayInfoTable {
    public string id;
    public string sceneId;
    public string sceneName;
    public string enterDate;
    public string quitDate;
    public string playTime;
    public int isSync;
    public override string ToString()
    {
        return id + " | " + sceneId + " | " + sceneName + " | " + enterDate + " | " + quitDate + " | " + playTime + " | " + isSync;
    }
}
public class SystemInfoTable
{
    public string id;
    public string deviceUniqueIdentifier;
    public string graphicsDeviceID;
    public string macAddress;
    public string registerDate;
    public string useDate;
    public int isSync;
    public override string ToString()
    {
        return id + " | " + deviceUniqueIdentifier + " | " + graphicsDeviceID + " | " + macAddress + " | " + registerDate + " | " + useDate + " | " + isSync;
    }
}

public class StartupTable
{
    public string id;
    public string startupDate;
    public string ipAddress;
    public int isSync;
    public override string ToString()
    {
        return id + " | " + startupDate + " | " + ipAddress  + " | " + isSync ;
    }
}