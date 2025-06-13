using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Onlinemovietickitproject.Models
{
    public class LoginViewModel
    {
        public string EmailOrUsername { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}