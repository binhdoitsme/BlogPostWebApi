using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogPostWebApi.Models;
using Microsoft.AspNetCore.Http.Extensions;
using BlogPostWebApi.ModelWrappers;

namespace BlogPostWebApi.Controllers
{
    //[Route("/posts/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private const int MAX_COUNT_PER_PAGE = 10;
        private readonly IWSTrainingContext _context;

        public PostController(IWSTrainingContext context)
        {
            _context = context;
        }

        // GET: api/Post
        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts([FromQuery] int page)
        {
            int currentPage = page <= 0 ? 1 : page;
            int offset = (currentPage - 1) * MAX_COUNT_PER_PAGE;
            string baseUrl = GetRequestedUrl();
            string httpString = HttpContext.Request.IsHttps ? "https://" : "http://";
            IList<Post> result = await _context.Posts
                                        .Include(p => p.Comments)
                                        .Skip(offset).Take(MAX_COUNT_PER_PAGE)
                                        .ToListAsync();
            IList<PostWrapper> data = result.Select(post => 
                                        new PostWrapper(post,
                                            $"{httpString}{HttpContext.Request.Host}/posts/{post.Id}"))
                                        .ToList();
            int totalCount = await _context.Posts.CountAsync();
            int pageCount = totalCount / MAX_COUNT_PER_PAGE + (totalCount % MAX_COUNT_PER_PAGE != 0 ? 1 : 0);
            var pagination = new
            {
                firstPage = $"{baseUrl}?page=1",
                lastPage = $"{baseUrl}?page={pageCount}",
                currentPage
            };
            return Ok(new
            {
                data,
                totalCount,
                pagination
            });
        }

        // GET: api/Post/5
        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPostById(int id)
        {
            var post = await _context.Posts.Include(p => p.Comments).FirstAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(new PostWrapper(post, GetRequestedUrl()));
        }

        // PUT: api/Post/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPatch("posts/{id}")]
        public async Task<IActionResult> UpdateOnePost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new PostWrapper(_context.Posts.Where(p => p.Id == id).FirstOrDefault(), GetRequestedUrl()));
        }

        // POST: api/Post
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("posts")]
        public async Task<IActionResult> CreateOnePost(Post postToAdd)
        {
            _context.Posts.Add(postToAdd);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostById",
                new { id = postToAdd.Id },
                new PostWrapper(postToAdd, $"{GetRequestedUrl()}/{postToAdd.Id}"));
        }

        // DELETE: api/Post/5
        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePosts(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                deletedId = id
            });
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        private string GetRequestedUrl()
        {
            return HttpContext?.Request?.GetDisplayUrl();
        }
    }
}
