using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static CheezAPI.Models;
using static CheezAPI.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace CheezAPI.Controllers
{
    [ApiController]
    [Route("api/v1/topics")]
    public class TopicController : ControllerBase
    {
        private readonly CheezContext _context;
        private readonly ILogger<TopicController> _logger;

        public TopicController(CheezContext context, ILogger<TopicController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //GET: api/v1/topics 200 OK
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopics()
        {
            var topics = await _context.Topics.ToListAsync();

            return Ok(topics.Select(t => new TopicDto
            { 
                TopicID = t.TopicID,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                CreatorId = t.CreatorID
            }));
        }

        //GET: api/v1/topics/{id} 200 OK
        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDto>> GetTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic not found");
            }
            return Ok(new TopicDto
            {
                TopicID = topic.TopicID,
                Title = topic.Title,
                Description = topic.Description,
                CreatorId = topic.CreatorID
            });
        }

        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetLatestTopics()
        {
            var topics = await _context.Topics.OrderByDescending(t => t.CreatedAt).Take(5).ToListAsync();

            return Ok(topics.Select(t => new TopicDto
            {
                TopicID = t.TopicID,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                CreatorId = t.CreatorID
            }));
        }

        //POST: api/v1/topics 201 Created
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] TopicCreateDto topicCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var topic = new Topic
            {
                Title = topicCreateDto.Title,
                Description = topicCreateDto.Description,
                CreatorID = loggedIn,
                CreatedAt = DateTime.UtcNow
            };

            if (await _context.Topics.AnyAsync(t => t.Title == topic.Title))
            {
                return Conflict("Topic title already used.");
            }

            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTopic), new { id = topic.TopicID}, new TopicDto 
            {
                TopicID = topic.TopicID,
                Title = topic.Title,
                Description = topic.Description, 
                CreatedAt = topic.CreatedAt,
                CreatorId = topic.CreatorID 
            });
        }

        //PUT: api/v1/topics/{id} 204 No Content
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, [FromBody] TopicUpdateDto topicUpdateDto)
        {
            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isAdmin = User.IsInRole("Admin");

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            if (!isAdmin && loggedIn != topic.CreatorID)
            {
                return Forbid();
            }

            if (await _context.Topics.AnyAsync(t => t.Title == topicUpdateDto.Title && t.TopicID != topic.TopicID))
            {
                return Conflict("Topic title already used.");
            }

            if (topicUpdateDto.Title != null || topicUpdateDto.Title == topic.Title)
            {
                topic.Title = topicUpdateDto.Title;
            }

            if (topicUpdateDto.Description != null)
            {
                topic.Description = topicUpdateDto.Description;
            }

            if (topicUpdateDto.IsHidden != null)
            {
                topic.IsHidden = topicUpdateDto.IsHidden.Value;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //DELETE: api/v1/topics/{id} 204 No Content
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isAdmin = User.IsInRole("Admin");
            
            if (!isAdmin && topic.CreatorID != loggedIn)
            {
                return Forbid();
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
