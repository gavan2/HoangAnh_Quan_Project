using System;
using System.Collections.Generic;

namespace BookStory.Models
{
    public partial class Author
    {
        public Author()
        {
            StoriesAuthors = new HashSet<StoriesAuthor>();
        }

        public int Aid { get; set; }
        public string? Name { get; set; }
        public string? Alias { get; set; }
        public string? Keyword { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<StoriesAuthor> StoriesAuthors { get; set; }
    }
}
