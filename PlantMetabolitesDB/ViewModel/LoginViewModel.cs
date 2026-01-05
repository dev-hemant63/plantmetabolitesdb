using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlantMetabolitesDB.ViewModel
{
    public class LoginViewModel
    {
        public int UserKey { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string CaptchaCode { get; set; }
    }
}