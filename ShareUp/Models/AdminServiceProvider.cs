using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareUp.Models
{
    public class AdminServiceProvider : IAdminServiceProvider
    {
        public string host { get; set; }
        public string port { get; set; }
        public string client { get; set; }
        public string secret { get; set; }
    }

    public interface IAdminServiceProvider
    {
        public string host { get; set; }
        public string port { get; set; }
        public string client { get; set; }
        public string secret { get; set; }
    }
}
