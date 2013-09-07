using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using DerpScrapper.Scraper;
using DerpScrapper.DBO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Net.Http;

namespace DerpScrapper.Scrapers
{
    class SerieInfoSearchResult
    {
        public string QueryName;
        public bool FailedHit;
        public SerieInfo SerieInfo;
        public bool UncertainHit;
        public List<UncertainSerieHit> PossibleSerieHits;

        private SerieInfoSearchResult(string query)
        {
            this.QueryName = query;
        }

        public SerieInfoSearchResult(string query, bool failed)
            : this(query)
        {
            this.FailedHit = failed;
            this.UncertainHit = false;
            this.PossibleSerieHits = null;
            this.SerieInfo = null;
        }

        public SerieInfoSearchResult(string query, List<UncertainSerieHit> hits)
            : this(query)
        {
            this.FailedHit = false;
            this.UncertainHit = true;
            this.PossibleSerieHits = hits;
            this.SerieInfo = null;
        }

        public SerieInfoSearchResult(string query, SerieInfo hit)
            : this(query)
        {
            this.FailedHit = false;
            this.UncertainHit = false;
            this.PossibleSerieHits = null;
            this.SerieInfo = hit;
        }
    }

    public class UncertainSerieHit
    {
        public string Name;
        public TVDBLanguage Language;
        public Uri Page;
        public Uri Image;
    }

    public struct TVDBLanguage
    {
        public string FriendlyName;
        public string Abbreviation;
        public int Id;

        public override string ToString()
        {
            return FriendlyName;
        }
    }

    class TVDBScraper : IScraper
    {
        private const string BaseURL = "http://thetvdb.com/api";
        private const string AccountIdentifier = "E8587E005FDDDE43";

        private const string BaseViewURL = "http://thetvdb.com/?tab=series&id={0}&lid={1}";
        private const string ImageBaseURL = "http://thetvdb.com/banners";

        private static TVDBLanguage[] _languages = { 
#region TVDB Language Codes
            new TVDBLanguage() {
	            FriendlyName = "Dansk",
	            Abbreviation = "da",
	            Id = 10
            },
            new TVDBLanguage() {
	            FriendlyName = "Suomeksi",
	            Abbreviation = "fi",
	            Id = 11
            },
            new TVDBLanguage() {
	            FriendlyName = "Nederlands",
	            Abbreviation = "nl",
	            Id = 13
            },
            new TVDBLanguage() {
	            FriendlyName = "Deutsch",
	            Abbreviation = "de",
	            Id = 14
            },
            new TVDBLanguage() {
	            FriendlyName = "Italiano",
	            Abbreviation = "it",
	            Id = 15
            },
            new TVDBLanguage() {
	            FriendlyName = "Español",
	            Abbreviation = "es",
	            Id = 16
            },
            new TVDBLanguage() {
	            FriendlyName = "Français",
	            Abbreviation = "fr",
	            Id = 17
            },
            new TVDBLanguage() {
	            FriendlyName = "Polski",
	            Abbreviation = "pl",
	            Id = 18
            },
            new TVDBLanguage() {
	            FriendlyName = "Magyar",
	            Abbreviation = "hu",
	            Id = 19
            },
            new TVDBLanguage() {
	            FriendlyName = "Ελληνικά",
	            Abbreviation = "el",
	            Id = 20
            },
            new TVDBLanguage() {
	            FriendlyName = "Türkçe",
	            Abbreviation = "tr",
	            Id = 21
            },
            new TVDBLanguage() {
	            FriendlyName = "русский язык",
	            Abbreviation = "ru",
	            Id = 22
            },
            new TVDBLanguage() {
	            FriendlyName = "עברית",
	            Abbreviation = "he",
	            Id = 24
            },
            new TVDBLanguage() {
	            FriendlyName = "日本語",
	            Abbreviation = "ja",
	            Id = 25
            },
            new TVDBLanguage() {
	            FriendlyName = "Português",
	            Abbreviation = "pt",
	            Id = 26
            },
            new TVDBLanguage() {
	            FriendlyName = "中文",
	            Abbreviation = "zh",
	            Id = 27
            },
            new TVDBLanguage() {
	            FriendlyName = "čeština",
	            Abbreviation = "cs",
	            Id = 28
            },
            new TVDBLanguage() {
	            FriendlyName = "Slovenski",
	            Abbreviation = "sl",
	            Id = 30
            },
            new TVDBLanguage() {
	            FriendlyName = "Hrvatski",
	            Abbreviation = "hr",
	            Id = 31
            },
            new TVDBLanguage() {
	            FriendlyName = "한국어",
	            Abbreviation = "ko",
	            Id = 32
            },
            new TVDBLanguage() {
	            FriendlyName = "English",
	            Abbreviation = "en",
	            Id = 7
            },
            new TVDBLanguage() {
	            FriendlyName = "Svenska",
	            Abbreviation = "sv",
	            Id = 8
            },
            new TVDBLanguage() {
	            FriendlyName = "Norsk",
	            Abbreviation = "no",
	            Id = 9
            }
#endregion
        };

        public ResourceSite GetResourceSite()
        {
            ResourceSite site = new ResourceSite(1);

            return site;
        }

        private static TVDBLanguage GetLanguage(string abbreviation)
        {
            return _languages.Where(l => l.Abbreviation == abbreviation).FirstOrDefault();
        }

        private static TVDBLanguage GetLanguage(int id)
        {
            return _languages.Where(l => l.Id == id).FirstOrDefault();
        }

        public async Task<SerieInfoSearchResult> FindSerie(string seriesQuery)
        {
            string contents = await ScraperUtility.GetContentOfUrl(BaseURL + "/GetSeries.php?seriesname=" + seriesQuery);
            var document = new XmlDocument();
            document.LoadXml(contents);

            var seriesNodes = document.SelectNodes("//Series");
            if (seriesNodes.Count == 0)
            {
                return new SerieInfoSearchResult(seriesQuery, true);
            }
            else if (seriesNodes.Count == 1)
            {
                // Likely success
                var serieNode = seriesNodes[0];
                var serieInfo = new SerieInfo();
                serieInfo.serie.Name = serieNode.SelectSingleNode("/SeriesName").InnerText;


                return new SerieInfoSearchResult(seriesQuery, serieInfo);
            }
            else
            {
                var uncertainHits = new List<UncertainSerieHit>();
                foreach (XmlNode serieNode in seriesNodes)
                {
                    var language = TVDBScraper.GetLanguage(serieNode["language"].InnerText);
                    string seriesId = serieNode["seriesid"].InnerText;
                    uncertainHits.Add(new UncertainSerieHit()
                    {
                        Name = serieNode["SeriesName"].InnerText,
                        Language = language,
                        Image = await GetPosterImageUrlForSeriesId(seriesId),
                        Page = new Uri(string.Format(BaseViewURL, seriesId, language.Id))
                    });
                }

                return new SerieInfoSearchResult(seriesQuery, uncertainHits);
            }
        }

        private async Task<Uri> GetPosterImageUrlForSeriesId(string seriesId)
        {
            string baseFormat = ImageBaseURL + "/posters/" + seriesId + "-1.jpg";

            HttpClient httpClient = new HttpClient();
            Uri requestUri = new Uri(baseFormat);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, requestUri);

            HttpResponseMessage response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return requestUri;
            }
            return null;
        }

        public async Task<SerieInfoSearchResult> FindAllInformationForSerie(string seriesQuery)
        {
            return null;
        }

        private static IOrderedEnumerable<Episode> OrderByEpNr(List<Episode> episodeList)
        {
            return episodeList.OrderBy(x => x.EpisodeNumber);
        }

        private static PossibleSearchHit FindEnglish(List<PossibleSearchHit> searchHits)
        {
            PossibleSearchHit englishHit = searchHits.Where(
                item => item.languageName.ToLower() == "english"
            ).FirstOrDefault();
            return englishHit;
        }

        private static HtmlNode NewMethod(HtmlDocument metaDataDoc)
        {
            var wtfAmIDoing = metaDataDoc.DocumentNode.Descendants("h1").Where(p => p.InnerText == "Information").FirstOrDefault();
            return wtfAmIDoing;
        }

        SerieInfo IScraper.FindAllInformationForSerie(string serieName)
        {
            throw new NotImplementedException();
        }

        ResourceSite IScraper.GetResourceSite()
        {
            throw new NotImplementedException();
        }
    }
}
