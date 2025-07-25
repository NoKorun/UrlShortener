using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace UrlShortener.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UsersController : ControllerBase
    {
        private readonly Models.DBContext dbContext;
        public UsersController(Models.DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("Registration")]
        public IActionResult Registration(Models.UserDto userDto)
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
                    UserName = userDto.UserName,
                    Password = userDto.Password
                });
                dbContext.SaveChanges();
                return Ok("User registered successfully");
            }
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(Models.UserDto userDto)
        {
            var objUser = dbContext.Users.FirstOrDefault(u => u.UserName == userDto.UserName && u.Password == userDto.Password);

            if (userDto == null)
            {
                return BadRequest("User data is null");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (objUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, objUser.UserName),
                    new Claim(ClaimTypes.NameIdentifier, objUser.UserId.ToString())
                };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                return Ok("Login successful");
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }
        }
        /*[HttpGet]
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
        }*/
        // Позволяет получить информацию о текущем пользователе
        /*[HttpGet]
        [Route("Me")]
        [Authorize] 
        public IActionResult Me()
        {
            var userName = User.Identity.Name;
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(userName);
            //return Ok(new { userId, userName });
        }*/
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return Ok("Logged out");
        }
    }
}