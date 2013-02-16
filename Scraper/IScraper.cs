using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using DerpScrapper.DBO;

namespace DerpScrapper
{
    interface IScraper
    {
        SerieInfo FindAllInformationForSerie(string serieName);

        ResourceSite GetResourceSite();
    }

    class PossibleSearchHit
    {
        public int seriesId;
        public string seriesName;
        public string languageName;
        public int languageId;
    }

    class SerieInfo
    {
        public Serie serie = new Serie();
        public List<Episode> episodes = new List<Episode>();
        public List<SerieGenre> genres = new List<SerieGenre>();
        public SerieMetadata metadata = new SerieMetadata();
        public SerieResource resource = new SerieResource();
    }

    public class PossibleDownloadHit
    {
        public string origSerieName;

        public int preference = 0;
        public string name;
        public string url;
        public string infoPageUrl;

        public bool isComplete = false;
        public List<SeasonEpisode> episodes;

        public float fileSize;

        public bool unSure;
    }
}
