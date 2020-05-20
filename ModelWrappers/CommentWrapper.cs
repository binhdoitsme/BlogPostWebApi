using BlogPostWebApi.Models;

namespace BlogPostWebApi.ModelWrappers
{
    public class CommentWrapper
    {
        public PostComment Comment { get; private set; }
        public string Link { get; private set; }

        public CommentWrapper(PostComment comment, string link)
        {
            Comment = comment;
            Link = link;
        }
    }
}
