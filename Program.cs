using System;
using System.IO;
using System.Windows.Forms;

namespace DerpScrapper
{
    class Program
    {
        public static bool ForceNewDB = true;
        public static string RootDirectory = @"C:\DerpScraper\";

        [STAThread()]
        static void Main(string[] args)
        {
            Program.Setup();

            Application.EnableVisualStyles();
            Application.Run(new MainWindow());
        }

        static void Setup()
        {
            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);
            }

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
