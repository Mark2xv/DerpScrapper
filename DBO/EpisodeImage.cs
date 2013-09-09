using System.Collections.Generic;
using System;

namespace DerpScrapper.DBO
{
    public class EpisodeImage : DBObject
    {
        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;

        public new static string tableName
        {
            get { return "EpisodeImage"; }
        }

        public EpisodeImage(int rowId = -1)
            : base("EpisodeImage", rowId)
        {
        }

        public int EpisodeId
        {
            get
            {
                return (int) this["EpisodeId"];
            }
            set
            {
                this["EpisodeId"] = value;
            }
        }

        public string RemoteURL
        {
            get
            {
                return (string) this["RemoteURL"];
            }
            set
            {
                this["RemoteURL"] = value;
            }
        }

        public string LocalCachePath
        {
            get
            {
                return (string) this["LocalCachePath"];
            }
            set
            {
                this["LocalCachePath"] = value;
            }
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();
                _columns.Add("EpisodeId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("RemoteURL", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("LocalCachePath", DerpScrapper.BaseDB.SQLiteDBType.Text);
            }
            this.columns = _columns;
        }
    }
}
