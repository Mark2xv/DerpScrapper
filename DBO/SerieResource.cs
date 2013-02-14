using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DerpScrapper.DBO
{
    class SerieResource : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;

        public static string tableName = "SerieResource";

        public SerieResource(int rowId = -1)
            : base("SerieResource", rowId)
        {
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();
                _columns.Add("SerieId", System.Data.DbType.Int32);
                _columns.Add("ResourceSiteId", System.Data.DbType.Int32);
                _columns.Add("ResourceSiteRating", System.Data.DbType.Double);
                _columns.Add("ResourceSiteUrl", System.Data.DbType.String);
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
