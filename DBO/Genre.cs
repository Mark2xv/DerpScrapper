using System;
using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    public class Genre : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;
        public new static string tableName
        {
            get { return "Genre"; }
        }

        public Genre(int rowId = -1)
            : base("Genre", rowId)
        {

        }

        override public void CreateColumns() 
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();
                _columns.Add("Name", System.Data.DbType.String);
            }
            this.columns = _columns;
        }

        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }

        public override bool hasDefaultValues()
        {
            return true;
        }

        public override string getDefaultValuesQuery()
        {
            return string.Format(@"/* Default values for Genre */
                INSERT INTO {0} (Name) VALUES ('Action'), ('Adventure'), ('Animation'), ('Anime'), ('Fantasy');", tableName);
        }

        public static Genre GetGenre(string name)
        {
            string query = "SELECT ROWID FROM Genre WHERE Name LIKE '" + name + "'";
            var comm = BaseDB.connection.CreateCommand();
            comm.CommandText = query;
            var row = comm.ExecuteScalar();
            if (row == null)
            {
                // Insert
                Genre g = new Genre();
                g["Name"] = name;
                g.Insert();
                return g;
            }
            else
            {
                Genre g = new Genre(Convert.ToInt32(row));
                return g;
            }
        }
        
    }
}
