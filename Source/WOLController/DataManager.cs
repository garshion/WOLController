using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace Bass.Util.WOL
{
    public class DataManager
    {
        private const string DATA_FILE_NAME = "SaveData.db";
        private const string DATASOURCE_STRING = "Data Source=SaveData.db;";


        public List<WOLData> LoadDatas()
        {
            _CheckAndMakeFile();

            List<WOLData> retList = new List<WOLData>();

            using (SQLiteConnection con = new SQLiteConnection(DATASOURCE_STRING))
            {
                con.Open();
                using (SQLiteCommand stmt = con.CreateCommand())
                {
                    stmt.CommandText = "SELECT MachineName, MacAddress FROM MachineList; ";
                    stmt.CommandType = System.Data.CommandType.Text;

                    var rs = stmt.ExecuteReader();
                    while (rs.Read())
                    {
                        WOLData addData = new WOLData();
                        addData.MachineName = rs.GetString(0);
                        addData.MacAddress = rs.GetString(1);
                        retList.Add(addData);
                    }
                }
            }

            return retList;
        }

        public bool SaveDatas(List<WOLData> datas)
        {
            if (null == datas)
                return false;

            _CheckAndMakeFile();

            using (SQLiteConnection con = new SQLiteConnection(DATASOURCE_STRING))
            {
                con.Open();

                foreach (WOLData data in datas)
                {
                    bool bNeedInsert = false;
                    using (SQLiteCommand stmt = con.CreateCommand())
                    {
                        stmt.CommandText = $"UPDATE MachineList SET MacAddress = '{data.MacAddress}' WHERE MachineName = '{data.MachineName}'; ";
                        stmt.CommandType = CommandType.Text;

                        bNeedInsert = 0 == stmt.ExecuteNonQuery();
                    }

                    if (bNeedInsert)
                    {
                        using (SQLiteCommand stmt = con.CreateCommand())
                        {
                            stmt.CommandText = $"INSERT INTO MachineList VALUES('{data.MachineName}', '{data.MacAddress}'); ";
                            stmt.CommandType = CommandType.Text;
                            stmt.ExecuteNonQuery();
                        }
                    }

                }

            }

            return true;
        }

        public bool InsertData(WOLData data)
        {
            if (null == data)
                return false;

            using (SQLiteConnection con = new SQLiteConnection(DATASOURCE_STRING))
            {
                con.Open();
                using (SQLiteCommand stmt = con.CreateCommand())
                {
                    stmt.CommandText = $"INSERT INTO MachineList VALUES('{data.MachineName}', '{data.MacAddress}'); ";
                    stmt.CommandType = CommandType.Text;
                    stmt.ExecuteNonQuery();
                }
            }

            return true;
        }

        public bool UpdateData(WOLData data)
        {
            if (null == data)
                return false;

            using (SQLiteConnection con = new SQLiteConnection(DATASOURCE_STRING))
            {
                con.Open();
                using (SQLiteCommand stmt = con.CreateCommand())
                {
                    stmt.CommandText = $"UPDATE MachineList SET MacAddress = '{data.MacAddress}' WHERE MachineName = '{data.MachineName}'; ";
                    stmt.CommandType = CommandType.Text;
                    stmt.ExecuteNonQuery();
                }
            }

            return true;
        }

        public bool DeleteData(WOLData data)
        {
            if (null == data)
                return false;

            using (SQLiteConnection con = new SQLiteConnection(DATASOURCE_STRING))
            {
                con.Open();
                using (SQLiteCommand stmt = con.CreateCommand())
                {
                    stmt.CommandText = $"DELETE FROM MachineList WHERE MachineName = '{data.MachineName}'; ";
                    stmt.CommandType = CommandType.Text;
                    stmt.ExecuteNonQuery();
                }
            }

            return true;
        }




        private void _CheckAndMakeFile()
        {
            if (!File.Exists(DATA_FILE_NAME))
            {
                SQLiteConnection.CreateFile(DATA_FILE_NAME);

                using (SQLiteConnection con = new SQLiteConnection(DATASOURCE_STRING))
                {
                    con.Open();
                    using (SQLiteCommand stmt = con.CreateCommand())
                    {
                        stmt.CommandText = _GetCreateTableString();
                        stmt.CommandType = System.Data.CommandType.Text;
                        stmt.ExecuteNonQuery();
                    }
                }
            }
        }

        private string _GetCreateTableString() => "CREATE TABLE IF NOT EXISTS MachineList (MachineName TEXT PRIMARY KEY, MacAddress TEXT NOT NULL) WITHOUT ROWID;";




    }
}
