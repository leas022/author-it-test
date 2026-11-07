namespace AuthorIt.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string Body { get; set; } = "";
        public List<string> Tags { get; set; } = [];
        public DateTime Created { get; init; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;
        public int Version { get; set; } = 1;

    }
}
