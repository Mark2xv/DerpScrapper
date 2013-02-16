using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using DerpScrapper.DBO;
using DerpScrapper.Scraper;
using HtmlAgilityPack;

namespace DerpScrapper.DownloadSite_Scrapers
{
    class Nyaa : IDownloadSite
    {
        enum TrustLevel
        {
            Remake = 0,
            None = 1,
            Trusted = 2,
            APlus = 3
        }

        TrustLevel trustLevel = TrustLevel.None;
        private string baseUrl = "http://nyaa.eu/?page=search&minsize=50&cats=1_37&filter={1}&term={0}&offet={2}";
        private char[] splitBySpace = new[] { ' ' };
        private List<Tuple<char, char>> removeCharCombos = new List<Tuple<char, char>>(new[] { 
            new Tuple<char,char>('[', ']'),
            new Tuple<char,char>('(', ')'),
            new Tuple<char,char>('{', '}'),
            new Tuple<char,char>('<', '>')
        });
        private string[] acceptedFileTypes = new string[] { 
            "mkv", "avi", "wmv", "xvid", "divx", "mpg", "mp4"
        };
        private Regex onlyNumbers = new Regex(@"[^\d]");
        private Regex oneDecimal = new Regex(@"^\d+$");
        private Regex onlyAlphaNumeric = new Regex(@"[^\w\s]");
        private Regex versionCheck = new Regex(@"[v]{1}\d{1}");
        private Regex isBdBatch = new Regex(@"vol(ume)?(.|\s)(\d)((\s|,)\d)*", RegexOptions.IgnoreCase);
        private Regex isEpisodeFile = new Regex(@"episode(.|\s)(\d)+$", RegexOptions.IgnoreCase);
        private Regex special = new Regex(@"(ova|special)$", RegexOptions.IgnoreCase);
        private Regex endsWithDecimal = new Regex(@"\d+$");
        private Regex isRangeIndicator = new Regex(@"\d+[~-]{1}\d+");

        private List<int> episodesFound = new List<int>();

        public List<PossibleDownloadHit> GetDownloadsForEntireSerie(Serie serie, List<Episode> knownEpisodes)
        {
            return this.getListOfAllDownloadsForName(serie["Name"].ToString(), knownEpisodes);
        }

        private List<SeasonEpisode> getEpisodeNumbersFromUrl(string serieName, string url, PossibleDownloadHit forHit)
        {
            HtmlDocument doc = ScraperUtility.HTMLDocumentOfContentFromURL(url + "&showfiles=1");
            HtmlNode fileList = doc.GetNodeWithTypeAndClass("table", "tinfofiletable");

            var list = new List<SeasonEpisode>();

            foreach (var node in fileList.Descendants("tr"))
            {
                string title = node.ChildNodes[0].InnerText;
                string size = node.ChildNodes[1].InnerText;

                string clTitle = ScraperUtility.CleanUpName(title, removeCharCombos).Replace("_", " ");

                int idxOfPeriod = title.LastIndexOf('.') + 1;
                string ext = title.Substring(idxOfPeriod).Trim();
                string noExt = title.Substring(0, idxOfPeriod - 1).Trim();

                if (ext != "")
                {
                    // Do the same for clTitle.
                    idxOfPeriod = clTitle.LastIndexOf('.') + 1;
                    clTitle = clTitle.Substring(0, idxOfPeriod - 1).Trim();
                }

                if (acceptedFileTypes.Contains(ext))
                {
                    if (clTitle.Matches(isEpisodeFile))
                    {
                        var match = isEpisodeFile.Match(clTitle);
                        if (match.Success)
                        {
                            string epNr = match.Value.Replace("episode ", "");

                            int guessEpisodeNumber;
                            bool success = int.TryParse(epNr, out guessEpisodeNumber);
                            if (success)
                            {
                                Console.WriteLine("Found " + title + " fitting to be episode number " + guessEpisodeNumber + "  | parse method = episoderegex");
                                list.Add(new SeasonEpisode(1, guessEpisodeNumber));
                            }
                            else
                            {
                                // .. did match episode x regex, did not come out with valid epnr.. wat
                            }
                        }
                        else
                        {
                            // It's using some other standard...?
                        }
                    }
                    else
                    {
                        int epNr;
                        // use alternate parsing

                        // Start off by trying to get everything after "-"
                        int lastIndexOfDash = clTitle.LastIndexOf('-');
                        string afterDash = clTitle.Substring(lastIndexOfDash + 1).Trim();
                        if (int.TryParse(afterDash, out epNr))
                        {
                            Console.WriteLine("Found " + title + " fitting to be episode number " + epNr + "  | parse method = last dash");
                            list.Add(new SeasonEpisode(1, epNr));
                        }
                        else
                        {
                            // If that fails, try with " "
                            int lastIndexOfSpace = clTitle.LastIndexOf(' ');
                            string afterSpace = clTitle.Substring(lastIndexOfSpace + 1).Trim();
                            if (int.TryParse(afterSpace, out epNr))
                            {
                                Console.WriteLine("Found " + title + " fitting to be episode number " + epNr + "  | parse method = last space");
                                list.Add(new SeasonEpisode(1, epNr));
                            }
                            else
                            {
                                Console.WriteLine("Uncertain about download {0} - could not figure out what {1} is.", forHit.name, title);
                                forHit.unSure = true;
                                continue;
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("Unparsable file \"{0}\"", title);
                }
            }
            // TODO: lol hack
            list.OrderBy(p => p.seasonNumber * 1000 + p.episodeNumber);
            return list;
        }

        private PossibleDownloadHit IsBDVolumeBatch(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            if (cleanedName.Matches(isBdBatch))
            {
                // Get the episodes from the detailpage
                var eps = this.getEpisodeNumbersFromUrl(baseHit.origSerieName, baseHit.infoPageUrl, baseHit);
                baseHit.episodes.AddRange(eps);
                baseHit.preference += (int)Math.Ceiling((float)eps.Count / 3);

                return baseHit;
            }
            return null;
        }

        private PossibleDownloadHit IsSpecial(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            int idxOfPeriod = cleanedName.LastIndexOf('.') + 1;
            string noExtension;
            if (idxOfPeriod != 0)
            {
                string ext = cleanedName.Substring(idxOfPeriod).Trim();
                noExtension = cleanedName.Substring(0, Math.Max(0, idxOfPeriod - 1)).Trim();
            }
            else
            {
                noExtension = cleanedName;
            }

            string noSerieName = noExtension.Replace(baseHit.origSerieName.ToLower(), "");
            if (noSerieName.Contains("ova") || noSerieName.Contains("special"))
            {
                // assume season 0 episode (special, extra, whatever)
                string noS = onlyAlphaNumeric.Replace(noSerieName, "").Replace("  ", " ");
                string x = special.Replace(noS, "").Trim();

                Match m = endsWithDecimal.Match(x);
                if (m.Success)
                {
                    string firstPart = x.Substring(0, m.Index).Trim();
                    string matchHit = m.Value;

                    int epVal;
                    bool s = int.TryParse(matchHit, out epVal);
                    if (s)
                    {
                        // Look through known ep info is there's any known serie with specifications as such
                        string f = string.Format("{0} {1}", firstPart, epVal);
                        var specialEpisodeMatch = FindEp(episodeInfo, f);
                        if (specialEpisodeMatch != null)
                        {
                            // actually found it!
                            Console.WriteLine("Matched {0} to special episode {1}", baseHit.name, specialEpisodeMatch.EpisodeName);
                            baseHit.episodes.Add(new SeasonEpisode(-1, epVal));
                        }
                        else
                        {
                            // nope.
                        }
                    }
                    else
                    {
                        // unparsable?
                    }
                }
                else
                {
                    // Only parse for ep name.. 
                }
            }
            return baseHit;
        }

        private PossibleDownloadHit IsRange(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            return null;
        }

        private PossibleDownloadHit AfterDash(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            return null;
        }

        private PossibleDownloadHit AfterSpace(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            return null;
        }

        private PossibleDownloadHit BruteForce(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            return null;
        }

        private PossibleDownloadHit ParseItem(XmlNode itemNode, string serieName, List<Episode> epInfo)
        {
            PossibleDownloadHit hit = null;

            string title = itemNode.ChildNodes[0].InnerText;
            Console.WriteLine("Begin parsing of item " + title);

            string url = itemNode.ChildNodes[2].InnerText;
            string infoPage = itemNode.ChildNodes[3].InnerText;
            string desc = itemNode.ChildNodes[4].InnerText;

            var parts = desc.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var parts2 = parts[2].Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            var sizeParts = parts2[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            float fileSize = float.Parse(sizeParts[0]);
            if (sizeParts[1] == "GiB")
            {
                fileSize *= 1000;
            }

            PossibleDownloadHit baseHit = new PossibleDownloadHit();
            baseHit.origSerieName = serieName;
            baseHit.name = title;
            baseHit.infoPageUrl = infoPage;
            baseHit.url = url;
            baseHit.fileSize = fileSize;

            if (parts2.Length >= 3) // TrustLevel > None
            {
                string trustLevel = parts2[2].Trim();
                switch (trustLevel)
                {
                    case "Remake":
                        hit.preference -= 1;
                        break;
                    case "Trusted":
                        hit.preference += 1;
                        break;
                    case "A+":
                        hit.preference += 2;
                        break;
                    default:
                        break;
                }
            }


            // Fix name so there's no more underscores (It's fucking 2013. Come on now.), html entities and all useless stuff between brackets
            string cleanName = ScraperUtility.CleanUpName(WebUtility.HtmlDecode(title).Replace('_', ' '), removeCharCombos);

            // Basically, try to parse the hit with all different methods. If one returns a usable result, it's good enough
            var funcs = new Func<string, PossibleDownloadHit, List<Episode>, PossibleDownloadHit>[] { 
                IsBDVolumeBatch,
                IsSpecial,
                IsRange,
                AfterDash,
                AfterSpace,
                BruteForce
            };

            foreach (var func in funcs)
            {
                var pHit = func(cleanName, baseHit, epInfo);
                if (pHit != null && pHit.preference > 0 && pHit.episodes.Count > 0)
                {
                    Console.WriteLine("Found result {0} with method: {1}", pHit.name, func.Method.Name);
                    if (func == BruteForce)
                        hit.preference -= 2;

                    hit = pHit;
                    break;
                }
                else
                {
                    Console.WriteLine("Did not find result with method {0}", func.Method.Name);
                }
            }

            return hit;

            string noExtension = cleanName;

            List<SeasonEpisode> epList = new List<SeasonEpisode>();
            // If the item contains 'Volume 1' or 'Vol 1' or something along those lines, assume its a BD batch -> we'll need to get the episodes from a seperate info page
            //      The filelist is not embedded in the RSS, so we'll need to fetch that seperately.
            if (cleanName.Matches(isBdBatch))
            {
                // Get the episodes from the detailpage
                var eps = this.getEpisodeNumbersFromUrl(serieName, infoPage, hit);
                epList.AddRange(eps);
                hit.preference += (int)Math.Ceiling((float)eps.Count / 3);
            }
            else
            {
                // Guess the episode from this page, else fall back to batch check

                int idxOfPeriod = cleanName.LastIndexOf('.') + 1;
                string ext = cleanName.Substring(idxOfPeriod).Trim();
                noExtension = cleanName.Substring(0, Math.Max(0, idxOfPeriod - 1)).Trim();
                if (acceptedFileTypes.Contains(ext))
                {
                }
                else
                {

                    // Okay... no usable extension. つづく...

                    if (cleanName == serieName.ToLower() || ScraperUtility.Levenshtein(serieName.ToLower(), cleanName.ToLower()) <= 3)
                    {
                        Console.WriteLine("Assumed {0} is a batch (name is, or looks like query {1})", title, serieName);
                        // Assume batch
                        //      Get the episodes from the detailpage
                        var eps = this.getEpisodeNumbersFromUrl(serieName, infoPage, hit);
                        epList.AddRange(eps);
                        hit.preference += (int)Math.Ceiling((float)eps.Count / 3);
                    }
                    else
                    {
                        // Torrent name does not look like query (enough), has no extension.. what do I do with this?

                        // Try the 'last space' tactic ( a bit different though)
                        int lastIndexOfSpace = cleanName.LastIndexOf(' ');
                        string afterSpace = cleanName.Substring(lastIndexOfSpace).Trim();

                        Match match = isRangeIndicator.Match(afterSpace);
                        if (match.Success)
                        {
                            var rangeInd = match.Value;
                            var epRangeParts = afterSpace.Split(new char[] { '-', '~' }, StringSplitOptions.RemoveEmptyEntries);

                            if (epRangeParts.Length == 2) // as it should be
                            {
                                int e1 = int.Parse(epRangeParts[0]);
                                int e2 = int.Parse(epRangeParts[1]);

                                Console.WriteLine("Matched hit \"{0}\" to group of episodes:", title);
                                for (int cEp = e1; cEp <= e2; cEp++)
                                {
                                    Console.WriteLine("\t" + cEp);
                                    epList.Add(new SeasonEpisode(1, cEp));
                                }
                            }
                            else
                            {
                                // I really don't know
                            }
                        }
                        else
                        {
                            // No match using normal method

                            int epNr;
                            bool success = int.TryParse(afterSpace, out epNr);
                            if (success)
                            {
                                Console.WriteLine("Found " + title + " fitting to be episode number " + epNr + "  | parse method = last space");
                                epList.Add(new SeasonEpisode(1, epNr));
                            }
                            else
                            {
                                // Once again, fall back to last dash (instead of last space)

                            }

                        }
                    }
                }
            }
            hit.episodes = epList;

            if (hit.episodes.Count == 0)
            {
                Console.WriteLine("Failed to get any usable information from hit \"{0}\"", title);
                return null;
            }

            hit.name = title;
            hit.url = url;
            hit.fileSize = fileSize;

            if (hit.preference >= 0)
                return hit;
            Console.WriteLine("Discarded hit: " + hit.name + ": Negative preference");
            return null;
        }

        private static Episode FindEp(List<Episode> epInfo, string f)
        {
            return epInfo.Where(p => p.EpisodeName.ToLower() == f).FirstOrDefault();
        }

        private List<PossibleDownloadHit> getListOfAllDownloadsForName(string name, List<Episode> knownEpisodes)
        {
            var list = new List<PossibleDownloadHit>();

            string rssUrl = string.Format("http://www.nyaa.eu/?page=rss&cats=1_37&term={0}", name);
            Console.WriteLine("Getting contents of url: " + rssUrl + " ...");
            string rss = ScraperUtility.GetContentOfUrl(rssUrl);
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            Console.WriteLine("Parsing XML...");
            doc.LoadXml(rss);
            for (int i = 0; i < doc.DocumentElement.FirstChild.ChildNodes.Count; i++)
            {
                XmlNode node = doc.DocumentElement.FirstChild.ChildNodes[i];
                // Skip first 5 items - they are descriptors of the RSS Feed
                if (node.Name != "item") continue;
                PossibleDownloadHit hit = this.ParseItem(node, name, knownEpisodes);
                if (hit != null)
                    list.Add(hit);
            }

            return list;
        }

        public List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(Serie serie, List<SeasonEpisode> episodes, List<Episode> epInfo)
        {
            throw new NotImplementedException();
        }
    }
}
