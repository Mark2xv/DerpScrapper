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

        private char[] splitBySpace = new[] { ' ' };

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
        private Regex isENDFile = new Regex(@"\d+.end", RegexOptions.IgnoreCase);

        private List<int> episodesFound = new List<int>();

        public List<PossibleDownloadHit> GetDownloadsForEntireSerie(SerieInfo forSerie)
        {
            var serie = forSerie.Serie;
            var knownEpisodes = forSerie.Episodes;

            Log.WriteLine("\n\n--------------------NEW RUN----------------------\n\n");

            var list = this.getListOfAllDownloadsForName(serie["Name"].ToString(), knownEpisodes);
            var key = 1;
            foreach (var x in list)
            {
                Log.WriteLine(key + " (p="+x.preference+") | " + x.name + " = ({0}) | cEp = " + x.episodes.Count + "\n\tUrl = " + x.infoPageUrl + "\n\t" + x.episodes.Implode(",\n\t"), x.qualityInfo);
                key++;
            }


            Console.WriteLine("--------LISTING------------");
            var grouped = list.GroupBy(p => p.qualityInfo.PreferenceBonus).OrderByDescending(p=>p.Key);
            foreach (var x in grouped)
            {
                Console.WriteLine("List: PrefIndex = " + x.Key);
                foreach (var y in x)
                {
                    Console.WriteLine(y.name + " | Q = " + y.qualityInfo);
                }
                Console.WriteLine("-----------------");
            }

            Console.WriteLine("\n------------------Getting hits--------------\n");

            List<PossibleDownloadHit> downloads = new List<PossibleDownloadHit>();
            foreach (Episode searchEpisode in knownEpisodes)
            {
                PossibleDownloadHit hit = null;

                foreach (var prefGroup in grouped)
                {
                    foreach (var dlHit in prefGroup)
                    {
                        if (dlHit.episodes.Where(p => p.episodeNumber == searchEpisode.EpisodeNumber && p.seasonNumber == searchEpisode.SeasonNumber).FirstOrDefault() != null)
                        {
                            hit = dlHit;
                            break;
                        }
                    }

                    if (hit != null)
                        break;
                }

                if (hit != null)
                {
                    if (!downloads.Contains(hit))
                    {
                        downloads.Add(hit);
                    }
                    else
                    {
                        // Not adding as this specific download had already been added
                    }
                }
                else
                {
                    Console.WriteLine("Could not find anything for Ep " + searchEpisode.EpisodeNumber);
                }
            }

            return downloads;
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

                string clTitle = ScraperUtility.CleanUpName(title).Replace("_", " ");

                // First check whether the file is in a folder, if so strip it off
                // Keep taking the 'right' part
                while (clTitle.Contains('/'))
                {
                    int idxOfSlash = clTitle.IndexOf('/');
                    clTitle = (clTitle.Substring(idxOfSlash+1)).Trim();
                }

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

                        Match m = versionCheck.Match(clTitle);
                        if(m.Success)
                        {
                            clTitle = clTitle.Substring(0, m.Index);
                        }



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
                                string _tmpName = onlyAlphaNumeric.Replace(clTitle, "");
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
                baseHit.qualityInfo.source = QualityInformation.Source.BlueRay;
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
                        string splitSpecialEpName = string.Format("{0} {1}", firstPart, epVal);
                        var specialEpisodeMatch = FindEp(episodeInfo, splitSpecialEpName);
                        if (specialEpisodeMatch != null)
                        {
                            // actually found it!
                            Log.WriteLine("Matched {0} to special episode {1}", baseHit.name, specialEpisodeMatch.EpisodeName);
                            baseHit.episodes.Add(new SeasonEpisode(-1, epVal, baseHit.name));
                        }
                        else
                        {
                            // nope. Could happen if all previous requirements hit, but for example name = {serieName} - {episodeName} - 1v2 - SPECIAL
                            if (firstPart.EndsWith("v"))
                                firstPart = firstPart.TrimEnd('v');

                            int epNr;
                            int idxOfSpace = firstPart.LastIndexOf(' ');
                            if (idxOfSpace != -1 && int.TryParse(firstPart.Substring(idxOfSpace), out epNr)) 
                            {
                                specialEpisodeMatch = FindEp(episodeInfo, -1, epNr);
                                if (specialEpisodeMatch != null)
                                    baseHit.episodes.Add(new SeasonEpisode(-1, epNr, baseHit.name));
                            }
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
                if (epNrPart.Contains(" end"))
                    epNrPart = epNrPart.Substring(0, epNrPart.IndexOf(' '));
                int epNr;
                bool success = int.TryParse(epNrPart, out epNr);
                if (success && FindEp(episodeInfo, epNr) != null) // If parsing succeeded and episode number exists
                {
                    baseHit.episodes.Add(new SeasonEpisode(1, epNr, baseHit.name));
                    return baseHit;
                }
                else
                {
                    // Check with vX test
                    var match = versionCheck.Match(epNrPart);
                    if (!match.Success)
                        return null;

                    epNrPart = epNrPart.Substring(0, match.Index);
                    success = int.TryParse(epNrPart, out epNr);
                    if (success && FindEp(episodeInfo, epNr) != null)
                    {
                        baseHit.episodes.Add(new SeasonEpisode(1, epNr, baseHit.name));
                        return baseHit;
                    }
                    else
                    {
                        string _tmpName = onlyAlphaNumeric.Replace(parts.Item1, "");
                    }
                }
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
            string cleanName = ScraperUtility.CleanUpName(name);

            QualityInformation qual = new QualityInformation();

            List<string> tags = ScraperUtility.TagContents(name);
            foreach (string tag in tags)
            {
                switch (tag.ToLower())
                {
                    // Containers
                    case "mkv":
                        baseHit.preference++;
                        break;
                    case "mp4":
                        baseHit.preference--;
                        break;

                    // Encoding Bit size
                    case "hi10":
                    case "hi10p":
                    case "10bit":
                        qual.encoding = QualityInformation.Encoding.TenBit;
                        baseHit.preference++;
                        break;

                    case "8bit":
                        qual.encoding = QualityInformation.Encoding.EightBit;
                        break;

                    // Resolution
                    case "1080p":
                    case "1920x1080":
                        qual.resolution = QualityInformation.Resolution.HD1080;
                        baseHit.preference += 2;
                        break;

                    case "720p":
                    case "1280x720":
                        qual.resolution = QualityInformation.Resolution.HD720;
                        baseHit.preference++;
                        break;

                    case "480p":
                        qual.resolution = QualityInformation.Resolution.Unknown_Lower;
                        break;

                    // Source
                    case "bd":
                    case "bluray":
                    case "bdrip":
                    case "blu-ray":
                        qual.source = QualityInformation.Source.BlueRay;
                        baseHit.preference += 2;
                        break;

                    case "dvd":
                    case "dvdrip":
                        qual.source = QualityInformation.Source.DVD;
                        break;

                    case "hdtv":
                        qual.source = QualityInformation.Source.HDTV;
                        break;

                    default:
                        qual.encoding = QualityInformation.Encoding.EightBit;
                        break;
                }
            }

            baseHit.qualityInfo = qual;

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

            string hitFunc = "";
            foreach (var func in funcs)
            {
                var pHit = func(cleanName, baseHit, epInfo);
                if (pHit != null && pHit.episodes.Count > 0)
                {
                    hitFunc = func.Method.Name;
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
                return null;
            }
            hit.preference += hit.qualityInfo.PreferenceBonus;

            Log.WriteLine("Found result \"{0}\", episodes:({2}) with method: {1} with qualInfo: {3}", hit.name, hitFunc, hit.episodes.Implode(", "), qual);
            SortByEpisodeNumber(hit);

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

        private static Episode FindEp(List<Episode> epInfo, int seasonNr, int epNr)
        {
            return epInfo.Where(p => p.EpisodeNumber == epNr && p.SeasonNumber == seasonNr).FirstOrDefault();
        }

        private static Episode FindEp(List<Episode> epInfo, int epNr)
        {
            return epInfo.Where(p => p.EpisodeNumber == epNr).FirstOrDefault();
        }

        private List<PossibleDownloadHit> getListOfAllDownloadsForName(string name, List<Episode> knownEpisodes)
        {
            var list = new List<PossibleDownloadHit>();

            int cOffset = 1;
            while (true)
            {
                string rssUrl = string.Format("http://www.nyaa.eu/?page=rss&cats=1_37&term={0}&offset={1}", name, cOffset);
                Log.WriteLine("Getting contents of url: " + rssUrl + " ...");
                
                
                var task = ScraperUtility.GetContentOfUrl(rssUrl);
                task.Wait();
                var rss = task.Result;

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                Log.WriteLine("Parsing XML...");
                doc.LoadXml(rss);
                int itemsThisPage = 0;
                for (int i = 0; i < doc.DocumentElement.FirstChild.ChildNodes.Count; i++)
                {
                    XmlNode node = doc.DocumentElement.FirstChild.ChildNodes[i];
                    // Skip first 5 items - they are descriptors of the RSS Feed
                    if (node.Name != "item") continue;
                    itemsThisPage++;

                    PossibleDownloadHit hit = this.ParseItem(node, name, knownEpisodes);

                    if (hit == null)
                        continue;

                    bool mustAbide = DownloadSettings.MustAbide;
                    QualityInformation prefferedQuality = DownloadSettings.PrefferedDownloadQuality;
                    if (mustAbide)
                    {
                        if (!(hit.qualityInfo.source == prefferedQuality.source && hit.qualityInfo.resolution == prefferedQuality.resolution && hit.qualityInfo.encoding == prefferedQuality.encoding))
                        {
                            Console.WriteLine("Dropped download: {0} - did not abide to set rules for downloads. (rules = {1}, hitQ = {2})", hit.name, prefferedQuality, hit.qualityInfo);
                            continue;
                        }
                    }
                    else
                    {
                        // Just prefer the ones who do abide to the preffered quality (as an extra above the default rules / pref system)
                        if (hit.qualityInfo.encoding == prefferedQuality.encoding)
                            hit.preference += 5;

                        if (hit.qualityInfo.resolution == prefferedQuality.resolution)
                            hit.preference += 5;

                        if (hit.qualityInfo.source == prefferedQuality.source)
                            hit.preference += 5;
                    }

                    if (hit != null)
                        list.Add(hit);
                }

                if (itemsThisPage == 0)
                    break;

                cOffset++;
            }

            list = Nyaa.OrderPreferenceThenEpCount(list);

            return list;
        }

        private static List<PossibleDownloadHit> OrderPreferenceThenEpCount(List<PossibleDownloadHit> list)
        {
            list = list.OrderByDescending(p => p.preference).ThenByDescending(p => p.episodes.Count).ToList();
            return list;
        }

        public List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(SerieInfo forSerie, List<Episode> wantedEpisodes)
        {
            throw new NotImplementedException();
        }

        public bool SupportsPartial()
        {
            return true;
        }

        public bool SupportsFull()
        {
            return true;
        }
    }
}
