using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DerpScrapper.DBO;
using DerpScrapper.Scraper;
using HtmlAgilityPack;

namespace DerpScrapper.DownloadSite_Scrapers
{
    class BakaBT : IDownloadSite
    {
        bool loggedIn = false;
        CookieContainer cookieContainer = new CookieContainer();

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

        public List<PossibleDownloadHit> GetDownloadsForEntireSerie(Serie serie, List<Episode> epInfo)
        {
            if (!this.loggedIn && !this.login())
                return null;

            string requestUrl = string.Format("http://bakabt.me/browse.php?only=0&hentai=0&incomplete=1&lossless=1&hd=1&multiaudio=1&bonus=1&c1=1&c2=1&reorder=1&q={0}", serie["Name"]);
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
            retVal.AddRange(getTorrentsFromTable(torrentsTable));

            foreach (var pageNode in pager.Descendants("a"))
            {
                if (pageNode.InnerText.Contains("Prev") || pageNode.InnerText.Contains("Next") || (pageNode.Attributes["class"].Value != null && pageNode.Attributes["class"].Value == "selected"))
                    continue;

                Uri uri = new Uri(new Uri("http://bakabt.me"), pageNode.Attributes["href"].Value);
                var pageDoc = ScraperUtility.HTMLDocumentOfContentFromURL(uri.AbsolutePath, cookieContainer, null, true);
                var torrentsPageTable = doc.DocumentNode.Descendants("table").Where(p => p.HasAttributes && p.Attributes["class"].Value != null && p.Attributes["class"].Value == "torrents").FirstOrDefault();
                if(torrentsPageTable != null)
                    retVal.AddRange(getTorrentsFromTable(torrentsPageTable));
            }

            return retVal;
        }

        private List<PossibleDownloadHit> getTorrentsFromTable(HtmlNode tableNode)
        {
            List<PossibleDownloadHit> retVal = new List<PossibleDownloadHit>();


            return retVal;
        }

        public List<PossibleDownloadHit> GetDownloadsForSerieWithEpisodes(DBO.Serie serie, List<SeasonEpisode> episodes, List<Episode> epInfo)
        {
            // Only use BakaBT for entire series..
            return null;
        }
    }
}
