using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    class ResourceSite : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;

        public new static string tableName
        {
            get { return "ResourceSite"; }
        }

        public ResourceSite(int rowId = -1)
            : base("ResourceSite", rowId)
        {
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();
                _columns.Add("Url", System.Data.DbType.String);
                _columns.Add("Priority", System.Data.DbType.Int32);
            }
            this.columns = _columns;
        }

        public override bool hasDefaultValues()
        {
            return true;
        }

        public override string getDefaultValuesQuery()
        {
            return 
@"/*default values for ResourceSite*/
INSERT INTO ResourceSite (Url, Priority) VALUES ('http://thetvdb.com', 1);";
        }

        public string Url
        {
            get
            {
                return (string)this["Url"];
            }
            set
            {
                this["Url"] = value;
            }
        }

        public int Priority
        {
            get
            {
                return (int)this["Priority"];
            }
            set
            {
                this["Priority"] = value;
            }
        }
    }
}
