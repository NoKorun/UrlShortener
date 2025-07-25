using Microsoft.AspNetCore.Mvc;
using UrlShortener.Models;
using HashidsNet;

namespace UrlShortener.Controllers
{
    public class LinkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("ShortenLink")]
        public IActionResult ShortenLink(Models.Link link)
        {
            if (ModelState.IsValid)
            {
                // Generate a short URL
                var hashids = new Hashids("rock salt");
                //var dateHash = DateTime.GetHashCode(link.DateOfCreation);
                var token = hashids.Encode(link.LinkId);

                var shorturl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{token}";
                link.Token = token;
                link.DateOfCreation = DateTime.Now;
                link.Clicks = 0;
                //link.Creator = Link.Authorized.login;
            }

            return View();
        }
    }
}
