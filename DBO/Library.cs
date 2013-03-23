﻿using System.Collections.Generic;
using System;

namespace DerpScrapper.DBO
{
    public class Library : DBObject
    {
        public static Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType> _columns = null;

        public new static string tableName
        {
            get { return "Library"; }
        }

        public Library(int rowId = -1)
            : base("Library", rowId)
        {
        }

        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }

        public string PrimaryLanguage
        {
            get
            {
                return (string)this["PrimaryLanguage"];
            }
            set
            {
                this["PrimaryLanguage"] = value;
            }
        }

        public string SecondaryLanguage
        {
            get
            {
                return (string)this["SecondaryLanguage"];
            }
            set
            {
                this["SecondaryLanguage"] = value;
            }
        }

        public override void CreateColumns()
        {
            if (_columns == null)
            {
                _columns = new Dictionary<string, DerpScrapper.BaseDB.SQLiteDBType>();
                _columns.Add("Name", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("PrimaryLanguage", DerpScrapper.BaseDB.SQLiteDBType.Text);
                _columns.Add("SecondaryLanguage", DerpScrapper.BaseDB.SQLiteDBType.Text);
            }
            this.columns = _columns;
        }

        static public List<Library> GetAll()
        {
            List<Library> list = new List<Library>();

            var command = BaseDB.connection.CreateCommand();
            command.CommandText = string.Format("SELECT ROWID FROM {0}", @"Library");
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Library lib = new Library(reader.GetInt32(0));
                    list.Add(lib);
                }
            }

            return list;
        }

        public bool LibraryNameExists()
        {
            if (this["Name"].ToString() == string.Empty)
                return false;

            var command = BaseDB.connection.CreateCommand();
            command.CommandText = string.Format("SELECT {1} FROM {0} WHERE {1} = {2}", "Library", "Name", "@Name");
            command.Parameters.AddWithValue("@Name", this["Name"]).DbType = this.columns["Name"].type;

            var reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public List<Serie> GetSeries()
        {
            List<Serie> list = new List<Serie>();
            if (this.rowId == -1)
                return list;

            var command = BaseDB.connection.CreateCommand();
            command.CommandText = string.Format("SELECT ROWID FROM {0} WHERE ", @"Serie");
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Serie serie = new Serie(reader.GetInt32(0));
                    list.Add(serie);
                }
            }

            return list;
        }
    }
}
