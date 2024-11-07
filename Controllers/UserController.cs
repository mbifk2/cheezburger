using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
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
        public async Task<ActionResult<UserDto>> CreateUser(UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
            {
                return BadRequest("wtf");
            }

            if (await _context.Users.AnyAsync(u => u.Username == userCreateDto.Username || u.Email == userCreateDto.Email))
            {
                return Conflict("Username and/or email already used.");
            }

            if (string.IsNullOrEmpty(userCreateDto.Password) || string.IsNullOrEmpty(userCreateDto.Username) || string.IsNullOrEmpty(userCreateDto.Email))
            {
                return BadRequest("One or more required fields are empty.");
            }

            if (!string.IsNullOrEmpty(userCreateDto.Email) && !Regex.IsMatch(userCreateDto.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                return UnprocessableEntity("Invalid email format.");
            }

            if (userCreateDto.Username.Length > 32)
            {
                return UnprocessableEntity("Username must be at most 32 characters long.");
            }

            if (userCreateDto.Password.Length < 8)
            {
                return UnprocessableEntity("Password must be at least 8 characters long.");
            }

            if (userCreateDto.Password.Length > 64)
            {
                return UnprocessableEntity("Password must be at most 64 characters long.");
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
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (await _context.Users.AnyAsync(u => u.Username == userUpdateDto.Username || u.Email == userUpdateDto.Email))
            {
                return Conflict("Username and/or email already used.");
            }

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
