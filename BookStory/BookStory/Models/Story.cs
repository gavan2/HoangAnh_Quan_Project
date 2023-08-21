using System;
using System.Collections.Generic;

namespace BookStory.Models
{
    public partial class Story
    {
        public Story()
        {
            Chapters = new HashSet<Chapter>();
            StoriesAuthors = new HashSet<StoriesAuthor>();
            StoriesCategories = new HashSet<StoriesCategory>();
        }

        public int Sid { get; set; }
        public string? Name { get; set; }
        public int? View { get; set; }
        public int? Status { get; set; }
        public string? Source { get; set; }
        public string? Image { get; set; }
        public string? Keyword { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual ICollection<StoriesAuthor> StoriesAuthors { get; set; }
        public virtual ICollection<StoriesCategory> StoriesCategories { get; set; }
    }
}
