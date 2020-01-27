using System;
using System.Collections.Generic;
using System.Text;

namespace Exam2020.DAL
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
