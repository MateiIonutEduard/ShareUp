using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareUp.Models
{
    public class AppSettings : IAppSettings
    {
        public string domain { get; set; }
        public string key { get; set; }
        public string salt { get; set; }
    }

    public interface IAppSettings
    {
        string domain { get; set; }
        string key { get; set; }
        string salt { get; set; }
    }
}
