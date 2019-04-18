namespace GitHubAPIs
{
    public class GitHubConfiguration : IGitHubConfiguration
    {
        public string token
        {
            get;
            set;
        }

        public string scheme
        {
            get;
            set;
        }

        public string mediaType
        {
            get;
            set;
        }

        public string baseAddress
        {
            get;
            set;
        }
        public string userName
        {
            get;
            set;
        }
    }
}
