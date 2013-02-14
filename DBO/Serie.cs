using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DerpScrapper.DBO
{
    class Serie : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;

        public static string tableName = "Serie";

        public Serie(int rowId = -1)
            : base("Serie", rowId)
        {
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();
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
    }
}
