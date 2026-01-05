using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlantMetabolitesDB.ViewModel
{
    public class UserReportViewModel
    {
        [DisplayName("User Type")]
        public int UserType { get; set; }
        public List<SelectListItem> UserTypes { get; set; }

    }

    public class UserExportViewModel
    {
        public string Code { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string  Mobile { get; set; }
        public string Country { get; set; }
        public string Organization { get; set; }
        public string Affiliation { get; set; }
        public string Status { get; set; }
        public string RegistrationDate { get; set; }
    }

    //public class StaticData
    //{
    //    public static List<Technology> Technologies
    //    {
    //        get
    //        {
    //            return new List<Technology>{
    //                 new Technology{Name="ASP.NET", Project=12,Developer=50, TeamLeader=6},
    //                new Technology{Name="Php", Project=40,Developer=60, TeamLeader=9},
    //                new Technology{Name="iOS", Project=11,Developer=5, TeamLeader=1},
    //                 new Technology{Name="Android", Project=20,Developer=26, TeamLeader=2}
    //            };
    //        }
    //    }
    //}

    //public class TechnologyViewModel
    //{
    //    public List<Technology> Technologies
    //    {
    //        get
    //        {
    //            return StaticData.Technologies;
    //        }
    //    }
    //}
}