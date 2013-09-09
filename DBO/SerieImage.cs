using System.Collections.Generic;
using System;

namespace DerpScrapper.DBO
{
    public class SerieImage : DBObject
    {
        public enum SerieImageType
        {
            Banner = 0,
            Poster = 1,
            FanArt = 2
        }

        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;

        public new static string tableName
        {
            get { return "SerieImage"; }
        }

        public SerieImage(int rowId = -1)
            : base("SerieImage", rowId)
        {
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

        public SerieImageType ImageType
        {
            get
            {
                return (SerieImageType) this["ImageType"];
            }
            set
            {
                this["ImageType"] = (int)value;
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

        public string Url
        {
            get
            {
                if (this.ContainsKey("LocalCachePath"))
                {
                    return (string) this["LocalCachePath"];
                }
                else if (this.ContainsKey("RemoteURL"))
                {
                    return (string) this["RemoteURL"];
                }
                else
                {
                    return "";
                }
            }
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();
                _columns.Add("SerieId", DerpScrapper.BaseDB.SQLiteDBType.Integer);
                _columns.Add("ImageType", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("RemoteURL", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("LocalCachePath", DerpScrapper.BaseDB.SQLiteDBType.Text);
            }
            this.columns = _columns;
        }
    }
}
