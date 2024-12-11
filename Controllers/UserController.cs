using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        private readonly IConfiguration _config;

        public UserController(CheezContext context, IPasswordService pws, ILogger<UserController> logger, IConfiguration config)
        {
            _context = context;
            _pws = pws;
            _logger = logger;
            _config = config;
        }

        private (string, string) GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var accessToken = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            var refreshToken = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            var encodedRefreshToken = new JwtSecurityTokenHandler().WriteToken(refreshToken);
            Response.Cookies.Append("refresh_token", encodedRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddDays(7)
            });
            return (new JwtSecurityTokenHandler().WriteToken(accessToken), encodedRefreshToken);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null) return Unauthorized("User doesn't exist.");
            if (!_pws.VerifyPassword(user.PasswordHash, loginDto.Password)) return Unauthorized("Invalid password.");

            string access_token, refresh_token;
            (access_token, refresh_token) = GenerateToken(user);
            return Ok(new { access_token, refresh_token });
        }

        [Authorize]
        [HttpPost("refresh")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized("Refresh token not found.");

            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var principal = handler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = key,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"]
                }, out SecurityToken validatedToken);

                var accessToken = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: principal.Claims,
                    expires: DateTime.Now.AddMinutes(15),
                    signingCredentials: credentials
                );

                var encodedAccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken);

                return Ok(new { access_token = encodedAccessToken });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Unauthorized("Invalid or expired token.");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return Ok();
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(new { userId });
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
                PFP_URL = u.PFP_URL,
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
                PFP_URL = user.PFP_URL,
                CreatedAt = user.CreatedAt
            });
        }

        // POST: api/v1/users 201 Created
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (userCreateDto is null)
            {
                return BadRequest("Unknown error");
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
                PFP_URL = "https://files.catbox.moe/vcx6ms.jpg",
                IsOnline = true,
                IsVerified = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                PFP_URL = user.PFP_URL,
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserID }, userDto);
        }

        // PUT: api/v1/users/{id} 204 No Content
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            var user = await _context.Users.FindAsync(id);
            var loggedIn = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var isAdmin = User.IsInRole("Admin");

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!isAdmin && user.UserID != loggedIn)
            {
                return Forbid();
            }

            if (await _context.Users.AnyAsync(u => u.Username == userUpdateDto.Username && u.UserID != user.UserID || u.Email == userUpdateDto.Email && u.UserID != user.UserID))
            {
                return Conflict("Username and/or email already used.");
            }

            if (!string.IsNullOrEmpty(userUpdateDto.Username) || user.Username == userUpdateDto.Username) user.Username = userUpdateDto.Username;
            if (!string.IsNullOrEmpty(userUpdateDto.Email) || user.Email == userUpdateDto.Email) user.Email = userUpdateDto.Email;
            if (!string.IsNullOrEmpty(userUpdateDto.PFP_URL) || user.PFP_URL == userUpdateDto.PFP_URL) user.PFP_URL = userUpdateDto.PFP_URL;

            if (!string.IsNullOrEmpty(userUpdateDto.Email) && !Regex.IsMatch(userUpdateDto.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                return UnprocessableEntity("Invalid email format.");
            }

            // validate the URL of the profile picture
            if (!string.IsNullOrEmpty(userUpdateDto.PFP_URL) && !Uri.TryCreate(userUpdateDto.PFP_URL, UriKind.Absolute, out _))
            {
                return UnprocessableEntity("Invalid URL format.");
            }

            if (User.IsInRole("Admin"))
            {
                if (userUpdateDto.IsBanned.HasValue) user.IsBanned = userUpdateDto.IsBanned.Value;
                if (userUpdateDto.IsAdmin.HasValue) user.IsAdmin = userUpdateDto.IsAdmin.Value;
                if (userUpdateDto.IsVerified.HasValue) user.IsVerified = userUpdateDto.IsVerified.Value;
            }
            else
            {
                if (userUpdateDto.IsBanned.HasValue || userUpdateDto.IsAdmin.HasValue || userUpdateDto.IsVerified.HasValue)
                {
                    return Forbid("You are not allowed to change these fields.");
                }
            }

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
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && loggedInUserId != id)
            {
                return Forbid();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
