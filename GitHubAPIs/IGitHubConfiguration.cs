using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAPIs
{
    public interface IGitHubConfiguration
    {
        string token { get; set; }
        string scheme { get; set; }
        string mediaType { get; set; }
        string baseAddress { get; set; }
        string userName { get; set; }
    }
}
