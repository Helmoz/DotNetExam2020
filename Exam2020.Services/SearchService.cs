using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exam2020.DAL;
using Exam2020.Models;

namespace Exam2020.Services
{
    public class SearchService
    {
        protected ApplicationDbContext Context { get; set; }

        public SearchService(ApplicationDbContext context)
        {
            Context = context;
        }

        public async Task<List<UrlContent>> Search(string domain)
        {
            var urlContent = Context.UrlContents.AsEnumerable()
                .Where(x => x.Url.GetHost() == domain).Distinct()
                .ToList();
            return urlContent;
        }
    }
}
