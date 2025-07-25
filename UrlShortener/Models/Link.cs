using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class Link
    {
        //public static List<Link> AllLinks = new List<Link>();
        //public static List<Link> UserLinks = new List<Link>();

        //public static User Authorized = null;

        public int LinkId { get; set; }

        public string Url { get; set; }

        public string ShortUrl { get; set; }

        public string Token { get; set; }

        public DateTime DateOfCreation { get; set; }

        public int Clicks { get; set; }

        public string Creator { get; set; } //Nickname (mb use UserId/User class instead)
    }
}
