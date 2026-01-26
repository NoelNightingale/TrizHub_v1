#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace TCR.Lib.Utility
{
    public static class StringUtils
    {
        public static string FirstWords(this string input, int numberWords, bool appendEllips = true)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;


            if (input.WordCount() <= numberWords)
                return input;

            // Number of words we still want to display.
            var words = numberWords;
            // Loop through entire summary.
            for (var i = 0; i < input.Length; i++)
            {
                // Increment words on a space.
                if (input[i] == ' ')
                {
                    words--;
                }
                // If we have no more words to display, return the substring.
                if (words == 0)
                {
                    if (appendEllips)
                        return input.Substring(0, i) + "...";
                    return input.Substring(0, i) + "...";
                }
            }
            return string.Empty;
        }

        public static int WordCount(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;

            return str.Split(new[] {' ', '.', '?'}, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static List<string> FindAllUrls(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return new List<string>();

            var result = new List<string>();
            var regx =
                new Regex(
                    "http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?",
                    RegexOptions.IgnoreCase);
            var mactches = regx.Matches(str);
            foreach (Match match in mactches)
            {
                if (!result.Contains(match.Value))
                    result.Add(match.Value);
            }
            return result;
        }

        public static string ToHtmlParagraphs(this string input, bool splitWithBrs = false)
        {

            if (string.IsNullOrWhiteSpace(input))
                return input;

            var result = "";
            foreach (var s in input.Split("\n".ToCharArray()))
            {
                if (splitWithBrs)
                {
                    if (!string.IsNullOrWhiteSpace(result))
                        result = result + "<br />" + s;
                    else
                        result = s;
                }
                else
                {
                    result = result + "<p>" + s + "</p>";
                }
            }
            return result;
        }

        public static bool IsValidURL(this string input, bool validateOnline = true)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            Uri uriResult;
            var result = Uri.TryCreate(input, UriKind.Absolute, out uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return validateOnline ? IsUrlActive(input) : result;
        }

        public static bool IsUrlActive(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var result = false;
            try
            {
                var request = WebRequest.Create(input) as HttpWebRequest;
                request.Method = "HEAD";
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    result = response.StatusCode == HttpStatusCode.OK;
                    response.Close();
                }
                return result;
            }
            catch
            {
                return false;
            }
        }

        public static string StripPunctuation(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            var sb = new StringBuilder();
            var ignorechars = "~`!@#$%^&*()_+{}:|\\<>?,./";
            foreach (var c in s)
            {
                if (!char.IsPunctuation(c) && (ignorechars.IndexOf(c) < 0))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public static bool IsNumeric(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            double retNum;
            var isNum = double.TryParse(Convert.ToString(s), NumberStyles.Any, NumberFormatInfo.InvariantInfo,
                out retNum);
            return isNum;
        }

        public static string TcrInteger(this int v)
        {
            return v.ToString("###,###,##0", CultureInfo.InvariantCulture);
        }
    }
}