using HashidsNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    public class LinkWebController : Controller
    {
        private readonly DBContext dbContext;

        public LinkWebController(DBContext context)
        {
            dbContext = context;
        }
        private string GetCurrentUserName()
        {
            return User.Identity.Name;
        }

        // GET: LinkWeb/Create
        public IActionResult Create()
        {
            var currentUser = GetCurrentUserName();
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public IActionResult Dashboard()
        {
            var currentUser = GetCurrentUserName();
            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToAction("Login", "Account");
            }

            var userLinks = dbContext.Links
                .Where(l => l.Creator == currentUser)
                /*.Select(l => new
                {
                    l.Url,
                    l.ShortUrl,
                    l.Clicks,
                    l.DateOfCreation
                })*/
                .ToList();

            return View(userLinks);
        }

        [HttpPost]
        public IActionResult Create(LinkDto link)
        {
            var currentUser = GetCurrentUserName();
            if (string.IsNullOrEmpty(currentUser))
            {
                return Unauthorized("User not logged in");
            }
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

                return RedirectToAction("Dashboard");
            }
            return BadRequest("Invalid data");
        }
    }
}
