using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CheezAPI.Dtos;
using static CheezAPI.Models;

namespace CheezAPI.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly CheezContext _context;
        private readonly IPasswordService _pws;
        private readonly ILogger<UserController> _logger;

        public UserController(CheezContext context, IPasswordService pws, ILogger<UserController> logger)
        {
            _context = context;
            _pws = pws;
            _logger = logger;
        }

        // GET: api/v1/users 200 OK
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return Ok(users.Select(u => new UserDto
            {
                UserID = u.UserID,
                Username = u.Username,
                CreatedAt = u.CreatedAt
            }));
        }

        // GET: api/v1/users/{id} 200 OK
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                CreatedAt = user.CreatedAt
            });
        }

        // POST: api/v1/users 201 Created
        [HttpPost]
        public async Task<ActionResult<UserDto>> PostUser(UserCreateDto userCreateDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userCreateDto.Username || u.Email == userCreateDto.Email))
            {
                return BadRequest("Username and/or email already used.");
            }

            if (string.IsNullOrEmpty(userCreateDto.Password) || string.IsNullOrEmpty(userCreateDto.Username) || string.IsNullOrEmpty(userCreateDto.Email))
            {
                return BadRequest("One or more required fields are empty.");
            }


            var hashedPassword = _pws.HashPassword(userCreateDto.Password);

            var user = new User
            {
                Username = userCreateDto.Username,
                Email = userCreateDto.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.Now,
                IsOnline = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                CreatedAt = user.CreatedAt,

            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, userDto);
        }

        // PUT: api/v1/users/{id} 204 No Content
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            // Find the user by ID
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(userUpdateDto.Username)) user.Username = userUpdateDto.Username;
            if (!string.IsNullOrEmpty(userUpdateDto.Email)) user.Email = userUpdateDto.Email;
            if (userUpdateDto.IsBanned.HasValue) user.IsBanned = userUpdateDto.IsBanned.Value;
            if (userUpdateDto.IsAdmin.HasValue) user.IsAdmin = userUpdateDto.IsAdmin.Value;

            var hashedPassword = "";
            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                hashedPassword = _pws.HashPassword(userUpdateDto.Password);
                user.PasswordHash = hashedPassword;
            }

            // Save changes
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/v1/users/{id} 204 No Content
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
