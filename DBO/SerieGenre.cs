using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DerpScrapper.DBO
{
    class SerieGenre : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;

        public static string tableName = "SerieGenre";

        public SerieGenre(int rowId = -1)
            : base("SerieGenre", rowId)
        {
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();
                _columns.Add("SerieId", System.Data.DbType.Int32);
                _columns.Add("GenreId", System.Data.DbType.Int32);

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
