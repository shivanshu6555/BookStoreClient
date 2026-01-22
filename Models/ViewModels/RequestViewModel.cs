using BookStoreClient.DTOs;

namespace BookStoreClient.Models.ViewModels
{
    public class RequestViewModel
    {
        public CreateBooksDto Books { get; set; }
        public CreateAuthorDto Author { get; set; }
    }
}
