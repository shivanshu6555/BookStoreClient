using BookStoreClient.DTOs;
using BookStoreClient.Models;
using BookStoreClient.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static BookStoreClient.Models.ViewModels.LoginViewModel;

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


        [HttpPost]
        public async Task<IActionResult> UserLogin()
        {
            var UserModel = new LoginViewModel { Username = "Shivanshu", Password = "Rodeo1" };
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7033/api/Auth/Login", UserModel);

            if (response == null || !response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View("~/Views/Auth/Login.cshtml");
            }

            var token = await response.Content.ReadAsStringAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, UserModel.Username),
                new Claim("AccessToken", token)
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookies", principal);

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult AddBooks()
        {
            return View();
        }

        [Route("Auth/Login")]
        public async Task<ActionResult> Login()
        {
            await UserLogin();
            return View("~/Views/Auth/Login.cshtml");
        }

        [Route("Auth/Register")]
        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }
        public async Task<IActionResult> AddBooksForm(Books model)
        {
            var book = new Books { Title = model.Title, Price = model.Price, StockQuantity = model.StockQuantity, Category = model.Category };
            List<Books> books = new List<Books>();
            books.Add(book);
            var Bookrequest = new CreateAuthorDto { name = model.Author.Name, Books = books };

            var response = await _httpClient.PostAsJsonAsync("https://localhost:7033/api/BooksApi", Bookrequest);
            if (!response.IsSuccessStatusCode)
                return null;

            return RedirectToAction("AddBooks");
        }

        public async Task<IActionResult> Books(string SearchTerm, int Page = 1, string OrderBy = "title_asc")
        {
            
            SearchTerm = string.IsNullOrEmpty(SearchTerm) ? "" : SearchTerm.ToLower();
            int pageSize = 5;
            if (SearchTerm != string.Empty)
            {
                Page = 1;
                pageSize = 100;
            }
            var response = await _httpClient.GetAsync($"https://localhost:7033/api/BooksAPi?page={Page}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
                return View(new List<Author>());
            BooksViewModel viewModel = new BooksViewModel();
            var json = await response.Content.ReadAsStringAsync();
            var ResponseData = JsonSerializer.Deserialize<PagedBooksResponseDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var books = ResponseData.Data.ToList();
            var book = books.Where(a => a.Title.ToLower().StartsWith(SearchTerm) || a.Author.ToLower().StartsWith(SearchTerm)).ToList();
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
