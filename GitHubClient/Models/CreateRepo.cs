using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GitHubClientApp.Models
{
    public class CreateRepo
    {
        public string name { get; set; }
        public string description { get; set; }
        public string homepage { get; set; }
        [JsonProperty(PropertyName = "private")]
        public bool Isprivate { get; set; }
        public bool has_issues { get; set; }
        public bool has_projects { get; set; }
        public bool has_wiki { get; set; }
    }
}