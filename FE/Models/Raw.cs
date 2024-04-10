using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace FE.Models
{
    public partial class Raw
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string PublishedDate { get; set; }
        public string TopImg { get; set; }
        public string Content { get; set; }
    }
}
