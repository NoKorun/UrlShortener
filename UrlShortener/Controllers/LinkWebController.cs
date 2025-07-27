using HashidsNet;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class LinkWebController : Controller
    {
        private readonly DBContext dbContext;

        // Constructor to inject the database context
        public LinkWebController(DBContext context)
        {
            dbContext = context;
        }

        private string GetCurrentUserName()
        {
            return User.Identity.Name;
        }

        public IActionResult Dashboard()
        {
            var currentUser = GetCurrentUserName();
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new LinkViewModel
            {
                NewLink = new LinkDto(),
                ExistingLinks = dbContext.Links
                .Where(l => l.Creator == User.Identity.Name)  // Фильтр по текущему пользователю
                .OrderByDescending(l => l.DateOfCreation)
                .ToList()
            };

            return View(model);
        }

        // Redirector action to handle short links
        [HttpGet("/{token}")]
        public IActionResult Redirector(string token)
        {
            var link = dbContext.Links.FirstOrDefault(l => l.Token == token);
            if (link == null)
            {
                return NotFound("Invalid short link");
            }

            link.Clicks++;
            dbContext.SaveChanges();

            return Redirect(link.Url);
        }

        [HttpPost]
        public IActionResult CreateShortLink(LinkViewModel model)
        {
            // check if user is logged in
            var currentUser = GetCurrentUserName();
            if (string.IsNullOrEmpty(currentUser))
            {
                return Unauthorized("User not logged in");
            }
            if (ModelState.IsValid)
            {
                // Check if link already exists for the current user
                LinkDto link = model.NewLink;
                var existingLink = dbContext.Links.FirstOrDefault(l => l.Url == link.Url && l.Creator == currentUser);
                if (existingLink != null)
                {
                    TempData["Error"] = "Link already shortened";
                    return RedirectToAction("Dashboard");
                }
                // Create local link (need id to generate token)
                var newLink = new Models.Link
                {
                    Url = link.Url,
                    DateOfCreation = DateTime.Now,
                    Clicks = 0,
                    Creator = GetCurrentUserName(),
                    ShortUrl = "",
                    Token = ""
                };
                dbContext.Links.Add(newLink);
                dbContext.SaveChanges();

                // Generate a short URL
                var hashids = new Hashids("rock salt");
                var token = hashids.Encode(newLink.LinkId);
                var shorturl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{token}";
                newLink.Token = token;
                newLink.ShortUrl = shorturl;
                dbContext.SaveChanges();

                return RedirectToAction("Dashboard");
            }
            return BadRequest("Invalid data");
        }
    }
}
