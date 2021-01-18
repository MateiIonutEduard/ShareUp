using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareUp.Models
{
    public class AdminServiceProvider
    {
        public string host { get; set; }
        public int port { get; set; }
        public string client { get; set; }
        public string secret { get; set; }
    }
}
