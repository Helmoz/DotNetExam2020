using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Exam2020.DAL;
using Exam2020.Models;
using System.Net;
using System.Threading;

namespace Exam2020.Services
{
    public class ScanService
    {
        protected ApplicationDbContext Context { get; set; }

        public ScanService(ApplicationDbContext context)
        {
            Context = context;
        }

        public List<UrlContent> ScanUrl(string url, int depth, int amount)
        {
            var mainList = Context.UrlContents.ToList();
            var t = new Thread(() => GetLinksByUrl(mainList, new UrlContent(url, ""), amount));
            t.Start();
            var t2 = new Thread(() => GetChildLinks(mainList, depth, amount));
            t2.Start();
            t.Join();
            t2.Join();

            return mainList;
        }

        private void GetChildLinks(List<UrlContent> links, int depth, int amount)
        {
            if (depth <= 0)
            {
                return;
            }

            var localList = new List<UrlContent>();

            foreach (var link in links)
            {
                GetLinksByUrl(localList, link, amount);
                GetChildLinks(localList, depth - 1, amount);
            }

            links.AddRange(localList);
        }

        private void GetLinksByUrl(List<UrlContent> mainList,  UrlContent urlContent, int amount)
        {
            var html = GetHtml(urlContent.Url);
            var host = urlContent.Url.GetHost();

            var nodes = html?.DocumentNode?.SelectNodes("//a[@href]");

            if (nodes != null)
            {
                mainList.AddRange(html.DocumentNode.SelectNodes("//a[@href]")
                    .Where(x => x.GetHref().GetHost() == host
                                && !mainList.Select(content => content.Url).Contains(x.GetHref()))
                    .Distinct()
                    .Take(amount)
                    .Select(x => new UrlContent(x.GetHref(), html.DocumentNode.InnerText.Trim()))
                    .ToList());
            }
        }

        private HtmlDocument GetHtml(string url)
        {
            HtmlDocument html = new HtmlDocument();
            using (var client = new WebClient())
            {
                if (!url.IsValidUrl())
                {
                    return null;
                }
                var res = client.DownloadString(url);
                if (res != null)
                {
                    html.LoadHtml(res);
                }
                
            }

            return html;
        }
    }
}
