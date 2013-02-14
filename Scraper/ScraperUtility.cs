using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace DerpScrapper.Scraper
{
        class ScraperUtility
        {
            public static string GetContentOfUrl(string url)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
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
                return contents;
            }

            public static HtmlAgilityPack.HtmlDocument HTMLDocumentOfContentFromURL(string requestURL, CookieContainer cookies = null, NetworkCredential cred = null, bool fakeAgent = false)
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestURL);
                if (cred != null)
                    req.Credentials = cred;
                if (cookies != null)
                    req.CookieContainer = cookies;

                if(fakeAgent)
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

            public static string CleanUpName(string input, IEnumerable<Tuple<char,char>> removeCharCombos)
            {
                input = input.ToLower();

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
                        d[i, j] = System.Math.Min(System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                                  d[i - 1, j - 1] + cost);
                    }
                }

                // Step 7
                return d[n, m];
            }
        
    }
}
