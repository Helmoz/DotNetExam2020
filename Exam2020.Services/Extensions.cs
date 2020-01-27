using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Exam2020.Services
{
    public static class StringExtensions
    {
        public static string GetHost(this string url)
        {
            if (!url.IsValidUrl())
            {
                return "";
            }

            return new Uri(url).Host;
        }

        public static bool IsValidUrl(this string url)
        {
            var pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            var rgx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return rgx.IsMatch(url);
        }
    }

    public static class HtmlNodeExtensions
    {
        public static string GetHref(this HtmlNode htmlNode)
        {
            return htmlNode.Attributes["href"].Value;
        }
    }
}
