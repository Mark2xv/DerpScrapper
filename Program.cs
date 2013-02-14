﻿using System;
using System.Data.SQLite;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using DerpScrapper.DBO;
using DerpScrapper.DownloadSite_Scrapers;
using DerpScrapper.Scrapers;
using System.Threading;

namespace DerpScrapper
{
    class Program
    {
        public static bool ForceNewDB = false;
        public static string RootDirectory = @"C:\DerpScraper\";

        [STAThread()]
        static void Main(string[] args)
        {
            Program.Setup();
            Serie _serie = new Serie();

            Application.EnableVisualStyles();
            Application.Run(new Empty());
        }

        static void Setup()
        {
            if (!BaseDB.CreateDB(ForceNewDB))
            {
                Console.ReadKey();
                Environment.Exit(1);
            }

            if (!Directory.Exists(RootDirectory + "Cache"))
            {
                Directory.CreateDirectory(RootDirectory + "Cache");
            }

            if (!Directory.Exists(RootDirectory + "Cache\\Series"))
            {
                Directory.CreateDirectory(RootDirectory + "Cache\\Series");
            }
        }
    }
}