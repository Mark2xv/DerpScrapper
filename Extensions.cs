﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DerpScrapper
{
    public static class Extensions
    {
        public static int ToUnixTimestamp(this DateTime dt)
        {
            return (int)(dt - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        public static DateTime ToDateTime(this int timestamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(timestamp).ToLocalTime();
            return dtDateTime;
        }

        public static bool ContainsBoth(this string subject, Tuple<char, char> charCombo) 
        {
            if (subject.Contains(charCombo.Item1.ToString()) && subject.Contains(charCombo.Item2.ToString()))
            {
                int idx1 = subject.IndexOf(charCombo.Item1);
                int idx2 = subject.IndexOf(charCombo.Item2);
                if(idx1 < idx2)
                    return true;
            }
            return false;
        }
        
        public static bool ContainsAll(this IEnumerable<string> subject, IEnumerable<string> parts) 
        {
            foreach (string part in parts)
            {
                if (!subject.Contains(part))
                    return false;
            } 
            
            return true;
        }

        public static bool Matches(this string subject, System.Text.RegularExpressions.Regex reg)
        {
            return reg.IsMatch(subject);
        }

        public static HtmlAgilityPack.HtmlNode GetNodeWithTypeAndClass(this HtmlAgilityPack.HtmlDocument doc, string type, string classname) {
            return doc.DocumentNode.Descendants(type).Where(p => p.HasAttributes && p.Attributes["class"] != null && p.Attributes["class"].Value != null && p.Attributes["class"].Value == classname).FirstOrDefault();
        }
    }

}