using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DailyTvTorrentsAPI
{
    public class Torrent
    {
        public string name { get; set; }
        public string quality { get; set; }
        public string torrentFile { get; set; }

        public int age { get; set; }
        public int dataSize { get; set; }
        public int seeds { get; set; }
        public int leechers { get; set; }

        public Torrent() { }

        public Torrent(JObject json)
        {
            name = (string)json["name"];
            quality = (string)json["quality"];
            torrentFile = (string)json["link"];
            age = (int)json["age"];
            dataSize = (int)json["data_size"];
            seeds = (int)json["seeds"];
            leechers = (int)json["leechers"];
        }
    }
}
