using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogPostWebApi.Models;
using BlogPostWebApi.ModelWrappers;
using Microsoft.AspNetCore.Http.Extensions;

namespace BlogPostWebApi.Controllers
{
    //[Route("comments/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private const int MAX_COUNT_PER_PAGE = 10;
        private readonly IWSTrainingContext _context;

        public CommentController(IWSTrainingContext context)
        {
            _context = context;
        }

        // GET: api/Comment
        [HttpGet("/posts/{postId}/comments")]
        public async Task<IActionResult> GetCommentsForPost([FromRoute] int postId, [FromQuery] int page)
        {
            int currentPage = page <= 0 ? 1 : page;
            int offset = (currentPage - 1) * MAX_COUNT_PER_PAGE;
            string baseUrl = GetRequestedUrl();
            string httpString = HttpContext.Request.IsHttps ? "https://" : "http://";

            var data = await _context.Comments.Where(cmt => cmt.PostId == postId)
                .Skip(offset).Take(MAX_COUNT_PER_PAGE)
                .Select(cmt => new CommentWrapper(cmt, $"{httpString}{HttpContext.Request.Host}/comments/{cmt.Id}"))
                .ToListAsync();

            int totalCount = await _context.Comments.Where(cmt => cmt.PostId == postId).CountAsync();
            int pageCount = totalCount / MAX_COUNT_PER_PAGE + 1;
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

        // GET: api/Comment/5
        [HttpGet("/comments/{id}")]
        public async Task<ActionResult<CommentWrapper>> GetCommentById([FromRoute] int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return new CommentWrapper(comment, GetRequestedUrl());
        }

        // PUT: api/Comment/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPatch("/comments/{id}")]
        public async Task<IActionResult> UpdateComment(int id, PostComment comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            CommentWrapper commentWrapper = new CommentWrapper(comment, GetRequestedUrl());

            return Ok(new
            {
                commentWrapper,
                updated = true
            });
        }

        // POST: api/Comment
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("/posts/{postId}/comments")]
        public async Task<IActionResult> CreateComment([FromRoute] int postId, PostComment comment)
        {
            comment.PostId = postId;
            Console.WriteLine(comment.PostId);
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            string httpString = HttpContext.Request.IsHttps ? "https://" : "http://";
            return CreatedAtAction("GetCommentById", 
                new { id = comment.Id }, 
                new CommentWrapper(comment, $"{httpString}{HttpContext.Request.Host}/comments/{comment.Id}"));
        }

        // DELETE: api/Comment/5
        [HttpDelete("/comments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                deletedId = id
            });
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

        private string GetRequestedUrl()
        {
            return HttpContext?.Request?.GetDisplayUrl();
        }
    }
}
