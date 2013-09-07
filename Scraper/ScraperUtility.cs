using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DerpScrapper.Scraper
{
    static class ScraperUtility
    {
        private static List<Tuple<char, char>> removeCharCombos = new List<Tuple<char, char>>(new[] { 
            new Tuple<char,char>('[', ']'),
            new Tuple<char,char>('(', ')'),
            new Tuple<char,char>('{', '}'),
            new Tuple<char,char>('<', '>')
        });

        public static async Task<string> GetContentOfUrl(string url)
        {
            var client = new WebClient();
            string text = await client.DownloadStringTaskAsync(url);
            return text;
        }

        public static HtmlAgilityPack.HtmlDocument HTMLDocumentOfContentFromURL(string requestURL, CookieContainer cookies = null, NetworkCredential cred = null, bool fakeAgent = false)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestURL);
            if (cred != null)
                req.Credentials = cred;
            if (cookies != null)
                req.CookieContainer = cookies;

            if (fakeAgent)
                req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.57 Safari/537.17";

            string contents = "";
            using (var response = req.GetResponse())
            {
                using (var content = response.GetResponseStream())
                {
                    using (var reader = new System.IO.StreamReader(content))
                    {
                        contents = reader.ReadToEnd();
                    }
                }
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(contents);

            return doc;
        }

        public static string PartAfterEquals(string input)
        {
            int idxOf = input.IndexOf("=") + 1;
            return input.Substring(idxOf);
        }

        public static string CleanUpName(string input, bool lower = true)
        {
            if (lower)
            {
                input = input.ToLower();
            }

            // Remove all stuff between brackets "( )" "[ ]" "< >" "{ }"
            foreach (var charCombo in removeCharCombos)
            {
                while (input.ContainsBoth(charCombo))
                {
                    int idx1 = input.IndexOf(charCombo.Item1);
                    int idx2 = input.IndexOf(charCombo.Item2) + 1;

                    string start = input.Substring(0, idx1);
                    string end = input.Substring(idx2);

                    input = (start + end).Trim();
                }
            }
            // Trim it, cause often there are left over spaces after above step
            return input.Trim();
        }

        public static int Levenshtein(string s, string t)
        {
            int n = s.Length; //length of s
            int m = t.Length; //length of t

            int[,] d = new int[n + 1, m + 1]; // matrix
            int cost; // cost
            // Step 1

            if (n == 0) return m;
            if (m == 0) return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1);

                    // Step 6
                    d[i, j] = System.Math.Min(
                               System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                               d[i - 1, j - 1] + cost
                            );
                }
            }

            // Step 7
            return d[n, m];
        }

        public static Regex alnumRegex = new Regex(@"[^\w\s]");
        public static List<string> TagContents(string input)
        {
            List<string> tags = new List<string>();

            // Get all stuff between brackets "( )" "[ ]" "< >" "{ }"
            foreach (var charCombo in removeCharCombos)
            {
                while (input.ContainsBoth(charCombo))
                {
                    int idx1 = input.IndexOf(charCombo.Item1);
                    int idx2 = input.IndexOf(charCombo.Item2);

                    string tagContents = input.Substring(idx1 + 1, (idx2 - idx1) - 1);
                    tagContents = alnumRegex.Replace(tagContents, " ");
                    var parts = tagContents.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    tags.AddRange(parts);

                    string start = input.Substring(0, idx1);
                    string end = input.Substring(idx2 + 1);

                    input = (start + end).Trim();
                }
            }

            return tags;
        }

        public static bool DoesContain(this List<PossibleDownloadHit> subject, SeasonEpisode item)
        {
            foreach (var hit in subject)
            {
                if (hit.episodes.DoesContain(item))
                    return true;
            }
            return false;
        }

        public static bool DoesContain(this List<SeasonEpisode> subject, SeasonEpisode item)
        {
            foreach (var episode in subject)
            {
                if (episode.seasonNumber == item.seasonNumber && episode.episodeNumber == item.episodeNumber)
                    return true;
            }
            return false;
        }

        public static PossibleDownloadHit FindBySeasonEpisode(this List<PossibleDownloadHit> subject, SeasonEpisode item)
        {
            foreach (var hit in subject)
            {
                var sHit = hit.episodes.FindBySeasonEpisode(item);
                if (sHit != null)
                {
                    return hit;
                }
            }
            return null;
        }

        public static SeasonEpisode FindBySeasonEpisode(this List<SeasonEpisode> subject, SeasonEpisode item)
        {
            foreach (var sEp in subject)
            {
                if (item.seasonNumber == sEp.seasonNumber && item.episodeNumber == sEp.episodeNumber)
                    return sEp;
            }
            return null;
        }
    }
}
