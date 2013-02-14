﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DerpScrapper.DBO
{
    class SerieMetadata : DBObject
    {
        public static Dictionary<string, System.Data.DbType> _columns = null;

        public static string tableName = "SerieMetadata";


        public SerieMetadata(int rowId = -1)
            : base("SerieMetadata", rowId)
        {

        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, System.Data.DbType>();

                _columns.Add("SerieId", System.Data.DbType.Int32);
                _columns.Add("FirstAired", System.Data.DbType.Int32);
                _columns.Add("Airday", System.Data.DbType.String);
                _columns.Add("Runtime", System.Data.DbType.Int32);
                _columns.Add("Network", System.Data.DbType.String);

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

        public int FirstAired
        {
            get
            {
                return (int)this["FirstAired"];
            }
            set
            {
                this["FirstAired"] = value;
            }
        }

        public string Airday
        {
            get
            {
                return (string)this["Airday"];
            }
            set
            {
                this["Airday"] = value;
            }
        }

        public int Runtime
        {
            get
            {
                return (int)this["Runtime"];
            }
            set
            {
                this["Runtime"] = value;
            }
        }

        public string Network
        {
            get
            {
                return (string)this["Network"];
            }
            set
            {
                this["Network"] = value;
            }
        }
    }
}