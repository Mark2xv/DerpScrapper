using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    public class SerieMetadata : DBObject
    {
        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;

        public new static string tableName
        {
            get { return "SerieMetadata"; }
        }


        public SerieMetadata(int rowId = -1)
            : base("SerieMetadata", rowId)
        {

        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();

                _columns.Add("SerieId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("FirstAired", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("Airday", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("Runtime", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("Network", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("NameAlternatives", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("NameNonAlternatives", DerpScrapper.BaseDB.SQLiteDBType.Text);

            }
            this.columns = _columns;
        }

        public int SerieId
        {
            get
            {
                return (int)this["SerieId"];
            }
            set
            {
                this["SerieId"] = value;
            }
        }

        public int FirstAired
        {
            get
            {
                return (int)this["FirstAired"];
            }
            set
            {
                this["FirstAired"] = value;
            }
        }

        public string Airday
        {
            get
            {
                return (string)this["Airday"];
            }
            set
            {
                this["Airday"] = value;
            }
        }

        public int Runtime
        {
            get
            {
                return (int)this["Runtime"];
            }
            set
            {
                this["Runtime"] = value;
            }
        }

        public string Network
        {
            get
            {
                return (string)this["Network"];
            }
            set
            {
                this["Network"] = value;
            }
        }

        public string NameAlternatives
        {
            get
            {
                return (string)this["NameAlternatives"];
            }
            set
            {
                this["NameAlternatives"] = value;
            }
        }

        public string NameNonAlternatives
        {
            get
            {
                return (string)this["NameNonAlternatives"];
            }
            set
            {
                this["NameNonAlternatives"] = value;
            }
        }
    }
}
