using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GitHubAPIs
{
    public abstract class GitHubService
    {
        protected readonly IGitHubConfiguration _configuration;
        protected readonly string _credential;
        protected readonly string _user;
        protected readonly string _scheme;
        protected readonly string _mediaType;
        protected readonly string _baseAddress;

        protected GitHubService(IGitHubConfiguration con)
        {
            _configuration = con;
            _credential = con.token;
            _user = con.userName;
            _scheme = con.scheme;
            _mediaType = con.mediaType;
            _baseAddress = con.baseAddress;
        }

        protected HttpClient GetHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_baseAddress)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_scheme, _credential);
            return client;
        }
    }
}
