using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CheezAPI.Models;
using static CheezAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace CheezAPI.Controllers
{
    [ApiController]
    [Route("api/v1/topics/{TopicID}/threads")]
    public class ThreadController : ControllerBase
    {
        private readonly CheezContext _context;
        private readonly ILogger<ThreadController> _logger;

        public ThreadController(CheezContext context, ILogger<ThreadController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/v1/topics/{TopicID]/threads 200 OK
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThreadDto>>> GetThreads(int TopicID)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }
            var fthreads = await _context.Threads.Where(f => f.TopicID == TopicID).ToListAsync();

            return Ok(fthreads.Select(f => new ThreadDto
            {
                ThreadID = f.FthreadID,
                Title = f.Title,
                CreatedAt = f.CreatedAt,
                IsLocked = f.IsLocked,
                VerifiedOnly = f.VerifiedOnly,
                CreatorId = f.CreatorID
            }));
        }

        //GET: api/v1/topics/{TopicID/threads/{id} 200 OK
        [HttpGet("{id}")]
        public async Task<ActionResult<ThreadDto>> GetThread(int TopicID, int id)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(id);
            if (thread == null)
            {
                return NotFound("Thread not found.");
            }
            return Ok(new ThreadDto
            {
                ThreadID = thread.FthreadID,
                Title = thread.Title,
                CreatedAt = thread.CreatedAt,
                IsLocked = thread.IsLocked,
                VerifiedOnly = thread.VerifiedOnly,
                CreatorId = thread.CreatorID
            });
        }

        //POST: api/v1/topics/{TopicID}/threads 201 Created
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ThreadDto>> PostThread(int TopicID, [FromBody] ThreadCreateDto threadCreateDto)
        {
            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var thread = new Fthread
            {
                Title = threadCreateDto.Title,
                CreatedAt = DateTime.Now,
                TopicID = TopicID,
                VerifiedOnly = false,
                CreatorID = loggedIn
            };

            if (await _context.Threads.AnyAsync(f => f.Title == threadCreateDto.Title))
            {
                return Conflict("Thread title already used.");
            }

            if (string.IsNullOrEmpty(thread.Title))
            {
                return BadRequest("Thread title cannot be empty.");
            }

            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            _context.Threads.Add(thread);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThread), new { TopicID = TopicID, id = thread.FthreadID }, new ThreadDto
            {
                ThreadID = thread.FthreadID,
                Title = thread.Title,
                CreatedAt = thread.CreatedAt,
                IsLocked = thread.IsLocked,
                VerifiedOnly = thread.VerifiedOnly,
                CreatorId = thread.CreatorID
            });
        }

        //PUT: api/v1/topics/{TopicID}/threads/{id} 204 No Content
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ThreadDto>> UpdateThread(int TopicID, int id, ThreadUpdateDto fthreadUpdateDto)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isAdmin = User.IsInRole("Admin");

            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(id);
            if (thread == null)
            {
                return NotFound("Thread not found");
            }

            if (await _context.Threads.AnyAsync(f => f.Title == fthreadUpdateDto.Title && f.FthreadID != thread.FthreadID))
            {
                return Conflict("Thread title already used.");
            }

            if (fthreadUpdateDto.Title != null)
            {
                thread.Title = fthreadUpdateDto.Title;
            }

            if (fthreadUpdateDto.IsLocked != null)
            {
                thread.IsLocked = fthreadUpdateDto.IsLocked.Value;
            }

            if (fthreadUpdateDto.VerifiedOnly != null)
            {
                thread.VerifiedOnly = fthreadUpdateDto.VerifiedOnly.Value;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: api/v1/topics/{TopicID}/threads/{id} 204 No Content
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThread(int TopicID, int id)
        {
            var topic = await _context.Topics.FindAsync(TopicID);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            var thread = await _context.Threads.FindAsync(id);
            if (thread == null)
            {
                return NotFound("Thread not found");
            }

            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && thread.CreatorID != loggedIn)
            {
                return Forbid();
            }

            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
