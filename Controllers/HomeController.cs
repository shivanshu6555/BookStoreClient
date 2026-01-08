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

        public async Task<IActionResult> Books(string SearchTerm, string OrderBy = "title_asc")
        {
            SearchTerm = string.IsNullOrEmpty(SearchTerm) ? "" : SearchTerm.ToLower();
            var response = await _httpClient.GetAsync("https://localhost:7033/api/BooksAPi");
            if (!response.IsSuccessStatusCode)
                return View(new List<Author>());
            BooksViewModel viewModel = new BooksViewModel();
            var json = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<BookResponseDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            List<BookResponseDto> book = books.Where(a => a.Title.ToLower().StartsWith(SearchTerm) || a.Author.ToLower().StartsWith(SearchTerm)).ToList();

            if (SearchTerm != "")
            {

                viewModel = new BooksViewModel()
                {
                    BooksResponse = book,
                };
            }
            else
            {
                viewModel = new BooksViewModel()
                {
                    BooksResponse = books,
                };
            }
            viewModel.TitleOrderBy = OrderBy == "title_asc" ? "title_desc" : "title_asc";
            viewModel.AuthorOrderBy = OrderBy == "author_asc" ? "author_desc" : "author_asc";
            viewModel.PriceSort = OrderBy == "price_asc" ? "price_desc" : "price_asc";
            viewModel.StockSort = OrderBy == "stock_asc" ? "stock_desc" : "stock_asc";
            viewModel.CategoryOrderBy = OrderBy == "category_asc" ? "category_desc" : "category_asc";

           

            switch (OrderBy)
            {
                case "title_asc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderBy(a => a.Title).ToList();
                    break;
                case "title_desc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderByDescending(a => a.Title).ToList();
                    break;
                case "author_asc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderBy(a => a.Author).ToList();
                    break;
                case "author_desc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderByDescending(a => a.Title).ToList();
                    break;
                case "price_asc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderBy(a => a.Price).ToList();
                    break;
                case "price_desc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderByDescending(a => a.Price).ToList();
                    break;
                case "stock_asc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderBy(a => a.StockQuantity).ToList();
                    break;
                case "stock_desc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderByDescending(a => a.StockQuantity).ToList();
                    break;
                case "category_asc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderBy(a => a.Category).ToList();
                    break;
                case "category_desc":
                    viewModel.BooksResponse = viewModel.BooksResponse.OrderByDescending(a => a.Category).ToList();
                    break;
            }
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
    }
}
