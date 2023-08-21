using System;
using System.Collections.Generic;

namespace BookStory.Models
{
    public partial class Category
    {
        public Category()
        {
            StoriesCategories = new HashSet<StoriesCategory>();
        }

        public int Cid { get; set; }
        public string Title { get; set; } = null!;
        public int? ParentId { get; set; }
        public string? Keyword { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<StoriesCategory> StoriesCategories { get; set; }
    }
}
