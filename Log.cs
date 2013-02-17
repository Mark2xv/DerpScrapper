using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DerpScrapper
{
    static class Log
    {
        static string path = "";

        public static void WriteLine(string contents, params object[] args)
        {
            if(path == "") {
                path = Program.RootDirectory + "log.txt";
            }

            if (!File.Exists(path))
                File.Create(path);

            File.AppendAllText(path, string.Format(contents, args));
            Console.WriteLine(contents, args);
        }
    }
}
