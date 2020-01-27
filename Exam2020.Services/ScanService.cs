using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Exam2020.DAL;
using Exam2020.Models;

namespace Exam2020.Services
{
    public class ScanService
    {
        protected ApplicationDbContext Context { get; set; }

        public ScanService(ApplicationDbContext context)
        {
            Context = context;
        }


        public async Task<List<UrlContent>> ScanUrl(string url, int depth, int amount)
        {
            var added = Context.UrlContents.ToList();
            var rootLinks = await GetLinksByUrl(added, new UrlContent(url, ""), amount);

            var result = await GetChildLinks(rootLinks, depth, amount);

            return result;
        }

        private async Task<List<UrlContent>> GetChildLinks(List<UrlContent> links, int depth, int amount)
        {
            if (depth <= 0)
            {
                return links;
            }

            var localLinks = new List<UrlContent>();

            foreach (var link in links)
            {
                var childLinks = await GetLinksByUrl(links, link, amount);
                localLinks.AddRange(await GetChildLinks(childLinks, depth - 1, amount));
            }

            return localLinks;
        }

        private async Task<List<UrlContent>> GetLinksByUrl(IReadOnlyCollection<UrlContent> addedLink, UrlContent urlContent, int amount)
        {
            var html = await GetHtmlAsync(urlContent.Url);
            var host = urlContent.Url.GetHost();

            var nodes = html?.DocumentNode?.SelectNodes("//a[@href]");

            if (nodes != null)
            {
                return html.DocumentNode.SelectNodes("//a[@href]")
                    .Where(x => x.GetHref().GetHost() == host
                                && !addedLink.Select(content => content.Url).Contains(x.GetHref()))
                    .Distinct()
                    .Take(amount)
                    .Select(x => new UrlContent(x.GetHref(), html.DocumentNode.InnerText.Trim()))
                    .ToList();
            }

            return new List<UrlContent>();
        }

        private async Task<HtmlDocument> GetHtmlAsync(string url)
        {
            HtmlDocument html = new HtmlDocument();
            using (HttpClient client = new HttpClient())
            {
                if (!url.IsValidUrl())
                {
                    return null;
                }
                using (HttpResponseMessage res = await client.GetAsync(url))
                using (HttpContent content = res.Content)
                {
                    string data = await content.ReadAsStringAsync();
                    if (data != null)
                    {
                        html.LoadHtml(data);
                    }
                }
            }

            return html;
        }
    }
}
