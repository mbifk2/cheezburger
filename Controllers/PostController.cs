using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CheezAPI.Models;
using static CheezAPI.Dtos;

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

            var thread = await _context.Fthreads.FindAsync(ThreadID);
            if (thread is null) 
            {
                return NotFound("Thread not found.");
            }

            var posts = await _context.Posts.Where(p => p.FthreadID == ThreadID).ToListAsync();

            return Ok(posts.Select(p => new PostGetDto
            {
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UserID = p.UserID
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

            var thread = await _context.Fthreads.FindAsync(ThreadID);

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
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserID = post.UserID
            });
        }

        //POST: api/v1/topics/{TopicID}/threads/{ThreadID}/posts 201 Created
        // created by specific user
        [HttpPost]
        public async Task<ActionResult<PostGetDto>> PostPost(int TopicID, int ThreadID, PostCreateDto postCreateDto)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Fthreads.FindAsync(ThreadID);

            if (thread is null)
            {
                return NotFound("Thread not found.");
            }

            var post = new Post
            {
                Content = postCreateDto.Content,
                CreatedAt = DateTime.Now,
                UserID = postCreateDto.UserID,
                FthreadID = ThreadID
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { TopicID, ThreadID, id = post.PostID }, new PostGetDto
            {
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserID = post.UserID
            });
        }

        //PUT: api/v1/topics/{TopicID}/threads/{ThreadID}/posts/{id} 200 OK
        [HttpPut("{id}")]
        public async Task UpdatePost(int TopicID, int ThreadID, int id, PostUpdateDto postUpdateDto)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                NotFound("Topic not found.");
            }

            var thread = await _context.Fthreads.FindAsync(ThreadID);

            if (thread is null)
            {
                NotFound("Thread not found.");
            }

            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                NotFound("Post not found.");
            }

            if (postUpdateDto.Content != null)
            {
                post.Content = postUpdateDto.Content;
            }

            await _context.SaveChangesAsync();
        }

        //DELETE: api/v1/topics/{TopicID}/threads/{ThreadID}/posts/{id} 204 No Content
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int TopicID, int ThreadID, int id)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic is null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Fthreads.FindAsync(ThreadID);

            if (thread is null)
            {
                return NotFound("Thread not found.");
            }

            var post = await _context.Posts.FindAsync(id);

            if (post is null)
            {
                return NotFound("Post not found.");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
