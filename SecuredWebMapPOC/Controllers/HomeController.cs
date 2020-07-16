using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SecuredWebMapPOC.Models;

namespace SecuredWebMapPOC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                WebMapPortalID = _config.GetValue<string>("WebMapPortalID")
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<string> GetToken()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://www.arcgis.com/sharing/rest/oauth2/token/");

            var tokenParams = new Dictionary<string, string>
            {
                { "client_id", _config.GetValue<string>("ClientID") },
                { "client_secret", _config.GetValue<string>("ClientSecret") },
                { "grant_type", "client_credentials" },
                { "expiration", "20160" },
                { "f", "json" }
            };

            var content = new FormUrlEncodedContent(tokenParams);
            var response = await client.PostAsync("", content);
            var json = await response.Content.ReadAsStringAsync();
            var tokenData = JsonConvert.DeserializeObject<TokenData>(json);

            return tokenData.access_token;
        }

        private class TokenData
        {
            public string access_token { get; set; }
            public long expires_in { get; set; }
        }
    }
}
