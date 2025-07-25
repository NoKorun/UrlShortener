using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Models.DBContext dbContext;
        public UsersController(Models.DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("Registeration")]
        public IActionResult Registeration(Models.UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var objUser = dbContext.Users.FirstOrDefault(u => u.UserName == userDto.UserName);
            if (objUser != null)
            {
                return BadRequest("User already exists");
            }
            else
            {
                dbContext.Users.Add(new Models.User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName = userDto.UserName,
                    Password = userDto.Password
                });
                dbContext.SaveChanges();
                return Ok("User registered successfully");
            }
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(Models.LoginDto loginDto)
        {
            var objUser = dbContext.Users.FirstOrDefault(u => u.UserName == loginDto.UserName && u.Password == loginDto.Password);

            if (loginDto == null)
            {
                return BadRequest("User data is null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (objUser != null)
            {
                return Ok("Login successful");
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }
        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = dbContext.Users.ToList();
            if (users == null)
            {
                return NotFound("No users found");
            }
            return Ok(users);
        }
        [HttpGet]
        [Route("GetUser")]
        public IActionResult GetUser(int userId)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
    }
}