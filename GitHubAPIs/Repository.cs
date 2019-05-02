using System.Net;
using System.Net.Http;
using System.Text;

namespace GitHubAPIs
{
    public class Repository : GitHubService
    {
        public Repository(IGitHubConfiguration con) : base(con)
        {
        }

        public HttpResponseMessage CreateRepository(string json)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            using (var client = GetHttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");
                var request = new HttpRequestMessage(method, "/user/repos") { Content = jsonContent };
                res = client.SendAsync(request).Result;
            }
            return res;
        }

        public HttpResponseMessage ImportRepository(string json, string owner, string repository)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            using (var client = GetHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github.barred-rock-preview"));
                client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
                var method = new HttpMethod("PUT");

                var request = new HttpRequestMessage(method, $"repos/{owner}/{repository}/import") { Content = jsonContent };
                res = client.SendAsync(request).Result;
            }
            return res;
        }

        public HttpResponseMessage ForkRepo(string repoName, string owner)
        {
            HttpResponseMessage res = new HttpResponseMessage();
            using (var client = GetHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", _configuration.userName);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                var jsonContent = new StringContent("", Encoding.UTF8, "application/json");
                var method = new HttpMethod("POST");
                //repos/octocat/Hello-World/forks
                var request = new HttpRequestMessage(method, $"repos/{repoName}/forks") { Content = jsonContent };
                res = client.SendAsync(request).Result;
            }
            return res;
        }
    }
}
