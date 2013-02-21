using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DerpScrapper.Scraper;
using System.Text.RegularExpressions;
using System.Xml;

namespace DerpScrapper.DownloadSite_Scrapers
{
    class DailyTVTorrents : IDownloadSite
    {
        DailyTvTorrentsAPI.DailyTvTorrents api = new DailyTvTorrentsAPI.DailyTvTorrents();
        private static Regex onlyAlphaNumeric = new Regex(@"[^\w\s]", RegexOptions.IgnoreCase);

        public List<PossibleDownloadHit> GetDownloads_Rss(SerieInfo forSerie, string tvTorrentsName)
        {
            string rssUrl = "http://www.dailytvtorrents.org/rss/show/" + tvTorrentsName + "?single=yes&norar=yes&prefer=720&wait=6";
            XmlDocument rss = new XmlDocument();
            try
            {
                rss.LoadXml(ScraperUtility.GetContentOfUrl(rssUrl));
            }
            catch(System.Net.WebException e)
            {
                if (!e.Message.Contains("404"))
                {
                    Console.WriteLine(e);
                }
                else
                {
                    Console.WriteLine("DailyTVTorrents does not know the serie \"" + tvTorrentsName + "\"");
                }
                return null;
            }

            foreach (XmlNode node in rss.GetElementsByTagName("item"))
            {
            }

            return null;
        }

        public List<PossibleDownloadHit> GetDownloads_Api(SerieInfo forSerie, string tvTorrentsName)
        {
            List<PossibleDownloadHit> totalHits = new List<PossibleDownloadHit>();

            bool hasMoreApiCredits = true;
            int page = 0;
            while (hasMoreApiCredits)
            {
                var res = api.ShowGetEpisodes(tvTorrentsName, page);
                if (res == null)
                {
                    hasMoreApiCredits = false;
                    break;
                }

                foreach (var hit in res.Episodes)
                {
                    string url = "";
                    if (hit.torrentFile1080 != null && hit.torrentFile1080 != "")
                        url = hit.torrentFile1080;
                    else if (hit.torrentFile720 != null && hit.torrentFile720 != "")
                        url = hit.torrentFile720;
                    else
                        url = hit.torrentFileHD;

                    var parts = hit.num.Split(new char[] { 'S', 'E' }, StringSplitOptions.RemoveEmptyEntries);
                    int season = int.Parse(parts[0]);
                    int ep = int.Parse(parts[1]);

                    SeasonEpisode sEp = new SeasonEpisode(season, ep);
                    totalHits.Add(new PossibleDownloadHit()
                    {
                        name = forSerie.serie.Name + " " + hit.num,
                        url = url,
                        episodes = new List<SeasonEpisode>(new[] { sEp })
                    });
                }

                if (res.CurrentPage == res.Pages)
                    break;
                else
                    page++;
            }
            return totalHits;
        }


        public List<PossibleDownloadHit> GetDownloadsForEntireSerie(SerieInfo forSerie)
        {
            var nameHit = api.ShowsSearch(forSerie.serie.Name);
            string sName = "";
            if (nameHit != null)
            {
                // Yay, we still have credits
                foreach (var y in nameHit)
                {
                    if (y.prettyName.ToLower() == forSerie.serie.Name.ToLower())
                    {
                        sName = y.name;
                        break;
                    }
                }
            }
            else
            {
                // 'Guess' the name.
                sName = onlyAlphaNumeric.Replace(forSerie.serie.Name, " ").ToLower().Replace(" ","-");
            }

            if (sName == "")
            {
                // DailyTVTorrents does not know the serie we're searching for
                // Could be caused by different names the series' known as - we assume that TVDB is right
                return null;
            }

            // API is more flexible, but has a usage limit (and a age limit as well)
            var apiHits = this.GetDownloads_Api(forSerie, sName);
            if (apiHits.Count == 0)
            {
                // Get via RSS, no limit but limited age for hits
                return this.GetDownloads_Rss(forSerie, sName);
            }
            return apiHits;
        }

        public List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(SerieInfo forSerie, List<DBO.Episode> wantedEpisodes)
        {
            throw new NotImplementedException();
        }

        public bool SupportsPartial()
        {
            return true;
        }

        public bool SupportsFull()
        {
            return false;
        }
    }
}
