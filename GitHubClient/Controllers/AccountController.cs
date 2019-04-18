using GitHubClient.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace GitHubClient.Controllers
{
    public class AccountController : Controller
    {
        AccessDetails accessDetails = new AccessDetails();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Callback()
        {
            var request = Request;
            string code = Request.QueryString["code"];
            if (!string.IsNullOrEmpty(code))
            {
                string reqUrl = FormatRequestUrl(code);
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
    }
}