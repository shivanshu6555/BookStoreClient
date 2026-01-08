using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookStoreClient.Models
{
    public enum BookCategory
    {
        Fiction,
        NonFiction,
        Science,
        Technology,
        Biography,
        History,
        Children
    }
    public class Books
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AuthorID { get; set; }
        [JsonIgnore]
        public Author Author { get; set; } 
        public Double Price { get; set; }
        public int StockQuantity { get; set; }
        public BookCategory Category { get; set; }
    }
}
