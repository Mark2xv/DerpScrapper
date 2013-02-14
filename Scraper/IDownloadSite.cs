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

    public struct SeasonEpisode
    {
        public SeasonEpisode(int seasonNumber, int episodeNumber)
        {
            this.seasonNumber = seasonNumber;
            this.episodeNumber = episodeNumber;
        }
        public int seasonNumber;
        public int episodeNumber;
    }

}
