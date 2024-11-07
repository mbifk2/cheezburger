using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CheezAPI.Models;
using static CheezAPI.Dtos;


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
                Title = f.Title,
                CreatedAt = f.CreatedAt,
                IsLocked = f.IsLocked
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
                Title = thread.Title,
                CreatedAt = thread.CreatedAt,
                IsLocked = thread.IsLocked
            });
        }

        //POST: api/v1/topics/{TopicID}/threads 201 Created
        [HttpPost]
        public async Task<ActionResult<ThreadDto>> PostThread(int TopicID, ThreadCreateDto threadCreateDto)
        {
            var thread = new Fthread
            {
                Title = threadCreateDto.Title,
                CreatedAt = DateTime.Now,
                TopicID = TopicID
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
                Title = thread.Title,
                CreatedAt = thread.CreatedAt,
                IsLocked = thread.IsLocked
            });
        }

        //PUT: api/v1/topics/{TopicID}/threads/{id} 204 No Content
        [HttpPut("{id}")]
        public async Task<ActionResult<ThreadDto>> UpdateThread(int TopicID, int id, ThreadUpdateDto fthreadUpdateDto)
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

            if (await _context.Threads.AnyAsync(f => f.Title == fthreadUpdateDto.Title))
            {
                return Conflict("Thread title already used.");
            }

            if (fthreadUpdateDto.Title != null)
            {
                thread.Title = fthreadUpdateDto.Title;
            }

            if (fthreadUpdateDto.IsLocked != null)
            {
                thread.IsLocked = (bool)fthreadUpdateDto.IsLocked;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: api/v1/topics/{TopicID}/threads/{id} 204 No Content
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

            _context.Threads.Remove(thread);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
