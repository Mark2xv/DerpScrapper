using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DerpScrapper
{
    partial class DBObject : Dictionary<string, object>
    {
        public Dictionary<string, System.Data.DbType> columns;
        public int rowId = -1;
        public string table;
        public static string tableName = "";

        public DBObject(string tableName, int rowId = -1) : base() 
        {
            this.table = tableName;
            this.CreateColumns();
            if (rowId != -1)
                this.Retrieve(rowId);
        }

        virtual public void CreateColumns() 
        {
            throw new Exception("extended DBObject requires CreateColumns to be used");
        }

        virtual public bool hasDefaultValues()
        {
            return false;
        }

        virtual public string getDefaultValuesQuery()
        {
            return "";
        }

        public new object this[string key] 
        {
            get
            {
                if (columns.ContainsKey(key))
                {
                    return (this.ContainsKey(key) ? base[key] : null);
                }
                throw new Exception("Invalid key on get call: " + key);
            }
            set
            {
                if (columns.ContainsKey(key))
                {
                    base[key] = value;
                    return;
                }
                throw new Exception("Invalid key on set call: " + key);
            }
        }

        public int Id
        {
            get
            {
                return this.rowId;
            }
        }

        public bool Exists;

        public int Insert()
        {
            if (this.rowId != -1)
                throw new Exception("What the fuck are you doin' son?");

            string columnsString = "";
            string valuesString = "";
            foreach (string key in this.Keys)
            {
                columnsString += key + ",";
                valuesString += "@" + key + ",";
            }
            columnsString = columnsString.TrimEnd(',');
            valuesString = valuesString.TrimEnd(',');
            var command = BaseDB.connection.CreateCommand();
            command.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT last_insert_rowid() as id ", this.table, columnsString, valuesString);
            
            // Any keys that have been set
            foreach (string key in this.Keys)
            {
                command.Parameters.AddWithValue("@" + key, this[key]).DbType = this.columns[key];
            }

            object retVal = command.ExecuteScalar();
            this.rowId = Convert.ToInt32(retVal);
            this.Exists = true;
            return this.rowId;
        }

        public void Retrieve(int rowId)
        {
            SQLiteCommand command = BaseDB.connection.CreateCommand();
            command.CommandText = string.Format("SELECT ROWID, * FROM {0} WHERE ROWID = {1}", this.table, rowId);
            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                this.Exists = true;
                while (reader.Read())
                {
                    this.rowId = reader.GetInt32(0);

                    foreach (string key in this.columns.Keys)
                    {
                        int colIdx = reader.GetOrdinal(key);
                        object colValue = reader.GetValue(colIdx);
                        if (!(colValue is System.DBNull))
                        {
                            System.Data.DbType type = this.columns[key];

                            switch (type)
                            {
                                case System.Data.DbType.String:
                                    this[key] = (string)colValue;
                                    break;
                                case System.Data.DbType.Int32:
                                    this[key] = Convert.ToInt32(colValue);
                                    break;
                                case System.Data.DbType.Double:
                                    this[key] = Convert.ToDouble(colValue);
                                    break;
                                default:

                                    break;
                            }


                        }
                    }

                    if (!reader.NextResult())
                        break;
                }
            }
            else
            {
                this.Exists = false;
            }
        }

        public bool Update() 
        {
            if (this.rowId == -1) 
                return false;

            string updateString = "";
            foreach (string key in this.Keys)
            {
                updateString += string.Format("{0} = @{0},", key);
            }
            updateString = updateString.TrimEnd(',');

            var command = BaseDB.connection.CreateCommand();
            command.CommandText = string.Format("UPDATE {0} SET {1} WHERE ROWID = {2}", table, updateString, this.rowId);

            // Any keys that have been set
            foreach (string key in this.Keys)
            {
                command.Parameters.AddWithValue("@" + key, this[key]).DbType = this.columns[key];
            }

            return command.ExecuteNonQuery() == 1;
        }

        public bool Delete()
        {
            if (this.rowId == -1)
                return false;

            var command = BaseDB.connection.CreateCommand();
            command.CommandText = string.Format("DELETE FROM {0} WHERE ROWID = {1}", this.table, this.rowId);

            int retVal = command.ExecuteNonQuery();
            if (retVal == 1)
            {
                this.Exists = false;
                return true;
            } 
            return false;
        }
    }
}
