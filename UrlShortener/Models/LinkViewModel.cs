namespace UrlShortener.Models
{
    public class LinkViewModel
    {
        public LinkDto NewLink { get; set; } = new LinkDto(); // Empty object for the form
        public List<Link> ExistingLinks { get; set; } = new List<Link>(); // List of links for the dashboard
    }
}
