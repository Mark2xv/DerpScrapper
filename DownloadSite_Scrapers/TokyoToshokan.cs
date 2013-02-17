using System;
using System.Collections.Generic;
using System.Linq;
using DerpScrapper.DBO;
using DerpScrapper.Scraper;

namespace DerpScrapper.DownloadSite_Scrapers
{
    class TokyoToshokan : IDownloadSite
    {
        Uri baseUri = new Uri("http://tokyotosho.info");
        string format = "/search.php?terms={0}&type=1&size_min=50";

        public List<PossibleDownloadHit> GetDownloadsForEntireSerie(DBO.Serie serie, List<Episode> epInfo)
        {
            var doc = ScraperUtility.HTMLDocumentOfContentFromURL(new Uri(baseUri, string.Format(format, serie["Name"])).AbsoluteUri);
            var table = doc.DocumentNode.Descendants("table").First().Descendants("tr");

            return null;
        }

        public List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(DBO.Serie serie, List<SeasonEpisode> episodes, List<Episode> epInfo)
        {
            throw new NotImplementedException();
        }
    }
}
