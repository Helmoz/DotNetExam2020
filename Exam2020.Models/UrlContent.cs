using System;

namespace Exam2020.Models
{
    public class UrlContent
    {
        public long Id { get; set; }

        public string Url { get; set; }

        public string Body { get; set; }

        public UrlContent(string url, string body)
        {
            Url = url;
            Body = body;
        }

    }
}
