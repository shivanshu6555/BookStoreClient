using BookStoreClient.Models;

namespace BookStoreClient.DTOs
{
    public record struct CreateBooksDto(string title, string Authorname, double price, int stockq, BookCategory category);

    public record struct CreateAuthorDto(string name, List<Books> Books);
}
