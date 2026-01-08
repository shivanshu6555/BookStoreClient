using BookStoreManagmentSystem.DTO_s;

namespace BookStoreClient.Models
{
    public class BooksViewModel
    {
        public List<BookResponseDto> BooksResponse { get; set; }
        public string TitleOrderBy { get; set; }
        public string AuthorOrderBy { get; set; }
        public string PriceSort { get; set; }
        public string StockSort { get; set; }
        public string CategoryOrderBy { get; set; }
    }
}
