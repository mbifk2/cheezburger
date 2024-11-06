using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static CheezAPI.Models;
using static CheezAPI.Dtos;

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
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt
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
                Title = topic.Title,
                Description = topic.Description,
                CreatedAt = topic.CreatedAt
            });
        }

        //POST: api/v1/topics 201 Created
        [HttpPost]
        public async Task<ActionResult<TopicDto>> PostTopic(TopicCreateDto topicCreateDto)
        {
            var topic = new Topic
            {
                Title = topicCreateDto.Title,
                Description = topicCreateDto.Description,
                CreatedAt = DateTime.Now
            };
            if (await _context.Topics.AnyAsync(t => t.Title == topicCreateDto.Title))
            {
                return BadRequest("Topic title already used.");
            }

            if (string.IsNullOrEmpty(topic.Title))
            {
                return BadRequest("Topic needs a title.");
            }

            if (string.IsNullOrEmpty(topic.Description))
            {
                topic.Description = "No description provided.";
            }
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTopic), new { id = topic.TopicID }, new TopicDto
            {
                Title = topic.Title,
                Description = topic.Description,
                CreatedAt = topic.CreatedAt
            });
        }

        //PUT: api/v1/topics/{id} 204 No Content
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTopic(int id, TopicUpdateDto topicUpdateDto)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            if (topicUpdateDto.Title != null)
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic not found.");
            }

            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
