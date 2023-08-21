using System;
using System.Collections.Generic;

namespace BookStory.Models
{
    public partial class StoriesCategory
    {
        public int Scid { get; set; }
        public int Sid { get; set; }
        public int Cid { get; set; }

        public virtual Category CidNavigation { get; set; } = null!;
        public virtual Story SidNavigation { get; set; } = null!;
    }
}
