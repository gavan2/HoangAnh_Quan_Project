using System;
using System.Collections.Generic;

namespace BookStory.Models
{
    public partial class User
    {
        public User()
        {
            Readings = new HashSet<Reading>();
        }

        public int Uid { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Reading> Readings { get; set; }
    }
}
