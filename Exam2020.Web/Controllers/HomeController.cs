using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Exam2020.DAL;
using Exam2020.Models;
using Exam2020.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Exam2020.Web.Models;
using HtmlAgilityPack;

namespace Exam2020.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        protected ApplicationDbContext Context { get; set; }

        protected ScanService ScanService { get; set; }

        protected SearchService SearchService { get; set; }

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, ScanService scanService, SearchService searchService)
        {
            _logger = logger;
            Context = context;
            ScanService = scanService;
            SearchService = searchService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Scan()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Scan(string url, int depth, int amount)
        {
            if (!ModelState.IsValid || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                ViewBag.Message = $"Введите корректные данные";
                return View();
            }
            var result = await ScanService.ScanUrl(url, depth, amount);

            foreach (var urlContent in result)
            {
                await Context.UrlContents.AddAsync(urlContent);
            }

            await Context.SaveChangesAsync();

            ViewBag.Message = $"Количество просканированных страниц: {result.Count}";
            return View();
        }

        public IActionResult Search()
        {
            return View(new List<UrlContent>());
        }

        [HttpPost]
        public async Task<IActionResult> Search(string domain)
        {
            var urlContent = await SearchService.Search(domain); 
            return View(urlContent);
        }


    }
}
