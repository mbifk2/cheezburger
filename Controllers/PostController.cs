using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CheezAPI.Models;
using static CheezAPI.Dtos;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CheezAPI.Controllers
{
    [ApiController]
    [Route("api/v1/topics/{TopicID}/threads/{ThreadID}/posts")]
    public class PostController : ControllerBase
    {
        private readonly CheezContext _context;
        private readonly ILogger<PostController> _logger;

        public PostController(CheezContext context, ILogger<PostController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/v1/topics/{TopicID}/threads/{ThreadID}/posts 200 OK
        [HttpGet]
        public async Task<ActionResult<PostGetDto>> GetPosts(int TopicID, int ThreadID)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(ThreadID);
            if (thread is null) 
            {
                return NotFound("Thread not found.");
            }

            var posts = await _context.Posts.Where(p => p.FthreadID == ThreadID).ToListAsync();

            return Ok(posts.Select(p => new PostGetDto
            {
                PostID = p.PostID,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                CreatorID = p.CreatorID,
            }));
        }

        //GET: api/v1/topics/{TopicID}/threads/{ThreadID}/posts/{id} 200 OK
        [HttpGet("{id}")]
        public async Task<ActionResult<PostGetDto>> GetPost(int TopicID, int ThreadID, int id)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(ThreadID);

            if (thread is null)
            {
                return NotFound("Thread not found.");
            }

            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return NotFound("Post not found.");
            }

            return Ok(new PostGetDto
            {
                PostID = post.PostID,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                CreatorID = post.CreatorID 
            });
        }

        //POST: api/v1/topics/{TopicID}/threads/{ThreadID}/posts 201 Created
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PostGetDto>> CreatePost(int TopicID, int ThreadID, [FromBody] PostCreateDto postCreateDto)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(ThreadID);

            if (thread is null)
            {
                return NotFound("Thread not found.");
            }

            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            topic.CreatedAt = DateTime.Now;
            thread.CreatedAt = DateTime.Now;

            var post = new Post
            {
                Content = postCreateDto.Content,
                CreatedAt = DateTime.Now,
                CreatorID = loggedIn,
                FthreadID = ThreadID
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { TopicID, ThreadID, id = post.PostID }, new PostGetDto
            {
                PostID = post.PostID,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                CreatorID = post.CreatorID
            });
        }

        //PUT: api/v1/topics/{TopicID}/threads/{ThreadID}/posts/{id} 204 No Content
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int TopicID, int ThreadID, int id, PostUpdateDto postUpdateDto)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(ThreadID);

            if (thread is null)
            {
                return NotFound("Thread not found.");
            }

            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return NotFound("Post not found.");
            }

            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && post.CreatorID != loggedIn)
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(postUpdateDto.Content))
            {
                post.Content = postUpdateDto.Content;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: api/v1/topics/{TopicID}/threads/{ThreadID}/posts/{id} 204 No Content
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int TopicID, int ThreadID, int id)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(ThreadID);

            if (thread is null)
            {
                return NotFound("Thread not found.");
            }

            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return NotFound("Post not found.");
            }

            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && post.CreatorID != loggedIn)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
