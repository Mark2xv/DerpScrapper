using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using DerpScrapper.Scraper;
using DerpScrapper.DBO;

namespace DerpScrapper.Scrapers
{
    class TVDBScraper : IScraper
    {
        public ResourceSite GetResourceSite()
        {
            ResourceSite site = new ResourceSite(1);

            return site;
        }

        public SerieInfo FindAllInformationForSerie(string seriesQuery)
        {
            var site = this.GetResourceSite();

            SerieInfo serieInfo = new SerieInfo();
            serieInfo.resource.ResourceSiteId = site.Id;

            Console.WriteLine("Searching for information on series \"" + seriesQuery + "\"");
            HtmlAgilityPack.HtmlDocument searchDocument = ScraperUtility.HTMLDocumentOfContentFromURL("http://thetvdb.com/?string=" + seriesQuery.Replace(" ", "+") + "&searchseriesid=&tab=listseries&function=Search");
            List<PossibleSearchHit> searchHits = new List<PossibleSearchHit>();
            var searchTableHits = searchDocument.GetElementbyId("listtable");
            bool firstNode = true;
            foreach (HtmlNode node in searchTableHits.Descendants("tr"))
            {
                if (firstNode)
                {
                    firstNode = false;
                    continue;
                }

                int tdNumber = 0;
                PossibleSearchHit searchHit = new PossibleSearchHit();
                foreach (HtmlNode subNode in node.Descendants("td"))
                {
                    if (tdNumber == 0)
                    {
                        // link stuff
                        searchHit.seriesName = subNode.FirstChild.InnerText;

                        var parts = subNode.FirstChild.Attributes["href"].Value.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                        searchHit.languageId = int.Parse(ScraperUtility.PartAfterEquals(parts[2]));

                    }
                    else if (tdNumber == 1)
                    {
                        searchHit.languageName = subNode.InnerText;
                    }
                    else if (tdNumber == 2)
                    {
                        searchHit.seriesId = int.Parse(subNode.InnerText);
                    }
                    tdNumber++;
                }
                searchHits.Add(searchHit);
            }

            PossibleSearchHit preferredHit = null;
            PossibleSearchHit englishHit = FindEnglish(searchHits);

            if (englishHit == null)
            {
                // find out which other hit is preffered

            }
            else
            {
                preferredHit = englishHit;
            }

            if (preferredHit == null)
            {
                // shit.
            }


            serieInfo.serie.Name = preferredHit.seriesName;

            Console.WriteLine("Getting metadata of series id " + preferredHit.seriesId.ToString() + " (Name = " + preferredHit.seriesName + ") ...");
            HtmlDocument metaDataDoc = ScraperUtility.HTMLDocumentOfContentFromURL("http://thetvdb.com/?tab=series&id=" + preferredHit.seriesId + "&lid=" + preferredHit.languageId);

            serieInfo.resource.ResourceSiteId = preferredHit.seriesId;
            serieInfo.resource.ResourceSiteUrl = "http://thetvdb.com/?tab=series&id=" + preferredHit.seriesId + "&lid=" + preferredHit.languageId;

            var contentNode = metaDataDoc.DocumentNode.Descendants("div").Where(p => p.Id == "content").FirstOrDefault();
            string seriesSynopsis = contentNode.LastChild.InnerText.Trim();

            var wtfAmIDoing = NewMethod(metaDataDoc);
            var noIdsAnywhere = wtfAmIDoing.NextSibling.NextSibling;

            foreach (var what in noIdsAnywhere.FirstChild.NextSibling.FirstChild.Descendants("tr"))
            {
                var tds = what.Descendants("td");
                var td1 = tds.ElementAt(0).InnerText.Trim().Trim(':');
                var td2 = tds.ElementAt(1).InnerText.Trim();

                switch (td1)
                {
                    case "First Aired":
                        DateTime dt = DateTime.Parse(td2);
                        serieInfo.metadata.FirstAired = dt.ToUnixTimestamp();
                        break;
                    case "Air Day":
                        serieInfo.metadata.Airday = td2;
                        break;
                    case "Air Time":
                        //
                        break;
                    case "Runtime":
                        serieInfo.metadata.Runtime = (int)TimeSpan.Parse(td2.Split(' ')[0]).TotalSeconds;
                        break;
                    case "Network":
                        serieInfo.metadata.Network = td2;
                        break;
                    case "Genre":
                        string[] genres = tds.ElementAt(1).InnerHtml.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string genre in genres)
                        {
                            Genre gen = Genre.GetGenre(genre);
                            SerieGenre serieGenre = new SerieGenre();
                            serieGenre.GenreId = gen.Id;
                            serieInfo.genres.Add(serieGenre);
                        }
                        break;
                }
            }
            string[] lines = noIdsAnywhere.InnerText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine("Getting contents of series id " + preferredHit.seriesId.ToString() + " (Name = " + preferredHit.seriesName + ") ...");
            HtmlDocument episodeListDoc = ScraperUtility.HTMLDocumentOfContentFromURL("http://thetvdb.com/?tab=seasonall&id=" + preferredHit.seriesId + "&lid=" + preferredHit.languageId);

            List<Episode> episodeList = new List<Episode>();
            var table = episodeListDoc.GetElementbyId("listtable");
            foreach (HtmlNode node in table.Descendants("tr"))
            {
                bool wasHead = false;
                Episode ep = new Episode();
                int propNr = 0;
                foreach (HtmlNode subNode in node.Descendants("td"))
                {
                    if (subNode.HasAttributes && subNode.Attributes["class"].Value == "head")
                    {
                        wasHead = true;
                        break;
                    }

                    if (propNr == 0)
                    {
                        string[] parts;
                        // get contents of <a> tag (innertext)
                        if (subNode.FirstChild.InnerText != "Special")
                        {
                            parts = subNode.FirstChild.InnerText.Split(new[] { " x " }, StringSplitOptions.RemoveEmptyEntries);
                            ep.SeasonNumber = int.Parse(parts[0]);
                            ep.EpisodeNumber = int.Parse(parts[1]);
                        }
                        else
                        {
                            ep.isSpecial = true;
                            ep.EpisodeNumber = -1;
                            ep.SeasonNumber = -1;
                        }

                        string href = subNode.FirstChild.Attributes["href"].Value;
                        parts = href.Split(new[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                        ep.SerieId = int.Parse(ScraperUtility.PartAfterEquals(parts[1]));
                        ep.SeasonId = int.Parse(ScraperUtility.PartAfterEquals(parts[2]));
                        ep.EpisodeId = int.Parse(ScraperUtility.PartAfterEquals(parts[3]));
                    }
                    else if (propNr == 1)
                    {
                        ep.EpisodeName = subNode.FirstChild.InnerText;
                    }
                    else if (propNr == 2)
                    {
                        ep.AirDate = DateTime.Parse(subNode.InnerText).ToUnixTimestamp();
                    }
                    else if (propNr == 3)
                    {
                        ep.hasImage = (subNode.HasChildNodes && subNode.FirstChild.Name == "img" && subNode.FirstChild.Attributes["src"].Value.Contains("checkmark"));
                    }
                    else
                    {
                        // wat
                    }


                    propNr++;
                }
                if (wasHead)
                    continue;

                episodeList.Add(ep);
            }

            for (int i = 0; i < episodeList.Count; i++)
            {
                Episode ep = episodeList.ElementAt(i);
                string url = "http://thetvdb.com/?tab=episode&seriesid=" + episodeList[i].SerieId + "&seasonid=" + episodeList[i].SeasonId + "&id=" + episodeList[i].EpisodeId + "&lid=" + preferredHit.languageId;

                Console.WriteLine("Getting contents for {0} - episode {1} : \"{2}\"", preferredHit.seriesName, episodeList[i].EpisodeNumber, episodeList[i].EpisodeName);
                HtmlAgilityPack.HtmlDocument episodeDoc = ScraperUtility.HTMLDocumentOfContentFromURL(url);

                string synopsys = (from textarea in episodeDoc.DocumentNode.Descendants("textarea")
                                   where textarea.HasAttributes && textarea.Attributes["name"].Value == "Overview_7" // scary. What if the fieldname changes?
                                   select textarea.InnerText).FirstOrDefault();

                ep.Synopsis = synopsys;
            }

            serieInfo.episodes.AddRange(OrderByEpNr(episodeList));

            return serieInfo;
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
    }
}
