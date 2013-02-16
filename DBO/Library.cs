using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    class Library : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;

        public new static string tableName
        {
            get { return "Library"; }
        }

        public Library(int rowId = -1)
            : base("Library", rowId)
        {
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();
                _columns.Add("Name", System.Data.DbType.String);
                _columns.Add("PrimaryLanguage", System.Data.DbType.String);
                _columns.Add("SecondaryLanguage", System.Data.DbType.String);
            }
            this.columns = _columns;
        }
    }
}
