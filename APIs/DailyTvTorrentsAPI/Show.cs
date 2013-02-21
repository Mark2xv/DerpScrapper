using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DailyTvTorrentsAPI
{
    public class Show
    {
        public string name { get; set; }
        public string prettyName { get; set; }
        public string genre { get; set; }
        public string link { get; set; }

        public Episode latestEpisode { get; set; }

        public Show() { }

        public Show(JObject json)
        {
            name = (string)json["name"];
            prettyName = (string)json["pretty_name"];
            genre = (string)json["genre"];
            link = (string)json["link"];

            latestEpisode = new Episode((JObject)json["latest_episode"]);
        }
    }
}
