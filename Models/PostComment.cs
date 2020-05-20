using System;
using System.Collections.Generic;

namespace BlogPostWebApi.Models
{
    public partial class PostComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int PostId { get; set; }

        //public virtual Post Post { get; set; }
    }
}
