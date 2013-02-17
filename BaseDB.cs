using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DerpScrapper
{
    class BaseDB
    {
        [ThreadStatic]
        public static SQLiteConnection connection;

        public static void Setup()
        {
            string tableQuery = "SELECT name FROM sqlite_master WHERE type='table'";
            SQLiteCommand tableCm = connection.CreateCommand();
            tableCm.CommandText = tableQuery;
            var reader = tableCm.ExecuteReader();
            List<string> tables = new List<string>();
            while (reader.HasRows && reader.Read())
            {
                tables.Add(reader.GetString(0));
            }

            string createQuery = "";

            foreach (var assembly in Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "DerpScrapper.DBO", StringComparison.Ordinal)))
            {
                var c = assembly.GetConstructor(new Type[] { typeof(int) });
                var dbObject = (DBObject)c.Invoke(new object[] { -1 });

                if (!tables.Contains(dbObject.table))
                {
                    string query = string.Format("\nCREATE TABLE {0} (Id INTEGER PRIMARY KEY AUTOINCREMENT ", dbObject.table);
                    foreach (var column in dbObject.columns)
                    {
                        string type = "";
                        switch (column.Value)
                        {
                            case System.Data.DbType.String:
                                type = "TEXT";
                                break;
                            case System.Data.DbType.Int64:
                            case System.Data.DbType.Int32:
                                type = "INT";
                                break;
                            case System.Data.DbType.Double:
                                type = "NUMERIC";
                                break;
                            default:
                                throw new Exception(string.Format("Unknown column datatype in definition of table {0}, column {1}", dbObject.table, column.Key));
                        }
                        query += string.Format(", {0} {1} ", column.Key, type);
                    }
                    query += ");";

                    if (dbObject.hasDefaultValues())
                    {
                        query += "\n" + dbObject.getDefaultValuesQuery();
                    }

                    createQuery += query;
                }
            }


            var initCommand = connection.CreateCommand();
            initCommand.CommandText = createQuery;
            initCommand.CommandType = System.Data.CommandType.Text;
            initCommand.ExecuteNonQuery();
        }

        public static bool CreateDB(bool force = false)
        {
            try
            {
                string dbPath = Program.RootDirectory + "db.sqlite";
                if (force || !File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                }
                connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3;");
                connection.Open();
                Setup();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
