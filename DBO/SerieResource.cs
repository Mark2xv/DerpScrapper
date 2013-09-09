using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    public class SerieResource : DBObject
    {
        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;

        public new static string tableName
        {
            get { return "SerieResource"; }
        }

        public SerieResource(int rowId = -1)
            : base("SerieResource", rowId)
        {
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();
                _columns.Add("SerieId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("ExternalSerieId", BaseDB.SQLiteDBType.Text);
                _columns.Add("ResourceSiteId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("ResourceSiteRating", DerpScrapper.BaseDB.SQLiteDBType.Double);
            }
            this.columns = _columns;
        }

        public int SerieId
        {
            get
            {
                if(ContainsKey("SerieId"))
                    return (int)this["SerieId"];
                return 0;
            }
            set
            {
                this["SerieId"] = value;
            }
        }

        public string ExternalSerieId
        {
            get
            {
                if (ContainsKey("ExternalSerieId"))
                    return (string) this["ExternalSerieId"];
                return "";
            }
            set
            {
                this["ExternalSerieId"] = value;
            }
        }

        public int ResourceSiteId
        {
            get
            {
                if (ContainsKey("ResourceSiteId"))
                    return (int)this["ResourceSiteId"];
                return 0;
            }
            set
            {
                this["ResourceSiteId"] = value;
            }
        }

        public double ResourceSiteRating
        {
            get
            {
                if (ContainsKey("ResourceSiteRating"))
                    return (double)this["ResourceSiteRating"];
                return 0;
            }
            set
            {
                this["ResourceSiteRating"] = value;
            }
        }
    }
}
