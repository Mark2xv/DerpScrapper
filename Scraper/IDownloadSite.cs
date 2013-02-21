using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DerpScrapper.DBO;

namespace DerpScrapper
{
    interface IDownloadSite
    {
        bool SupportsPartial();
        bool SupportsFull();

        List<PossibleDownloadHit> GetDownloadsForEntireSerie(SerieInfo forSerie);
        List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(SerieInfo forSerie, List<Episode> wantedEpisodes);
    }

    public class SeasonEpisode
    {
        public SeasonEpisode(int seasonNumber, int episodeNumber, string fileName = "")
        {
            this.seasonNumber = seasonNumber;
            this.episodeNumber = episodeNumber;
            this.fileName = fileName;
        }

        public override string ToString()
        {
            return (fileName == "" ? "" : "") + "(" + this.seasonNumber + "x" + this.episodeNumber + ")";
        }
        public int seasonNumber;
        public int episodeNumber;
        public string fileName;
    }

}
