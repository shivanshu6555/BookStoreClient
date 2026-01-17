using BookStoreClient.DTOs;
using BookStoreClient.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
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

        public async Task<IActionResult> Books(string SearchTerm, int Page = 1, string OrderBy = "title_asc")
        {
            
            SearchTerm = string.IsNullOrEmpty(SearchTerm) ? "" : SearchTerm.ToLower();
            var response = await _httpClient.GetAsync($"https://localhost:7033/api/BooksAPi?page={Page}&pageSize=5");
            if (!response.IsSuccessStatusCode)
                return View(new List<Author>());
            BooksViewModel viewModel = new BooksViewModel();
            var json = await response.Content.ReadAsStringAsync();
            var ResponseData = JsonSerializer.Deserialize<PagedBooksResponseDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var books = ResponseData.Data.ToList();
            var book = books.Where(a => a.Title.StartsWith(SearchTerm) || a.Author.StartsWith(SearchTerm)).ToList();
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
                    CurrentPage = ResponseData.Pagination.CurrentPage,
                    TotalPages = ResponseData.Pagination.TotalPages
                };
            }
            viewModel.TitleOrderBy = OrderBy == "title_asc" ? "title_desc" : "title_asc";
            viewModel.AuthorOrderBy = OrderBy == "author_asc" ? "author_desc" : "author_asc";
            viewModel.PriceSort = OrderBy == "price_asc" ? "price_desc" : "price_asc";
            viewModel.StockSort = OrderBy == "stock_asc" ? "stock_desc" : "stock_asc";
            viewModel.CategoryOrderBy = OrderBy == "category_asc" ? "category_desc" : "category_asc";
            viewModel.TotalPages = ResponseData.Pagination.TotalPages;

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
