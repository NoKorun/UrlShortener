using HashidsNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class LinkController : ControllerBase
    {
        private readonly Models.DBContext dbContext;
        public LinkController(Models.DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("ShortenLink")]
        public IActionResult ShortenLink(Models.LinkDto link)
        {
            if (ModelState.IsValid)
            {
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
                return Ok("Link successfully shortened");
            }

            return BadRequest("Invalid data");
        }
        [HttpGet("MyLinks")]
        [Authorize]
        public IActionResult MyLinks()
        {
            var currentUser = GetCurrentUserName();
            if (string.IsNullOrEmpty(currentUser))
            {
                return Unauthorized("User not logged in");
            }

            var userLinks = dbContext.Links
                .Where(l => l.Creator == currentUser)
                .Select(l => new
                {
                    l.Url,
                    l.ShortUrl,
                    l.Clicks,
                    l.DateOfCreation
                })
                .ToList();

            return Ok(userLinks);
        }
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
        private string GetCurrentUserName()
        {
            return User.Identity.Name;
        }
    }
}
