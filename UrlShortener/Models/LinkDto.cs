namespace UrlShortener.Models
{
    public class LinkDto
    { 
        public string Url { get; set; }

        public string ShortUrl { get; set; }

        public string Token { get; set; }

        public DateTime DateOfCreation { get; set; }

        public int Clicks { get; set; }

        public string Creator { get; set; }
    }
}
