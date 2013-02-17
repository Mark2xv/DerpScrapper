using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DerpScrapper.DBO;

namespace DerpScrapper
{
    interface IDownloadSite
    {
        List<PossibleDownloadHit> GetDownloadsForEntireSerie(Serie serie, List<Episode> episodeInfo);
        List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(Serie serie, List<SeasonEpisode> episodes, List<Episode> episodeInfo);
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
