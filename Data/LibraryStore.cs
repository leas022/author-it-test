using AuthorIt.Models;

namespace AuthorIt.Data
{
    public class LibraryStore
    {
        public Dictionary<int, Topic> Topics { get; set; }
        private int _lastTopicId = 0;

        public LibraryStore()
        {
            Topics = [];
            Seed();
        }


        private void Seed()
        {
            Topics.Add(1, new Topic
            {
                Id = 1,
                Title = "Introduction to Author-it",
                Tags = ["overview", "getting-started"],
            });

            Topics.Add(2, new Topic
            {
                Id = 2,
                Title = "Installing the Desktop Client",
                Tags = ["installation", "windows", "desktop"],
            });

            Topics.Add(3, new Topic
            {
                Id = 3,
                Title = "Connecting to a Library",
                Tags = ["setup", "library", "configuration"],
            });

            Topics.Add(4, new Topic
            {
                Id = 4,
                Title = "Creating Your First Topic",
                Tags = ["authoring", "topics", "beginner"],
            });

            Topics.Add(5, new Topic
            {
                Id = 5,
                Title = "Publishing to PDF",
                Tags = ["publishing", "output", "pdf"],
            });

            _lastTopicId = 5;
        }

        public int GetNextTopicId()
        {
            return Interlocked.Increment(ref _lastTopicId);
        }

    }
}
