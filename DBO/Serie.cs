using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    public class Serie : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;

        public new static string tableName
        {
            get { return "Serie"; }
        }

        public Serie(int rowId = -1)
            : base("Serie", rowId)
        {
        }

        public static Serie GetByName(string name)
        {
            var com = BaseDB.connection.CreateCommand("SELECT ROWID FROM Serie WHERE Name LIKE '" + name + "' COLLATE NOCASE");
            var reader = com.ExecuteReader();
            if (reader.HasRows && reader.Read())
            {
                return new Serie(reader.GetInt32(0));
            }
            return null;
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();
                _columns.Add("LibraryId", System.Data.DbType.Int32);
                _columns.Add("Name", System.Data.DbType.String);
                _columns.Add("Plot", System.Data.DbType.String);
                _columns.Add("Ongoing", System.Data.DbType.Int32);
                _columns.Add("Language", System.Data.DbType.String);
                _columns.Add("FolderPath", System.Data.DbType.String);
                _columns.Add("FolderIsNetworkMount", System.Data.DbType.Int32);
                _columns.Add("UpdatePlanned", System.Data.DbType.Int32);
            }


            this.columns = _columns;
        }

        public List<Episode> GetEpisodes()
        {
            List<Episode> eps = new List<Episode>();

            var com = BaseDB.connection.CreateCommand("SELECT ROWID FROM Episode WHERE SerieId = " + this.Id.ToString());
            var reader = com.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                eps.Add(new Episode(reader.GetInt32(0)));
            }

            return eps;
        }

        public List<SerieGenre> GetGenres()
        {
            List<SerieGenre> eps = new List<SerieGenre>();

            var com = BaseDB.connection.CreateCommand("SELECT ROWID FROM SerieGenre WHERE SerieId = " + this.Id.ToString());
            var reader = com.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                eps.Add(new SerieGenre(reader.GetInt32(0)));
            }

            return eps;
        }

        public SerieMetadata GetMetadata()
        {
            SerieMetadata meta = null;

            var com = BaseDB.connection.CreateCommand("SELECT ROWID FROM SerieMetadata WHERE SerieId = " + this.Id.ToString());
            var reader = com.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                meta = new SerieMetadata(reader.GetInt32(0));
                break;
            }

            return meta;
        }

        public SerieResource GetResource()
        {
            SerieResource res = null;

            var com = BaseDB.connection.CreateCommand("SELECT ROWID FROM SerieResource WHERE SerieId = " + this.Id.ToString());
            var reader = com.ExecuteReader();
            while (reader.HasRows && reader.Read())
            {
                res = new SerieResource(reader.GetInt32(0));
                break;
            }

            return res;
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

        public string Plot
        {
            get
            {
                return (string)this["Plot"];
            }
            set
            {
                this["Plot"] = value;
            }
        }

        public bool Ongoing
        {
            get
            {
                return (bool)this["Ongoing"];
            }
            set
            {
                this["Ongoing"] = value;
            }
        }

        public string Language
        {
            get
            {
                return (string)this["Language"];
            }
            set
            {
                this["Language"] = value;
            }
        }

        public string FolderPath
        {
            get
            {
                return (string)this["FolderPath"];
            }
            set
            {
                this["FolderPath"] = value;
            }
        }

        public bool FolderIsNetworkMount
        {
            get
            {
                return (bool)this["FolderIsNetworkMount"];
            }
            set
            {
                this["FolderIsNetworkMount"] = value;
            }
        }

        public int UpdatePlanned
        {
            get
            {
                return (int)this["UpdatePlanned"];
            }
            set
            {
                this["UpdatePlanned"] = value;
            }
        }

        public int LibraryId
        {
            get
            {
                return (int)this["LibraryId"];
            }
            set
            {
                this["LibraryId"] = value;
            }
        }
    }
}
