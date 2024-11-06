using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CheezAPI.Models;
using static CheezAPI.Dtos;


namespace CheezAPI.Controllers
{
    [ApiController]
    [Route("api/v1/topics/{TopicID}/threads")]
    public class FthreadController : ControllerBase
    {
        private readonly CheezContext _context;
        private readonly ILogger<FthreadController> _logger;

        public FthreadController(CheezContext context, ILogger<FthreadController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/v1/topics/{TopicID]/threads 200 OK
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FthreadDto>>> GetFthreads(int TopicID)
        {
            var fthreads = await _context.Fthreads.Where(f => f.TopicID == TopicID).ToListAsync();

            return Ok(fthreads.Select(f => new FthreadDto
            {
                Title = f.Title,
                CreatedAt = f.CreatedAt,
                IsLocked = f.IsLocked
            }));
        }

        //GET: api/v1/topics/{TopicID/threads/{id} 200 OK
        [HttpGet("{id}")]
        public async Task<ActionResult<FthreadDto>> GetThread(int TopicID, int id)
        {
            var fthread = await _context.Fthreads.FindAsync(id);
            if (fthread == null)
            {
                return NotFound();
            }
            return Ok(new FthreadDto
            {
                Title = fthread.Title,
                CreatedAt = fthread.CreatedAt,
                IsLocked = fthread.IsLocked
            });
        }

        //POST: api/v1/topics/{TopicID}/threads 201 Created
        [HttpPost]
        public async Task<ActionResult<FthreadDto>> PostThread(int TopicID, FthreadCreateDto fthreadCreateDto)
        {
            var thread = new Fthread
            {
                Title = fthreadCreateDto.Title,
                CreatedAt = DateTime.Now,
                TopicID = TopicID
            };

            if (await _context.Fthreads.AnyAsync(f => f.Title == fthreadCreateDto.Title))
            {
                return BadRequest("Thread title already used.");
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

            _context.Fthreads.Add(thread);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThread), new { TopicID = TopicID, id = thread.FthreadID }, new FthreadDto
            {
                Title = thread.Title,
                CreatedAt = thread.CreatedAt,
                IsLocked = thread.IsLocked
            });
        }

        //PUT: api/v1/topics/{TopicID}/threads/{id} 200 OK
        [HttpPut("{id}")]
        public async Task<ActionResult<FthreadDto>> PutThread(int TopicID, int id, FthreadUpdateDto fthreadUpdateDto)
        {
            var thread = await _context.Fthreads.FindAsync(id);
            if (thread == null)
            {
                return NotFound();
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

            return Ok(new FthreadDto
            {
                Title = thread.Title,
                CreatedAt = thread.CreatedAt,
                IsLocked = thread.IsLocked
            });
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

            var thread = await _context.Fthreads.FindAsync(id);
            if (thread == null)
            {
                return NotFound();
            }

            _context.Fthreads.Remove(thread);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
