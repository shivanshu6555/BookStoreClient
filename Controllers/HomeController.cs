using BookStoreClient.Models;
using BookStoreManagmentSystem.DTO_s;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text.Json;

namespace BookStoreClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Books(string SearchTerm,string OrderBy="")
        {
            SearchTerm = string.IsNullOrEmpty(SearchTerm) ? "" : SearchTerm.ToLower();
            var response = await _httpClient.GetAsync("https://localhost:7033/api/BooksAPi");
            if (!response.IsSuccessStatusCode)
                return View(new List<Author>());

            var json = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<BookResponseDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            List<BookResponseDto> book = books.Where(a => a.Title.ToLower().StartsWith(SearchTerm) || a.Author.ToLower().StartsWith(SearchTerm)).ToList();

            BooksViewModel viewModel = new BooksViewModel()
            {
                BooksResponse = books,
                NameOrderBy = "",
                EmailOrderBy = "",
            };

            viewModel.EmailOrderBy = OrderBy == "shivanshu" ? "shivanshu1" : "shivanshu";
            if (SearchTerm == "")
                return View(viewModel);
            return View(book);
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
    }
}
