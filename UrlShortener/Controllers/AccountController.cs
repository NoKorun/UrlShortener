using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext dbContext;

        public AccountController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Register() => View();
        public IActionResult Login() => View();
        [HttpGet]
        [ActionName("Logout")]
        public IActionResult Logout_Get() => View("Logout");


        public IActionResult Debug()
        {
            ViewBag.Debug = TempData["debug"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserDto userDto)
        {
            var objUser = dbContext.Users.FirstOrDefault(u => u.UserName == userDto.UserName && u.Password == userDto.Password);
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Invalid credentials";
                return View(userDto);
            }
            if (objUser != null)
            { 
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, objUser.UserName),
                    new Claim(ClaimTypes.NameIdentifier, objUser.UserId.ToString())
                };

                // Provide cookie authentication
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                return Ok("Login successful");
                //return RedirectToAction("Dashboard", "LinkWeb");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                //return View(userDto);
                return Unauthorized($"////" +    $"Username: {userDto.UserName}" + $"////, " +$"////Password: {userDto.Password}" +$"////");
                //return Unauthorized("Invalid username or password");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Logout")]
        public async Task<IActionResult> Logout_Post()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");  
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public IActionResult Register(UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userDto);
            }

            var existingUser = dbContext.Users.FirstOrDefault(u => u.UserName == userDto.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "Username is already taken");
                return View(userDto);
            }

            var newUser = new User
            {
                UserName = userDto.UserName,
                Password = userDto.Password
            };

            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();

            return RedirectToAction("Login");
        }
    }
}
