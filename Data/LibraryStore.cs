using AuthorIt.Models;

namespace AuthorIt.Data
{
    public class LibraryStore
    {
        public Dictionary<int, Topic> Topics { get; set; } = [];
        private int _lastTopicId = 0;

        public Dictionary<int, Book> Books { get; set; } = [];
        private int _lastBookId = 0;

        private int _lastReferenceId = 0;

        public LibraryStore()
        {
            Seed();
        }


        private void Seed()
        {
            AddTopic("Introduction to Author-it", "An overview of the Author-it platform and its features.", ["overview", "getting-started"]);
            AddTopic("Installing the Desktop Client", "Instructions for installing the Author-it desktop client.", ["installation", "windows", "desktop"]);
            AddTopic("Connecting to a Library", "How to connect to a library in Author-it.", ["setup", "library", "configuration"]);
            AddTopic("Creating Your First Topic", "A guide to creating your first topic in Author-it.", ["authoring", "topics", "beginner"]);
            AddTopic("Publishing to PDF", "How to publish your content to PDF in Author-it.", ["publishing", "output", "pdf"]);

            AddBook("Getting Started Guide", [
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 1, Level = 1 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 2, Level = 1 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 3, Level = 2 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 4, Level = 1 }
            ]);
            AddBook(" Publishing Handbook", [
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 1, Level = 1 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 5, Level = 1 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 4, Level = 2 }
            ]);
            AddBook("Administrator Guide", [
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 2, Level = 1 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 3, Level = 2 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 5, Level = 1 },
                new Reference { ReferenceId = GetNextReferenceId(), TopicId = 3, Level = 2 }
            ]);
        }

        private void AddTopic(string title, string body, List<string> tags)
        {
            var id = GetNextTopicId();
            Topics.Add(id, new Topic
            {
                Id = id,
                Title = title,
                Body = body,
                Tags = tags,
            });
        }

        private void AddBook(string title, List<Reference> contents)
        {
            var id = GetNextBookId();
            Books.Add(id, new Book
            {
                Id = id,
                Title = title,
                Contents = contents
            });
        }

        public int GetNextTopicId()
        {
            return Interlocked.Increment(ref _lastTopicId);
        }

        public int GetNextBookId()
        {
            return Interlocked.Increment(ref _lastBookId);
        }

        public int GetNextReferenceId()
        {
            return Interlocked.Increment(ref _lastReferenceId);
        }

    }
}
