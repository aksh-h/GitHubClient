using System.Web.Mvc;

namespace GitHubClient.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            //Request User GitHub Identity
            string ClientID = System.Configuration.ConfigurationManager.AppSettings["ClientID"];
            string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
            string RedirectUrl = System.Configuration.ConfigurationManager.AppSettings["RedirectUrl"];
            string Scope = System.Configuration.ConfigurationManager.AppSettings["Scope"];
            string state = System.Configuration.ConfigurationManager.AppSettings["State"];
            string url = string.Format("https://github.com/login/oauth/authorize?client_id={0}&scope={1}&redirect_uri={2}&state={3}", ClientID, Scope, RedirectUrl, state);
            return Redirect(url);
        }
    }
}