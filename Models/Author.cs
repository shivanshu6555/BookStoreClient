namespace BookStoreClient.Models
{
    public class Author
    {

        public int Id { get; set; }
        public int BookId { get; set; }
        public string Name { get; set; }
        public List<Books> Books { get; set; }
    }
}
