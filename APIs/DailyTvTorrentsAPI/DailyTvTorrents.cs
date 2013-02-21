using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DailyTvTorrentsAPI
{
    public class DailyTvTorrents
    {
        private string apiEndpoint = "http://api.dailytvtorrents.org/1.0/";
        public bool success = false;

        public string Fetch(string apiMethod, Dictionary<string, string> parameters)
        {
            success = false;
            string parametersForUrl = GetParametersString(parameters);
            string urlToFetch = Uri.EscapeUriString(apiEndpoint + apiMethod + "?" + parametersForUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToFetch);
            request.Method = "GET";

            try
            {
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    response.Close();
                    return "";
                }
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                success = true;
                return responseFromServer;
            }
            catch (WebException)
            {
                return "";
            }
        }

        private string GetParametersString(Dictionary<string, string> parameters)
        {
            string paramsString = "";

            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                paramsString += "&" + parameter.Key + "=" + parameter.Value;
            }

            return paramsString;
        }

        public string MiscGetCleanFileName(string fileName)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("filename", fileName);
            string fn = Fetch("misc.getCleanFileName", parameters);
            if (success) return fn;
            else return "";
        }

        public string ShowsGetTextInfo(string showNames, int? maxAgeHours = null,
            string colors = "no", string links = "no")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_names", showNames);
            if (maxAgeHours != null)
                parameters.Add("max_age_hours", maxAgeHours.Value.ToString());
            parameters.Add("colors", colors);
            parameters.Add("links", links);
            string info = Fetch("shows.getTextInfo", parameters);
            if (success) return info;
            else return "";
        }

        public Episode EpisodeGetLatest(string showName)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_name", showName);
            string jsonText = Fetch("episode.getLatest", parameters);
            if (success)
            {
                JObject json = JObject.Parse(jsonText);
                return new Episode(json);
            }
            else
            {
                return new Episode();
            }
        }

        public class GetEpisodeResult 
        {
            public List<Episode> Episodes;
            public int TotalEpisodes;
            public int Pages;
            public int CurrentPage;
        }

        public GetEpisodeResult ShowGetEpisodes(string showName, int page = 0)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_name", showName);
            parameters.Add("page", page.ToString());
            string jsonText = Fetch("show.getEpisodes", parameters);

            GetEpisodeResult result = new GetEpisodeResult();
            result.Episodes = new List<Episode>();

            if (success)
            {
                JObject json = JObject.Parse(jsonText);
                result.TotalEpisodes = json["total_episodes"].Value<int>();
                result.Pages = json["total_pages"].Value<int>();
                result.CurrentPage = json["current_page"].Value<int>(); ;

                var eps =
                  from p in json["episodes"].Children()
                  select p;

                foreach (JObject item in eps)
                {
                    result.Episodes.Add(new Episode(item));
                }
            }
            else
            {
                return null;
            }

            return result;
        }

        public Show ShowGetInfo(string showName)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_name", showName);
            string jsonText = Fetch("show.getInfo", parameters);

            if (success)
            {
                JObject json = JObject.Parse(jsonText);
                return new Show(json);
            }
            else
            {
                return new Show();
            }
        }

        public List<Show> ShowsGetNew(string strict = "no", int? maxAgeHours = null,
            int maxItems = 6, string sort = "age")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("strict", strict);
            if (maxAgeHours != null)
                parameters.Add("max_age_hours", maxAgeHours.Value.ToString());
            parameters.Add("max_items", maxItems.ToString());
            parameters.Add("sort", sort);
            string jsonText = Fetch("shows.getNew", parameters);
            if (success) return ParseShows(jsonText);
            else return new List<Show>();
        }

        public List<Show> ShowsGetPopular(int maxItems = 6)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("max_items", maxItems.ToString());
            string jsonText = Fetch("shows.getPopular", parameters);
            if (success) return ParseShows(jsonText);
            else return new List<Show>();
        }

        public List<Show> ShowsGetReturning(int maxItems = 6)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("max_items", maxItems.ToString());
            string jsonText = Fetch("shows.getReturning", parameters);
            if (success) return ParseShows(jsonText);
            else return new List<Show>();
        }

        public List<Show> ShowsSearch(string query, int page = 0)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("query", query);
            parameters.Add("page", page.ToString());
            string jsonText = Fetch("shows.search", parameters);
            if (success) return ParseShows(jsonText, true, "shows");
            else return null;
        }

        public Torrent TorrentGetInfo(string showName, string episodeNum,
            string quality = "hd", string fallback = "no")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_name", showName);
            parameters.Add("episode_num", episodeNum);
            parameters.Add("quality", quality);
            parameters.Add("fallback", fallback);
            string jsonText = Fetch("torrent.getInfo", parameters);

            if (success)
            {
                JObject json = JObject.Parse(jsonText);
                return new Torrent(json);
            }
            else
            {
                return new Torrent();
            }
        }

        public Torrent TorrentGetLatest(string showName, string quality = "hd",
            string fallback = "no")
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_name", showName);
            parameters.Add("quality", quality);
            parameters.Add("fallback", fallback);
            string jsonText = Fetch("torrent.getLatest", parameters);

            if (success)
            {
                JObject json = JObject.Parse(jsonText);
                return new Torrent(json);
            }
            else
            {
                return new Torrent();
            }
        }

        protected List<Show> ParseShows(string jsonText, bool inElement = false, string element = "")
        {
            List<Show> shows = new List<Show>();

            if (inElement)
            {
                JObject json = JObject.Parse(jsonText);

                var eps =
                  from p in json[element].Children()
                  select p;

                foreach (JObject item in eps)
                    shows.Add(new Show(item));
            }
            else
            {
                JArray json = JArray.Parse(jsonText);

                foreach (JObject item in json)
                    shows.Add(new Show(item));
            }

            return shows;
        }

        protected List<Torrent> ParseTorrents(string jsonText)
        {
            JArray json = JArray.Parse(jsonText);
            List<Torrent> torrents = new List<Torrent>();

            foreach (JObject item in json)
                torrents.Add(new Torrent(item));

            return torrents;
        }

        public List<Torrent> TorrentGetInfos(string showName, string episodeNum)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_name", showName);
            parameters.Add("episode_num", episodeNum);
            string jsonText = Fetch("torrent.getInfos", parameters);
            if (success) return ParseTorrents(jsonText);
            else return new List<Torrent>();
        }

        public List<Torrent> TorrentGetInfosAll(string showName, string episodeNum)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("show_name", showName);
            parameters.Add("episode_num", episodeNum);
            string jsonText = Fetch("torrent.getInfosAll", parameters);
            if (success) return ParseTorrents(jsonText);
            else return new List<Torrent>();
        }
    }
}
