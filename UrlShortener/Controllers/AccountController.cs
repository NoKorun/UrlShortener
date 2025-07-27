using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext dbContext;

        // Constructor to inject the database context
        public AccountController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Login() => View();
        public IActionResult Register() 
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "Linkweb");
            }
            return View();
        }
        [ActionName("Logout")]
        public IActionResult Logout_Get() => View("Logout");


        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserDto userDto)
        {
            var objUser = dbContext.Users.FirstOrDefault(u => u.UserName == userDto.UserName && u.Password == userDto.Password);
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Invalid username or password";
                return View(userDto);
            }
            if (objUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, objUser.UserName),
                    new Claim(ClaimTypes.NameIdentifier, objUser.UserId.ToString())
                };

                // Provide cookie for authentication
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);
                return RedirectToAction("Dashboard", "Linkweb");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View(userDto);
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
                ViewBag.Error = "Invalid username or password";
                return View(userDto);
            }

            // Check if the username already exists
            var existingUser = dbContext.Users.FirstOrDefault(u => u.UserName == userDto.UserName);
            if (existingUser != null)
            {
                ViewBag.Error = "Username is already taken";
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
