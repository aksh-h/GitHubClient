using GitHubAPIs;
using GitHubClientApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace GitHubClientApp.Controllers
{
    public class AccountController : Controller
    {
        private AccessDetails accessDetails = new AccessDetails();
        private delegate string[] ProcessEnvironment(string token);
        private static string CreatedRepoName = string.Empty;
        // VIEWS
        public ActionResult Callback()
        {
            // Here we get the Code in the Query String, using that we can get access token
            var request = Request;
            string code = Request.QueryString["code"];
            if (!string.IsNullOrEmpty(code))
            {
                string reqUrl = FormatRequestUrl(code);
                // Getting access token, if access token is null, will return to Index page [relogin takes place]
                AccessDetails _accessDetails = GetAccessToken(reqUrl);
                if (_accessDetails.access_token != null)
                {
                    Session["Token"] = _accessDetails.access_token;
                    ViewBag.Response = _accessDetails.access_token;
                    return RedirectToAction("process");
                }
                else
                {
                    return RedirectToAction("index", "home");
                }
            }
            return RedirectToAction("index", "home");
        }
        public ActionResult Process()
        {
            if (Session["Token"] != null)
            {
                ViewBag.Response = Session["Token"].ToString();
                return View();
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }

        // Formatting the POST URL
        public string FormatRequestUrl(string code)
        {
            string ClientID = System.Configuration.ConfigurationManager.AppSettings["ClientID"];
            string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
            string RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrl"];
            string Scope = System.Configuration.ConfigurationManager.AppSettings["Scope"];
            string state = System.Configuration.ConfigurationManager.AppSettings["State"];
            string requestUrl = string.Format("?client_id={0}&client_secret={1}&code={2}&redirect_uri={3}&state={4}", ClientID, ClientSecret, code, RedirectUrl, state);
            return requestUrl;
        }
        // Get the access token
        public AccessDetails GetAccessToken(string body)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var request = new HttpRequestMessage(HttpMethod.Post, string.Format("https://github.com/login/oauth/access_token{0}", body));
                    var response = client.SendAsync(request).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        accessDetails = JsonConvert.DeserializeObject<AccessDetails>(response.Content.ReadAsStringAsync().Result);
                        return accessDetails;
                    }
                    else
                    {
                        return new AccessDetails();
                    }
                }
            }
            catch (Exception)
            {
            }
            return new AccessDetails();
        }

        // initiate the process
        public bool StartProcess(string token)
        {
            // Invoking the process using Deligate
            ProcessEnvironment processTask = new ProcessEnvironment(SetUpGitHubEnvironment);
            processTask.BeginInvoke(token, new AsyncCallback(EndProcess), processTask);
            return true;
        }
        // Revoke the process
        public void EndProcess(IAsyncResult res)
        {
            return;
        }
        // Start of the set up action
        public string[] SetUpGitHubEnvironment(string token)
        {
            string jsonFilePath = Server.MapPath("~") + @"\Json\";
            string baseAddress = System.Configuration.ConfigurationManager.AppSettings["BaseAddress"];

            GitHubConfiguration con = new GitHubConfiguration { baseAddress = baseAddress, mediaType = "application/json", scheme = "Bearer", token = token };
            Users user = new Users(con);

            HttpResponseMessage res = user.GetUserDetail();
            UserDetail userDetail = new UserDetail();
            GitHubRepoResponse.RepoCreated GitHubRepo = new GitHubRepoResponse.RepoCreated();
            if (res.IsSuccessStatusCode)
            {
                userDetail = JsonConvert.DeserializeObject<UserDetail>(res.Content.ReadAsStringAsync().Result);
            }
            if (!string.IsNullOrEmpty(userDetail.login))
            {
                GitHubConfiguration repoCon = new GitHubConfiguration { baseAddress = baseAddress, mediaType = "application/json", scheme = "Bearer", token = token, userName = userDetail.login };
                Repository repo = new Repository(repoCon);

                //Reading Create Repo JSON and Source code import JSON
                string createRepoJson = accessDetails.ReadJsonFile(jsonFilePath + "CreateRepo.json");
                string srcCodeImportJson = accessDetails.ReadJsonFile(jsonFilePath + "SourceImport.json");

                //Deserializing Source code JSON into JObject to get the RepoName by splitting the "vcs_url"
                JObject jobj = JsonConvert.DeserializeObject<JObject>(srcCodeImportJson);
                string repoName = jobj["vcs_url"].ToString().Split('/')[jobj["vcs_url"].ToString().Split('/').Length - 1]; // splitting the "vcs_url" to get repoName
                createRepoJson = createRepoJson.Replace("$RepoName$", repoName); // Replacing RepoName in create repo json, which creates repo with same name
                HttpResponseMessage response = repo.CreateRepository(createRepoJson);
                if (response.StatusCode.ToString() == "422")
                {
                    // if the Repo already exist, this part will be executed, will append the GUID and create the Repo
                    CreateRepo CreateRepo = JsonConvert.DeserializeObject<CreateRepo>(createRepoJson);
                    string guidToAppend = Guid.NewGuid().ToString();
                    guidToAppend = guidToAppend.Substring(0, 8);

                    string RepoName = CreateRepo.name + "_" + guidToAppend;
                    CreateRepo.name = RepoName;
                    string UpdatedJson = JsonConvert.SerializeObject(CreateRepo);
                    response = repo.CreateRepository(UpdatedJson);
                }
                if (response.IsSuccessStatusCode)
                {
                    GitHubRepo = JsonConvert.DeserializeObject<GitHubRepoResponse.RepoCreated>(response.Content.ReadAsStringAsync().Result);
                    CreatedRepoName = GitHubRepo.name;
                }
                // Import the source code to user repo[to newly created repo], this is an async process which takes some time to import repo
                repo.ImportRepository(srcCodeImportJson, GitHubRepo.owner.login, GitHubRepo.name);
            }
            return new string[] { };
        }
    }
}