using BlogPostWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPostWebApi.ModelWrappers
{
    public class PostWrapper
    {
        public Post Post { get; private set; }
        public string Link { get; private set; }
        
        public PostWrapper(Post post, string link)
        {
            Post = post;
            Link = link;
        }
    }
}
