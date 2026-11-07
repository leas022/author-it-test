namespace AuthorIt.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Description { get; set; } = "";
        public DateTime Created { get; init; } = DateTime.Now;
    }
}
