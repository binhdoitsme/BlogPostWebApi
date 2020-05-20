using System;
using System.Collections.Generic;

namespace BlogPostWebApi.Models
{
    public partial class Post
    {
        public Post()
        {
            Comments = new HashSet<PostComment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public virtual ICollection<PostComment> Comments { get; set; }
    }
}
