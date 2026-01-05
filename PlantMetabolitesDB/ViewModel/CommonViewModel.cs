using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace PlantMetabolitesDB.ViewModel
{
    public class CommonViewModel
    {
        public bool IsActive { get; set; }
        public string StatusName { get; set; }
        public Int16? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Int16? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class CompoundStats
    {
        public int CompoundCount { get; set; }
        public int MS2Count { get; set; }
        public int MS3Count { get; set; }
        public int AductCount { get; set; }
    }

    public class CountryUserCountViewModel
    {
        public string CountryName { get; set; }
        public int UserCount { get; set; }
    }

    public static class AppSettings
    {
        public static string BaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["BaseUrl"];
            }
        }

    }
}