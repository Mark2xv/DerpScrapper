using System.Collections.Generic;

namespace DerpScrapper.DBO
{
    public class Episode : DBObject
    {
        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;
        public new static string tableName
        {
            get { return "Episode"; }
        }

        public Episode(int rowId = -1)
            : base(Episode.tableName, rowId)
        {
    
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();
                _columns.Add("SerieId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("FileName", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("Downloading", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("AbsoluteEpisodeNumber", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("SeasonNumber", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("EpisodeNumber", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("EpisodeName", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("Special", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("Movie", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("AirDate", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("Synopsis", DerpScrapper.BaseDB.SQLiteDBType.Text);
            }
            this.columns = _columns;
        }

        public int SerieId
        {
            get
            {
                return (int) this["SerieId"];
            }
            set
            {
                this["SerieId"] = value;
            }
        }

        public string EpisodeName
        {
            get
            {
                return (string)this["EpisodeName"];
            }
            set
            {
                this["EpisodeName"] = value;
            }
        }

        public int SeasonNumber
        {
            get
            {
                return (int)this["SeasonNumber"];
            }
            set
            {
                this["SeasonNumber"] = value;
            }
        }

        public int EpisodeNumber
        {
            get
            {
                return (int)this["EpisodeNumber"];
            }
            set
            {
                this["EpisodeNumber"] = value;
            }
        }

        public int AbsoluteEpisodeNumber
        {
            get
            {
                return (int)this["AbsoluteEpisodeNumber"];
            }
            set
            {
                this["AbsoluteEpisodeNumber"] = value;
            }
        }

        public int AirDate
        {
            get
            {
                return (int)this["AirDate"];
            }
            set
            {
                this["AirDate"] = value;
            }
        }

        public bool isSpecial
        {
            get
            {
                return (bool)this["Special"];
            }
            set
            {
                this["Special"] = value;
            }
        }

        public bool isMovie
        {
            get
            {
                return (bool)this["Movie"];
            }
            set
            {
                this["Movie"] = value;
            }
        }

        public string Synopsis
        {
            get
            {
                return (string)this["Synopsis"];
            }
            set
            {
                this["Synopsis"] = value;
            }
        }

        public bool hasImage;
        public int SeasonId;
        public int EpisodeId;
    }
}
