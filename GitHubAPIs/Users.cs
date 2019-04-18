using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAPIs
{
    public class Users : GitHubService
    {
        public Users(IGitHubConfiguration con) : base(con)
        {
        }

        public HttpResponseMessage GetUserDetail()
        {
            //https://api.github.com/user
            using (var client = GetHttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "demogenapi");
                HttpResponseMessage response = client.GetAsync("/user").Result;
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            return new HttpResponseMessage();
        }
    }
}
