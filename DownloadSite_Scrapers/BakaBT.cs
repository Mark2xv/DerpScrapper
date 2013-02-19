using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DerpScrapper.DBO;
using DerpScrapper.Scraper;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace DerpScrapper.DownloadSite_Scrapers
{
    class BakaBT : IDownloadSite
    {
        bool loggedIn = false;
        CookieContainer cookieContainer = new CookieContainer();
        private static Regex onlyAlphaNumeric = new Regex(@"[^\w\s]");
        private static Regex seasonCheck = new Regex(@"(season|s){1}.+(\d)+$");

        private bool login()
        {
            if (!loggedIn)
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("http://bakabt.me/login.php");
                req.CookieContainer = cookieContainer;
                req.ContentType = "application/x-www-form-urlencoded";
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.57 Safari/537.17";
                req.Method = "POST";

                string[] creds = File.ReadAllLines(Program.RootDirectory + "Cache/bakabt.txt");
                string uid = creds[0].Split('=')[1];
                string pw = creds[1].Split('=')[1];

                byte[] bytes = Encoding.ASCII.GetBytes(string.Format("username={0}&password={1}", uid, pw));
                req.ContentLength = bytes.Length;

                using (Stream os = req.GetRequestStream())
                {
                    os.Write(bytes, 0, bytes.Length);
                }

                WebResponse resp = req.GetResponse();
                string o = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                if (o.Contains("Could not connect to MySQL database: Too many connections"))
                {
                    Console.WriteLine("BakaBT down..");
                    return false;
                }

                var p = cookieContainer.GetCookies(new Uri("http://bakabt.me"));

                bool hasUID = false;
                bool hasPass = false;
                foreach (var z in p)
                {
                    Cookie c = (Cookie)z;
                    if (c.Name == "uid")
                    {
                        hasUID = true;
                    }
                    else if (c.Name == "pass")
                    {
                        hasPass = true;
                    }
                }

                loggedIn = (hasUID && hasPass);

                if (!loggedIn)
                {
                    Console.WriteLine("Logging in to bakabt failed.");
                    return false;
                }
            }

            return true;
        }

        public List<PossibleDownloadHit> GetDownloadsForEntireSerie(SerieInfo serieInfo)
        {
            if (!this.loggedIn && !this.login())
                return null;

            var serie = serieInfo.serie;

            string requestUrl = string.Format("http://bakabt.me/browse.php?only=0&hentai=0&incomplete=1&lossless=1&hd=1&multiaudio=1&bonus=1&c1=1&c2=1&c5=1&reorder=1&q={0}", serie["Name"]);
            var doc = ScraperUtility.HTMLDocumentOfContentFromURL(requestUrl, cookieContainer, null, true);

            var pager = doc.DocumentNode.Descendants("div").Where(p => p.HasAttributes && p.Attributes["class"] != null && p.Attributes["class"].Value != null && p.Attributes["class"].Value == "pager").FirstOrDefault();
            var torrentsTable = doc.DocumentNode.Descendants("table").Where(p => p.HasAttributes && p.Attributes["class"].Value != null && p.Attributes["class"].Value == "torrents").FirstOrDefault();

            List<PossibleDownloadHit> retVal = new List<PossibleDownloadHit>();

            if (torrentsTable == null)
            {
                Console.WriteLine("Nothing found on BakaBT!");
                return retVal;
            }

            // Read what's on this current page
            retVal.AddRange(getTorrentsFromTable(torrentsTable, serieInfo));

            if (pager != null)
            {
                foreach (var pageNode in pager.Descendants("a"))
                {
                    if (pageNode.InnerText.Contains("Prev") || pageNode.InnerText.Contains("Next") || (pageNode.Attributes["class"].Value != null && pageNode.Attributes["class"].Value == "selected"))
                        continue;

                    Uri uri = new Uri(new Uri("http://bakabt.me"), pageNode.Attributes["href"].Value);
                    var pageDoc = ScraperUtility.HTMLDocumentOfContentFromURL(uri.AbsolutePath, cookieContainer, null, true);
                    var torrentsPageTable = doc.DocumentNode.Descendants("table").Where(p => p.HasAttributes && p.Attributes["class"].Value != null && p.Attributes["class"].Value == "torrents").FirstOrDefault();
                    if (torrentsPageTable != null)
                        retVal.AddRange(getTorrentsFromTable(torrentsPageTable, serieInfo));
                }
            }

            return retVal;
        }

        private List<PossibleDownloadHit> getTorrentsFromTable(HtmlNode tableNode, SerieInfo forSerie)
        {
            List<string> badAlts = (forSerie.metadata.NameNonAlternatives != "") ? forSerie.metadata.NameNonAlternatives.Split('|').ToList() : new List<string>();

            List<PossibleDownloadHit> retVal = new List<PossibleDownloadHit>();
            var cookies = this.cookieContainer.GetCookies(new Uri("http://bakabt.me"));
            string uid = "";
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "uid")
                {
                    uid = cookie.Value; 
                    break;
                }
            }

            var searchHits = GetSearchHits(tableNode, uid);
            List<BakaBTSearchHit> hits_serie = new List<BakaBTSearchHit>();
            List<BakaBTSearchHit> hits_movie = new List<BakaBTSearchHit>();
            List<BakaBTSearchHit> hits_ova = new List<BakaBTSearchHit>();

            foreach (var searchHit in searchHits)
            {
                string cleanHit = ScraperUtility.CleanUpName(searchHit.name);
                if (cleanHit.ContainsAny(badAlts))
                {
                    Console.WriteLine("Hit {0} skipped - contained any of bad alt names", searchHit.name);
                    continue;
                }
                switch (searchHit.type)
                {
                    case "OVA":
                        hits_ova.Add(searchHit);
                        break;
                    case "Anime Movie":
                        hits_movie.Add(searchHit);
                        break;
                    case "Anime Series":
                        hits_serie.Add(searchHit);
                        break;
                    default:
                        break;
                }
            }

            var totalDownloads = new List<BakaBTSearchHit>();
            totalDownloads.AddRange(hits_ova);
            totalDownloads.AddRange(hits_movie);

            if (hits_serie.Count >= 2)
            {
                Console.WriteLine("Not 100% final yet. (for incompletes/neverending such as naruto shipp, pokemon, etc)");
                foreach (var serieHit in hits_serie)
                {
                    string clName = ScraperUtility.CleanUpName(serieHit.name);
                    var match = seasonCheck.Match(clName);
                    if (match.Success)
                        totalDownloads.Add(serieHit);
                    else
                        Console.WriteLine("Ditched hit " + serieHit.name + " (did not match [season|s] \\d)");
                }
            }
            else if (hits_serie.Count == 1)
            {
                totalDownloads.AddRange(hits_serie);
            }

            foreach (BakaBTSearchHit searchHit in totalDownloads)
            {
                var doc = ScraperUtility.HTMLDocumentOfContentFromURL(searchHit.url, cookieContainer, null, true);
                var node = doc.GetNodeWithTypeAndClass("a", "download_link");
                string tUrl = "http://bakabt.me" + node.GetAttributeValue("href", "");

                retVal.Add(new PossibleDownloadHit() { origSerieName = forSerie.serie.Name, name = searchHit.name, infoPageUrl = searchHit.url, url = tUrl });
            }

            return retVal;
        }

        private static List<BakaBTSearchHit> GetSearchHits(HtmlNode node, string uid)
        {
            var baseNodes = node.Descendants("tr");
            var listGroups = new List<BakaBTSearchHit>();

            BakaBTSearchHit currHit = null;

            bool firstAltHit = true;

            foreach (var searchHitNode in baseNodes)
            {
                if (searchHitNode.ParentNode.Name == "thead")
                    continue;

                if (searchHitNode.GetAttributeValue("class", "").Contains("torrent_alt"))
                {
                    if (firstAltHit)
                    {
                        firstAltHit = false;
                        continue;
                    }

                    BakaBTSearchHit alt = new BakaBTSearchHit();
                    alt.type = currHit.type;

                    var altNode = searchHitNode.ChildNodes[1].ChildNodes[1];
                    string altNodeId = altNode.Id;
                    var altParts = altNodeId.Split('_');
                    string name = altNode.InnerText;
                    string tId = altParts[1];

                    // Fill in info for alt

                    if (currHit.alternatives == null)
                        currHit.alternatives = new List<BakaBTSearchHit>();

                    alt.name = name;
                    alt.url = string.Format("http://bakabt.me/{0}-", tId);
                    currHit.alternatives.Add(alt);
                }
                else
                {
                    if (currHit != null)
                    {
                        listGroups.Add(currHit);
                        firstAltHit = true;
                        currHit = new BakaBTSearchHit();
                    }

                    currHit = new BakaBTSearchHit();
                    var xNode = searchHitNode.ChildNodes[1].ChildNodes[1];
                    string torrIdS = xNode.GetAttributeValue("class", "");
                    if (torrIdS == "")
                    {
                        Console.WriteLine("Failed to get tID from hit");
                        break;
                    }

                    var parts = torrIdS.Split('_');
                    string tid = parts[1];

                    var typeNode = searchHitNode.ChildNodes[1].ChildNodes[1].ChildNodes[1].ChildNodes[0];
                    currHit.type = typeNode.GetAttributeValue("title", "");

                    var descNode = searchHitNode.ChildNodes[3];
                    var nameNode = descNode.ChildNodes[1].ChildNodes[1];

                    var tagNode = searchHitNode.ChildNodes[3].ChildNodes[3];

                    currHit.name = nameNode.InnerText;
                    var tags = new List<string>();
                    foreach(var tag in tagNode.InnerText.Split(',')) 
                    {
                        tags.Add(tag.Trim());
                    }
                    currHit.tags = tags;

                    currHit.url = string.Format("http://bakabt.me/{0}-.html", tid);

                }
            }
            if (currHit != null)
                listGroups.Add(currHit);

            return listGroups;
        }

        public List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(SerieInfo forSerie, List<Episode> epInfo)
        {
            // Only use BakaBT for entire series..
            return null;
        }

        private class BakaBTSearchHit
        {
            public string name;
            public string url;
            public string type;
            public List<string> tags;
            public List<BakaBTSearchHit> alternatives;
        }
    }
}
