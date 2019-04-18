namespace GitHubClientApp.Models
{
    public class AccessDetails
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
    public class UserDetail
    {
        public string login { get; set; }
    }
}