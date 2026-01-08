using BookStoreManagmentSystem.DTO_s;

namespace BookStoreClient.Models
{
    public class BooksViewModel
    {
        public List<BookResponseDto> BooksResponse { get; set; }
        public string NameOrderBy { get; set; }
        public string EmailOrderBy { get; set; }
    }
}
