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

        public QualityInformation qualityInfo;

        public override string ToString()
        {
            string s = "PossibleDownloadHit: \n\tName = {0}\n\tInfoUrl = {1}\n\tPreference = {2}\n\tQualInfo = {3}\n\tEps = {4}";
            string eps = "";
            foreach (var x in episodes)
            {
                eps += "\t\t" + x.ToString() + "\n";
            }

            return string.Format(s, name, infoPageUrl, preference, qualityInfo, eps);
        }
    }

    public struct QualityInformation
    {
        public override string ToString()
        {
            return "S=" + this.source.ToString() + " @ " + "E=" + this.encoding.ToString() + " R=" + this.resolution.ToString();
        }

        public enum Source
        {
            Unset,
            BlueRay,
            HDTV,
            DVD,
            Unknown_Lower
        }

        public enum Resolution
        {
            Unset,
            HD1080,
            HD720,
            WVGA,
            Unknown_Lower
        }

        public enum Encoding
        {
            Unset,
            TenBit,
            EightBit,
            Unknown_Lower
        }

        public QualityInformation(Source s, Resolution r, Encoding e) {
            this.resolution = r;
            this.source = s;
            this.encoding = e;
        }

        public int PreferenceBonus
        {
            get
            {
                int p = 0;
                switch (source)
                {   
                    case Source.BlueRay:
                        p += 3;
                        break;
                    case Source.HDTV:
                        p += 2;
                        break;
                    case Source.DVD:
                        p += 1;
                        break;
                    default:
                        break;
                }
                switch (resolution)
                {
                    case Resolution.HD1080:
                        p += 2;
                        break;
                    case Resolution.HD720:
                        p += 1;
                        break;
                    case Resolution.WVGA:
                    case Resolution.Unknown_Lower:
                    default:
                        break;
                }

                if (encoding == Encoding.TenBit)
                    p++;

                return p;
            }
        }

        public Source source;
        public Resolution resolution;
        public Encoding encoding;
    }
}
