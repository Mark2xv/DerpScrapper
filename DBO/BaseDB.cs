using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DerpScrapper
{
    public static class BaseDB
    {
        private static string DBPath;

        [ThreadStatic]
        private static SQLiteConnection _connection;

        public static SQLiteConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection("Data Source=" + DBPath + ";Version=3;");
                    _connection.Open();
                }
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public struct SQLiteDBType
        {
            public static SQLiteDBType Text
            {
                get 
                {
                    return new SQLiteDBType(DbType.String);
                }
            }

            public static SQLiteDBType Integer
            {
                get
                {
                    return new SQLiteDBType(DbType.Int64);
                }
            }

            public static SQLiteDBType Double
            {
                get
                {
                    return new SQLiteDBType(DbType.Double);
                }
            }

            public DbType type;
            public SQLiteDBType(DbType type)
            {
                switch (type)
                {
                    case DbType.UInt16:
                    case DbType.UInt32:
                    case DbType.UInt64:
                    case DbType.Byte:
                    case DbType.SByte:
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.Int64:
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                        this.type = DbType.Int64;
                        break;
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.String:
                    case DbType.StringFixedLength:
                        this.type = DbType.String;
                        break;
                    case DbType.Double:
                    case DbType.VarNumeric:
                    case DbType.Decimal:
                        this.type = DbType.Double;
                        break;
                    default:
                        throw new Exception("Unsupported datatype for SQLite");
                }
            }
        }

        public class TableDefinition
        {
            public string tableName;
            public List<ColumnDefinition> columns;

            public override string ToString()
            {
                return string.Format("{0}", tableName);
            }
        }

        public struct ColumnDefinition
        {
            public string columnName;
            public System.Data.DbType dbType;
            public string attributes;

            public override string ToString()
            {
                return string.Format("{0} ({1}", columnName, dbType.ToString()) + (attributes != "" ? ", " + attributes + ")" : ")");
            }
        }

        public static void Setup()
        {
            string tableQuery = "SELECT name,sql FROM sqlite_master WHERE type='table'";
            SQLiteCommand tableCm = Connection.CreateCommand();
            tableCm.CommandText = tableQuery;
            var reader = tableCm.ExecuteReader();
            List<TableDefinition> tables = new List<TableDefinition>();
            while (reader.HasRows && reader.Read())
            {
                string tableName = reader.GetString(0);
                if (tableName == "sqlite_sequence" || tableName == "sqlite_master")
                    continue;

                TableDefinition tableDef = new TableDefinition();
                tableDef.tableName = tableName;
                tableDef.columns = new List<ColumnDefinition>();


                string sql = reader.GetString(1);
                int idxOfOpen = sql.IndexOf('(') + 1;
                int idxOfClose = sql.LastIndexOf(')');

                string cols = sql.Substring(idxOfOpen, idxOfClose - idxOfOpen);

                var parts = cols.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    string colDefS = part.Trim();
                    var colDefParts = colDefS.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string colName = colDefParts[0];
                    string colType = colDefParts[1];
                    string colAttrs = "";
                    if (colDefParts.Length > 2)
                    {
                        for (int i = 2; i < colDefParts.Length; i++)
                        {
                            colAttrs += colDefParts[i] + " ";
                        }
                    }
                    colAttrs = colAttrs.Trim();
                    ColumnDefinition colDef = new ColumnDefinition();

                    colDef.columnName = colName;
                    colDef.attributes = colAttrs;

                    switch (colType)
                    {
                        case "INTEGER":
                        case "INT":
                            colDef.dbType = System.Data.DbType.Int64;
                            break;
                        case "TEXT":
                            colDef.dbType = System.Data.DbType.String;
                            break;
                        case "NUMERIC":
                            colDef.dbType = System.Data.DbType.Double;
                            break;
                    }

                    tableDef.columns.Add(colDef);
                }

                tables.Add(tableDef);
            }

            string createQuery = "";

            List<string> redoTables = new List<string>();

            foreach (var assembly in Assembly.GetExecutingAssembly().GetTypes().Where(t => String.Equals(t.Namespace, "DerpScrapper.DBO", StringComparison.Ordinal)))
            {
                if (assembly.BaseType.Name == "Enum")
                    continue;

                var c = assembly.GetConstructor(new Type[] { typeof(int) });
                DBObject dbObject = null;
                dbObject = (DBObject) c.Invoke(new object[] { -1 });
           
                bool contains = false;
                foreach (var table in tables)
                {
                    if (table.tableName == dbObject.table)
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                    string query = string.Format("\nCREATE TABLE {0} (Id INTEGER PRIMARY KEY AUTOINCREMENT ", dbObject.table);
                    foreach (var column in dbObject.columns)
                    {
                        string type = "";
                        switch (column.Value.type)
                        {
                            case System.Data.DbType.String:
                                type = "TEXT";
                                break;
                            case System.Data.DbType.Int64:
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
                else
                {
                    // find the table definition we're looking for
                    TableDefinition def = null;
                    foreach (var table in tables)
                    {
                        if (table.tableName == dbObject.table)
                        {
                            def = table;
                            break;
                        }
                    }

                    // Find which columns are 'different' or absent altogether (step 1: code defined)
                    foreach (var codeDefinedColumn in dbObject.columns)
                    {
                        bool foundColumn = false;
                        bool different = false;
                        // Find which columns are different in the DB defined table
                        foreach (var dbDefinedColumn in def.columns)
                        {
                            if (dbDefinedColumn.columnName == codeDefinedColumn.Key)
                            {
                                foundColumn = true;

                                if (dbDefinedColumn.dbType != codeDefinedColumn.Value.type)
                                {                                   
                                    // Column is different
                                    different = true;
                                }
                                break;
                            }
                        }

                        if (!different && !foundColumn)
                        {
                            // Create the column (that's easy.)
                            string type = "";
                            switch (codeDefinedColumn.Value.type)
                            {
                                case System.Data.DbType.String:
                                    type = "TEXT";
                                    break;
                                case System.Data.DbType.Int32:
                                case System.Data.DbType.Int64:
                                    type = "INT";
                                    break;
                                case System.Data.DbType.Double:
                                    type = "NUMERIC";
                                    break;
                            }
                            createQuery += string.Format("ALTER TABLE {0} ADD COLUMN {1} {2}", def.tableName, codeDefinedColumn.Key, type) + "\n";
                        }
                        else if (foundColumn && different)
                        {
                            // Shit.
                        }
                    }
                }
            }


            var initCommand = Connection.CreateCommand();
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
                DBPath = dbPath;

                var get = Connection;

                Setup();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
