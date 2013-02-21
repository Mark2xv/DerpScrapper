using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DailyTvTorrentsAPI
{
    public class Episode
    {
        public string torrentFile720 { get; set; }
        public string torrentFile1080 { get; set; }
        public string torrentFileHD { get; set; }
        public string num { get; set; }
        public string title { get; set; }

        public int age { get; set; }

        public Episode() { }

        public Episode(JObject json)
        {
            torrentFile720 = (string)json["720"];
            torrentFile1080 = (string)json["1080"];
            torrentFileHD = (string)json["hd"];
            num = (string)json["num"];
            title = (string)json["title"];
            age = (int)json["age"];
        }
    }
}
