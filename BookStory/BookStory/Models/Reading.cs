using System;
using System.Collections.Generic;

namespace BookStory.Models
{
    public partial class Reading
    {
        public int Rid { get; set; }
        public int? Uid { get; set; }
        public int? Ctid { get; set; }

        public virtual Chapter? Ct { get; set; }
        public virtual User? UidNavigation { get; set; }
    }
}
