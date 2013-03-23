using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    public class SerieGenre : DBObject
    {
        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;

        public new static string tableName
        {
            get { return "SerieMetadata"; }
        }

        public SerieGenre(int rowId = -1)
            : base("SerieGenre", rowId)
        {
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();
                _columns.Add("SerieId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("GenreId", DerpScrapper.BaseDB.SQLiteDBType.Integer);

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

        public int GenreId
        {
            get
            {
                return (int)this["GenreId"];
            }
            set
            {
                this["GenreId"] = value;
            }
        }
    }
}
