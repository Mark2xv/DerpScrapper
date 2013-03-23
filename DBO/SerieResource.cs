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
                _columns.Add("ResourceSiteId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("ResourceSiteRating", DerpScrapper.BaseDB.SQLiteDBType.Double);
                _columns.Add("ResourceSiteUrl", DerpScrapper.BaseDB.SQLiteDBType.Text);
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

        public int ResourceSiteId
        {
            get
            {
                return (int)this["ResourceSiteId"];
            }
            set
            {
                this["ResourceSiteId"] = value;
            }
        }

        public string ResourceSiteUrl
        {
            get
            {
                return (string)this["ResourceSiteUrl"];
            }
            set
            {
                this["ResourceSiteUrl"] = value;
            }
        }

        public double ResourceSiteRating
        {
            get
            {
                return (double)this["ResourceSiteRating"];
            }
            set
            {
                this["ResourceSiteRating"] = value;
            }
        }
    }
}
