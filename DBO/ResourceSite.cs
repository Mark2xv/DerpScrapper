using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    public class ResourceSite : DBObject
    {
        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;

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
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();
                _columns.Add("Url", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("Priority", DerpScrapper.BaseDB.SQLiteDBType.Integer);
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
