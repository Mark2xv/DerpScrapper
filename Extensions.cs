using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

        public static SQLiteCommand CreateCommand(this SQLiteConnection connection, string query)
        {
            var com = connection.CreateCommand();
            com.CommandText = query;
            return com;
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

        public static bool ContainsAny(this string subject, IEnumerable<string> checkAgainst)
        {
            foreach (string part in checkAgainst)
            {
                if (subject.Contains(part))
                    return true;
            }
            return false;
        }

        public static bool Matches(this string subject, System.Text.RegularExpressions.Regex reg)
        {
            return reg.IsMatch(subject);
        }

        public static HtmlAgilityPack.HtmlNode GetNodeWithTypeAndClass(this HtmlAgilityPack.HtmlDocument doc, string type, string classname) {
            return doc.DocumentNode.Descendants(type).Where(p => p.HasAttributes && p.Attributes["class"] != null && p.Attributes["class"].Value != null && p.Attributes["class"].Value == classname).FirstOrDefault();
        }

        public static string Implode(this IEnumerable<object> list, string glue)
        {
            string s = "";
            foreach (var ob in list)
            {
                s += ob.ToString() + glue;
            }
            return s.TrimEnd(glue.ToArray());
        }

        /* Language picker requirements */
        public static Dictionary<string, string> Languages()
        {
            Dictionary<string, string> languages = new Dictionary<string, string>();
            languages.Add("nl", "Dutch");
            languages.Add("en", "English");
            languages.Add("sp", "Spanish");
            languages.Add("jp", "Japanese");
            languages.Add("de", "German");
            return languages;
        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        /* End of requirements */

        /// <summary>
        /// Centers the control both horizontially and vertically 
        /// according to the parent control that contains it.
        /// </summary>
        /// <param name="control"></param>
        public static void Center(this Control control)
        {
            control.CenterHorizontally();
            control.CenterVertically();
        }

        /// <summary>
        /// Centers the control horizontially according 
        /// to the parent control that contains it.
        /// </summary>
        public static void CenterHorizontally(this Control control)
        {
            Rectangle parentRect = control.Parent.ClientRectangle;
            control.Left = (parentRect.Width - control.Width) / 2;
        }

        /// <summary>
        /// Centers the control vertically according 
        /// to the parent control that contains it.
        /// </summary>
        public static void CenterVertically(this Control control)
        {
            Rectangle parentRect = control.Parent.ClientRectangle;
            control.Top = (parentRect.Height - control.Height) / 2;
        }
    }
}
