﻿using System;
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
            var list = this.getListOfAllDownloadsForName(serie["Name"].ToString(), knownEpisodes);

            var key = 1;
            foreach (var x in list)
            {
                Log.WriteLine(key + " (p="+x.preference+") | " + x.name + " | cEp = " + x.episodes.Count);
                key++;
            }

            return list;
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
                                list.Add(new SeasonEpisode(1, guessEpisodeNumber, title));
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
                            list.Add(new SeasonEpisode(1, epNr, title));
                        }
                        else
                        {
                            // If that fails, try with " "
                            int lastIndexOfSpace = clTitle.LastIndexOf(' ');
                            string afterSpace = clTitle.Substring(lastIndexOfSpace + 1).Trim();
                            if (int.TryParse(afterSpace, out epNr))
                            {
                                list.Add(new SeasonEpisode(1, epNr, title));
                            }
                            else
                            {
                                forHit.unSure = true;
                                continue;
                            }

                        }
                    }
                }
                else
                {
                    Log.WriteLine("Unparsable file \"{0}\"", title);
                }
            }
            // TODO: lol hack
            list.OrderBy(p => p.seasonNumber * 1000 + p.episodeNumber);
            return list;
        }

        private Tuple<string,string> splitExt(string input)
        {
            int idxOfPeriod = input.LastIndexOf('.') + 1;
            string noExtension;
            if (idxOfPeriod != 0)
            {
                string ext = input.Substring(idxOfPeriod).Trim();
                noExtension = input.Substring(0, Math.Max(0, idxOfPeriod - 1)).Trim();
                return new Tuple<string, string>(noExtension, ext);
            }
            else
            {
                return new Tuple<string, string>(input, "");
            }
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
                            Log.WriteLine("Matched {0} to special episode {1}", baseHit.name, specialEpisodeMatch.EpisodeName);
                            baseHit.episodes.Add(new SeasonEpisode(-1, epVal, baseHit.name));
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
            var parts = splitExt(cleanedName);

            string afterSpace = parts.Item1.Substring(parts.Item1.LastIndexOf(' ')).Trim();

            Match match = isRangeIndicator.Match(afterSpace);
            if (match.Success)
            {
                var rangeInd = match.Value;
                var epRangeParts = afterSpace.Split(new char[] { '-', '~' }, StringSplitOptions.RemoveEmptyEntries);

                if (epRangeParts.Length == 2) // as it should be
                {
                    int e1 = int.Parse(epRangeParts[0]);
                    int e2 = int.Parse(epRangeParts[1]);

                    for (int cEp = e1; cEp <= e2; cEp++)
                    {
                        baseHit.episodes.Add(new SeasonEpisode(1, cEp, "(implied by filename"));
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

            return baseHit;
        }

        private PossibleDownloadHit AfterDash(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            var parts = splitExt(cleanedName);

            int idxOfDash = parts.Item1.LastIndexOf('-') + 1;
            if (idxOfDash != 0)
            {
                string epNrPart = parts.Item1.Substring(idxOfDash).Trim();
                int epNr;
                bool success = int.TryParse(epNrPart, out epNr);
                if (success && FindEp(episodeInfo, epNr) != null) // If parsing succeeded and episode number exists
                {
                    baseHit.episodes.Add(new SeasonEpisode(1, epNr, baseHit.name));
                    return baseHit;
                }
                return null;
            }
            return null;
        }

        private PossibleDownloadHit AfterSpace(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            var parts = splitExt(cleanedName);

            int idxOfDash = parts.Item1.LastIndexOf(' ') + 1;
            if (idxOfDash != 0)
            {
                string epNrPart = parts.Item1.Substring(idxOfDash).Trim();
                int epNr;
                bool success = int.TryParse(epNrPart, out epNr);
                if (success && FindEp(episodeInfo, epNr) != null) // If parsing succeeded and episode number exists
                {
                    baseHit.episodes.Add(new SeasonEpisode(1, epNr, baseHit.name));
                    return baseHit;
                }
                return null;
            }
            return null;
        }

        private PossibleDownloadHit IsComplete(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            if (cleanedName == baseHit.origSerieName.ToLower())
            {
                // Name of entry is exactly the same as query - assume complete?
                // No. Check entry page.
                var eps = this.getEpisodeNumbersFromUrl(baseHit.origSerieName, baseHit.infoPageUrl, baseHit);
                baseHit.episodes.AddRange(eps);
                baseHit.preference += (int)Math.Ceiling((float)eps.Count / 3);
            }
            else if (ScraperUtility.Levenshtein(cleanedName, baseHit.origSerieName.ToLower()) <= 3) // Name is pretty much the same, save for -some- characters.
            {
                Log.WriteLine("Assumption made: " + cleanedName + " ~=~ " + baseHit.origSerieName + ". Correct?");
                
                var eps = this.getEpisodeNumbersFromUrl(baseHit.origSerieName, baseHit.infoPageUrl, baseHit);
                baseHit.episodes.AddRange(eps);
                baseHit.preference += (int)Math.Ceiling((float)eps.Count / 3);
            }

            return baseHit;
        }

        private PossibleDownloadHit BruteForce(string cleanedName, PossibleDownloadHit baseHit, List<Episode> episodeInfo)
        {
            return null;
        }

        private PossibleDownloadHit ParseItem(XmlNode itemNode, string serieName, List<Episode> epInfo)
        {
            Log.WriteLine("\n------------------------------------------\n");

            PossibleDownloadHit hit = null;

            string title = itemNode.ChildNodes[0].InnerText;

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
            baseHit.episodes = new List<SeasonEpisode>();
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
                        baseHit.preference -= 1;
                        break;
                    case "Trusted":
                        baseHit.preference += 1;
                        break;
                    case "A+":
                        baseHit.preference += 2;
                        break;
                    default:
                        break;
                }
            }

            string name = WebUtility.HtmlDecode(title).Replace('_', ' ');

            // Fix name so there's no more underscores (It's fucking 2013. Come on now.), html entities and all useless stuff between brackets
            string cleanName = ScraperUtility.CleanUpName(name, removeCharCombos);

            List<string> tags = ScraperUtility.TagContents(name, removeCharCombos);
            foreach (string tag in tags)
            {
                switch (tag.ToLower())
                {
                    case "mkv":
                        baseHit.preference++;
                        break;

                    case "mp4":
                        baseHit.preference--;
                        break;

                    case "hi10":
                    case "hi10p":
                    case "10bit":
                        baseHit.preference++;
                        break;

                    case "1080p":
                    case "1920x1080":
                        baseHit.preference += 2;
                        break;

                    case "720p":
                    case "1280x720":
                        baseHit.preference++;
                        break;

                    case "bd":
                    case "bluray":
                    case "bdrip":
                    case "blu-ray":
                        baseHit.preference += 2;
                        break;

                    case "8bit": // 8 bit is normal, so nothing special there
                    default:
                        break;
                }
            }

            // Basically, try to parse the hit with all different methods. If any of these returns a usable result, it's good enough
            var funcs = new Func<string, PossibleDownloadHit, List<Episode>, PossibleDownloadHit>[] { 
                IsBDVolumeBatch,
                IsComplete,
                IsSpecial,
                IsRange,
                AfterDash,
                AfterSpace,
                BruteForce
            };

            foreach (var func in funcs)
            {
                var pHit = func(cleanName, baseHit, epInfo);
                if (pHit != null && pHit.episodes.Count > 0)
                {
                    Log.WriteLine("Found result \"{0}\", episodes:({2}) with method: {1}", pHit.name, func.Method.Name, pHit.episodes.Implode(", "));
                    switch (func.Method.Name)
                    {
                        case "IsBDVolumeBatch":
                            pHit.preference += 3;
                            break;
                        case "IsComplete":
                            pHit.preference += 2;
                            break;
                        case "IsSpecial":
                            pHit.preference++;
                            break;
                        default:
                            break;
                    }

                    hit = pHit;
                    break;
                }
            }

            if (hit == null)
            {
                // Nothing has been found. At all.
                Log.WriteLine("Could not resolve title " + title + " to anything");
            }
            else
            {
                SortByEpisodeNumber(hit);
            }

            return hit;
        }

        private static void SortByEpisodeNumber(PossibleDownloadHit hit)
        {
            hit.episodes = hit.episodes.OrderBy(p => p.seasonNumber * 1000 + p.episodeNumber).ToList();
        }

        private static Episode FindEp(List<Episode> epInfo, string epName)
        {
            return epInfo.Where(p => p.EpisodeName.ToLower() == epName).FirstOrDefault();
        }

        private static Episode FindEp(List<Episode> epInfo, int epNr)
        {
            return epInfo.Where(p => p.EpisodeNumber == epNr).FirstOrDefault();
        }

        private List<PossibleDownloadHit> getListOfAllDownloadsForName(string name, List<Episode> knownEpisodes)
        {
            var list = new List<PossibleDownloadHit>();

            string rssUrl = string.Format("http://www.nyaa.eu/?page=rss&cats=1_37&term={0}", name);
            Log.WriteLine("Getting contents of url: " + rssUrl + " ...");
            string rss = ScraperUtility.GetContentOfUrl(rssUrl);
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            Log.WriteLine("Parsing XML...");
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

            list = Nyaa.OrderByEpCountThenPreference(list);

            return list;
        }

        private static List<PossibleDownloadHit> OrderByEpCountThenPreference(List<PossibleDownloadHit> list)
        {
            list = list.OrderByDescending(p => p.episodes.Count).ThenByDescending(p => p.preference).ToList();
            return list;
        }

        public List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(Serie serie, List<SeasonEpisode> episodes, List<Episode> epInfo)
        {
            throw new NotImplementedException();
        }
    }
}
