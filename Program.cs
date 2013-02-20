using System;
using System.IO;
using System.Windows.Forms;
using DerpScrapper.DBO;

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

            Application.EnableVisualStyles();
<<<<<<< HEAD
            Application.Run(new Empty());
=======
            Application.Run(new UI.Main());
>>>>>>> Commit to get newest DB structure in dev branch
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
